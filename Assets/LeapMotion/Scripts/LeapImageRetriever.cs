/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Leap;

// To use the LeapImageRetriever you must be on version 2.1+
// and enable "Allow Images" in the Leap Motion settings.
public class LeapImageRetriever : MonoBehaviour {
    public const string IR_SHADER_VARIANT_NAME = "LEAP_FORMAT_IR";
    public const string RGB_SHADER_VARIANT_NAME = "LEAP_FORMAT_RGB";
    public const string DEPTH_TEXTURE_VARIANT_NAME = "USE_DEPTH_TEXTURE";
    public const int IMAGE_WARNING_WAIT = 10;

    public enum EYE {
        LEFT = 0,
        RIGHT = 1
    }

    public enum SYNC_MODE {
        SYNC_WITH_HANDS,
        LOW_LATENCY
    }

    public EYE eye = (EYE)(-1);
    [Tooltip ("Should the image match the tracked hand, or should it be displayed as fast as possible")]
    public SYNC_MODE syncMode = SYNC_MODE.LOW_LATENCY;
    public float gammaCorrection = 1.0f;


    private int _missedImages = 0;
    private Controller _controller;

    //Information about the current format the retriever is configured for.  Used to detect changes in format
    private int _currentWidth = 0;
    private int _currentHeight = 0;
    private Image.FormatType _currentFormat = (Image.FormatType)(-1);

    //ImageList to use during rendering.  Can either be updated in OnPreRender or in Update
    private ImageList _imageList;

    //Holders for Image Based Materials
    private static List<LeapImageBasedMaterial> _registeredImageBasedMaterials = new List<LeapImageBasedMaterial>();
    private static List<LeapImageBasedMaterial> _imageBasedMaterialsToInit = new List<LeapImageBasedMaterial>();

    // Main texture.
    private Texture2D _mainTexture = null;
    private byte[] _mainTextureData = null;

    //Used to recalculate the distortion every time a hand enters the frame.  Used because there is no way to tell if the device has flipped (which changes the distortion)
    private bool _requestDistortionRecalc = false;
    private bool _forceDistortionRecalc = false;

    // Distortion textures.
    private Texture2D _distortion = null;
    private Color32[] _distortionPixels = null;

    public static void registerImageBasedMaterial(LeapImageBasedMaterial imageBasedMaterial) {
        _registeredImageBasedMaterials.Add(imageBasedMaterial);
        _imageBasedMaterialsToInit.Add(imageBasedMaterial);
    }

    public static void unregisterImageBasedMaterial(LeapImageBasedMaterial imageBasedMaterial) {
        _registeredImageBasedMaterials.Remove(imageBasedMaterial);
    }

