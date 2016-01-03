using Photon;

/// <summary>Sample script that uses ColorPerPlayer to apply it to an object's material color.</summary>
public class ColorPerPlayerApply : PunBehaviour
{
    private ColorPerPlayer colorPicker;


    private void Awake()
    {
        this.colorPicker = FindObjectOfType<ColorPerPlayer>() as ColorPerPlayer;
        if (this.colorPicker == null)
        {
            enabled = false;
        }
        if (photonView.isSceneView)
        {
            enabled = false;
        }
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        ApplyColor(); // this applies a color, even before the initial Update() call is done
    }

    // player colors might change. we could react to events but for simplicity, we just check every update.
    private void Update()
    {
        ApplyColor();
    }


    public void ApplyColor()
    {
        if (photonView.owner == null)
        {
            return;
        }

        if (photonView.owner.customProperties.ContainsKey(ColorPerPlayer.ColorProp))
        {
            int playersColorIndex = (int)photonView.owner.customProperties[ColorPerPlayer.ColorProp];
            GetComponent<UnityEngine.Renderer>().material.color = this.colorPicker.Colors[playersColorIndex];
        }
    }
}