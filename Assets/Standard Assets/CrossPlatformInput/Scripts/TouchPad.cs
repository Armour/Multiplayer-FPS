using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnitySampleAssets.CrossPlatformInput;


[RequireComponent(typeof(Image))]
public class TouchPad : MonoBehaviour , IPointerDownHandler , IPointerUpHandler
{

    public enum AxisOption
    {                                                    // Options for which axes to use                                                     
        Both,                                                                   // Use both
        OnlyHorizontal,                                                         // Only horizontal
        OnlyVertical                                                            // Only vertical
    }

    public enum ControlStyle
    {
        Absolute,               // operates from teh center of the image
        Relative,                // operates from the center of the initial touch
        Swipe,                  // swipe to touch touch no maintained center
    }

    public AxisOption axesToUse = AxisOption.Both;   // The options for the axes that the still will use
    public ControlStyle controlStyle = ControlStyle.Absolute;   // control style to use
    public string horizontalAxisName = "Horizontal";// The name given to the horizontal axis for the cross platform input
    public string verticalAxisName = "Vertical";    // The name given to the vertical axis for the cross platform input 
    public float sensitivity = 1f;

    private Vector3 startPos;
    private Vector2 previousDelta;
    private Vector3 m_JoytickOutput;
    private bool useX;                                                          // Toggle for using the x axis
    private bool useY;                                                          // Toggle for using the Y axis
    private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;               // Reference to the joystick in the cross platform input
    private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;                 // Reference to the joystick in the cross platform input
    private bool dragging = false;
    private int id =-1;


#if !UNITY_EDITOR
    private Vector3 center;
    private Image image;
#endif
    private Vector2 previousTouchPos;                                           // swipe style control touch 

#if UNITY_EDITOR
    private Vector3 previousMouse;
#endif

    void OnEnable()
    {
        CreateVirtualAxes();
#if !UNITY_EDITOR
        image = GetComponent<Image>();
        center = image.transform.position;
#endif
    }


    private void CreateVirtualAxes()
    {
        // set axes to use
        useX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
        useY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

        // create new axes based on axes to use
        if (useX)
            horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
        if (useY)
            verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
    }

    private void UpdateVirtualAxes(Vector3 value)
    {

        value = value.normalized;
        if (useX)
            horizontalVirtualAxis.Update(value.x);

        if (useY)
            verticalVirtualAxis.Update(value.y);

    }

    public void OnPointerDown(PointerEventData data)
    {
        dragging = true;
        id = data.pointerId;

#if !UNITY_EDITOR
        if (controlStyle != ControlStyle.Absolute )
            center = data.position;
#endif
    }

    void Update () {
        if (!dragging) {
            return;
        }
        if (Input.touchCount >= id && id != -1) {
#if !UNITY_EDITOR

            if (controlStyle == ControlStyle.Swipe)
            {
                center = previousTouchPos;
                previousTouchPos = Input.touches[id].position;
            }
            Vector2 pointerDelta = new Vector2(Input.touches[id].position.x - center.x , Input.touches[id].position.y - center.y).normalized * sensitivity;
#else
            Vector2 pointerDelta;;
            pointerDelta.x = Input.mousePosition.x - previousMouse.x;
            pointerDelta.y = Input.mousePosition.y - previousMouse.y;
            previousMouse = new Vector3(Input.mousePosition.x , Input.mousePosition.y , 0f);
#endif
            UpdateVirtualAxes (new Vector3 (pointerDelta.x, pointerDelta.y, 0));
        }
    }


    public void OnPointerUp(PointerEventData data) {
        dragging = false;
        id = -1;
        UpdateVirtualAxes(Vector3.zero);
    }


}