    private void initImageBasedMaterial(LeapImageBasedMaterial imageBasedMaterial) {
        Material material = imageBasedMaterial.GetComponent<Renderer>().material;

        switch (_currentFormat) {
            case Image.FormatType.INFRARED:
                material.EnableKeyword(IR_SHADER_VARIANT_NAME);
                material.DisableKeyword(RGB_SHADER_VARIANT_NAME);
                break;
            case (Image.FormatType)4:
                material.EnableKeyword(RGB_SHADER_VARIANT_NAME);
                material.DisableKeyword(IR_SHADER_VARIANT_NAME);
                break;
            default:
                Debug.LogWarning("Unexpected format type " + _currentFormat);
                break;
        }

        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)) {
            material.EnableKeyword(DEPTH_TEXTURE_VARIANT_NAME);
        } else {
            material.DisableKeyword(DEPTH_TEXTURE_VARIANT_NAME);
        }

        imageBasedMaterial.GetComponent<Renderer>().material.SetFloat("_LeapGammaCorrectionExponent", 1.0f / gammaCorrection);
    }

    private void updateImageBasedMaterial(LeapImageBasedMaterial imageBasedMaterial, ref Image image) {
        imageBasedMaterial.GetComponent<Renderer>().material.SetTexture("_LeapTexture", _mainTexture);

        Vector4 projection = new Vector4();
        projection.x = GetComponent<Camera>().projectionMatrix[0, 2];
        projection.z = GetComponent<Camera>().projectionMatrix[0, 0];
        projection.w = GetComponent<Camera>().projectionMatrix[1, 1];
        imageBasedMaterial.GetComponent<Renderer>().material.SetVector("_LeapProjection", projection);

        if (_distortion == null) {
            initDistortion(image);
            loadDistortion(image);
            _forceDistortionRecalc = false;
        }

        if (_forceDistortionRecalc || (_requestDistortionRecalc && _controller.Frame().Hands.Count != 0)) {
            loadDistortion(image);
            _requestDistortionRecalc = false;
            _forceDistortionRecalc = false;
        }

        imageBasedMaterial.GetComponent<Renderer>().material.SetTexture("_LeapDistortion", _distortion);
    }

    private TextureFormat getTextureFormat(Image image) {
        switch (image.Format) {
            case Image.FormatType.INFRARED:
                return TextureFormat.Alpha8;
            case (Image.FormatType)4:
                return TextureFormat.RGBA32;
            default:
                throw new System.Exception("Unexpected image format!");
        }
    }

    private int bytesPerPixel(TextureFormat format) {
        switch (format) {
            case TextureFormat.Alpha8: return 1;
            case TextureFormat.RGBA32:
            case TextureFormat.BGRA32:
            case TextureFormat.ARGB32: return 4;
            default: throw new System.Exception("Unexpected texture format " + format);
        }
    }

    private int totalBytes(Texture2D texture) {
        return texture.width * texture.height * bytesPerPixel(texture.format);
    }

    private void initMainTexture(Image image) {
        TextureFormat format = getTextureFormat(image);

        if (_mainTexture != null) {
            DestroyImmediate(_mainTexture);
        }

        _mainTexture = new Texture2D(image.Width, image.Height, format, false, true);
        _mainTexture.wrapMode = TextureWrapMode.Clamp;
        _mainTexture.filterMode = FilterMode.Bilinear;
        _mainTextureData = new byte[_mainTexture.width * _mainTexture.height * bytesPerPixel(format)];
    }

    private void loadMainTexture(Image sourceImage) {
        Marshal.Copy(sourceImage.DataPointer(), _mainTextureData, 0, _mainTextureData.Length);
        _mainTexture.LoadRawTextureData(_mainTextureData);
        _mainTexture.Apply();
    }

    private void initDistortion(Image image) {
        int width = image.DistortionWidth / 2;
        int height = image.DistortionHeight;

        _distortionPixels = new Color32[width * height];
        if (_distortion != null) {
            DestroyImmediate(_distortion);
        }
        _distortion = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
        _distortion.wrapMode = TextureWrapMode.Clamp;
    }

    private void encodeFloat(float value, out byte byte0, out byte byte1) {
        // The distortion range is -0.6 to +1.7. Normalize to range [0..1).
        value = (value + 0.6f) / 2.3f;
        float enc_0 = value;
        float enc_1 = value * 255.0f;

        enc_0 = enc_0 - (int)enc_0;
        enc_1 = enc_1 - (int)enc_1;

        enc_0 -= 1.0f / 255.0f * enc_1;

        byte0 = (byte)(enc_0 * 256.0f);
        byte1 = (byte)(enc_1 * 256.0f);
    }

    private void loadDistortion(Image image) {
        float[] distortionData = image.Distortion;

        // Move distortion data to distortion texture
        for (int i = 0; i < distortionData.Length; i += 2) {
            byte b0, b1, b2, b3;
            encodeFloat(distortionData[i], out b0, out b1);
            encodeFloat(distortionData[i + 1], out b2, out b3);
            _distortionPixels[i / 2] = new Color32(b0, b1, b2, b3);
        }

        _distortion.SetPixels32(_distortionPixels);
        _distortion.Apply();
    }

    void Start() {
        HandController handController = FindObjectOfType<HandController>();
        if (handController == null) {
            Debug.LogWarning("Cannot use LeapImageRetriever if there is no HandController in the scene!");
            enabled = false;
            return;
        }

        _controller = handController.GetLeapController();
        _controller.SetPolicy(Controller.PolicyFlag.POLICY_IMAGES);
    }

    void Update() {
        Frame frame = _controller.Frame();

        if (frame.Hands.Count == 0) {
            _requestDistortionRecalc = true;
        }

        if (syncMode == SYNC_MODE.SYNC_WITH_HANDS) {
            _imageList = frame.Images;
        }
    }

    void OnPreRender() {
        if (syncMode == SYNC_MODE.LOW_LATENCY) {
            _imageList = _controller.Images;
        }

        Image referenceImage = _imageList[(int)eye];

        if (referenceImage.Width == 0 || referenceImage.Height == 0) {
            _missedImages++;
            if (_missedImages == IMAGE_WARNING_WAIT) {
                Debug.LogWarning("Can't find any images. " +
                                  "Make sure you enabled 'Allow Images' in the Leap Motion Settings, " +
                                  "you are on tracking version 2.1+ and " +
                                  "your Leap Motion device is plugged in.");
            }
            return;
        }

        if (referenceImage.Height != _currentHeight || referenceImage.Width != _currentWidth || referenceImage.Format != _currentFormat) {
            initMainTexture(referenceImage);

            _currentHeight = referenceImage.Height;
            _currentWidth = referenceImage.Width;
            _currentFormat = referenceImage.Format;

            _imageBasedMaterialsToInit.Clear();
            _imageBasedMaterialsToInit.AddRange(_registeredImageBasedMaterials);

            _forceDistortionRecalc = true;
        }

        loadMainTexture(referenceImage);

        for (int i = _imageBasedMaterialsToInit.Count - 1; i >= 0; i--) {
            LeapImageBasedMaterial material = _imageBasedMaterialsToInit[i];
            initImageBasedMaterial(material);
            _imageBasedMaterialsToInit.RemoveAt(i);
        }

        foreach (LeapImageBasedMaterial material in _registeredImageBasedMaterials) {
            if (material.imageMode == LeapImageBasedMaterial.ImageMode.STEREO ||
               (material.imageMode == LeapImageBasedMaterial.ImageMode.LEFT_ONLY && eye == EYE.LEFT) ||
               (material.imageMode == LeapImageBasedMaterial.ImageMode.RIGHT_ONLY && eye == EYE.RIGHT)) {
                updateImageBasedMaterial(material, ref referenceImage);
            }
        }
    }
}
