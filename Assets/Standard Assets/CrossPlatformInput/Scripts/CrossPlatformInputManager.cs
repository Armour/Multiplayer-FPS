using System;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput.PlatformSpecific;


namespace UnitySampleAssets.CrossPlatformInput
{
    public static class CrossPlatformInputManager
    {
        private static VirtualInput virtualInput;


        static CrossPlatformInputManager()
        {
#if MOBILE_INPUT
            virtualInput = new MobileInput ();
#else
            virtualInput = new StandaloneInput();
#endif
        }


        public static void RegisterVirtualAxis(VirtualAxis axis)
        {
            virtualInput.RegisterVirtualAxis(axis);
        }


        public static void RegisterVirtualButton(VirtualButton button)
        {
            virtualInput.RegisterVirtualButton(button);
        }


        public static void UnRegisterVirtualAxis(string _name)
        {
            if (_name == null)
            {
                throw new ArgumentNullException("_name");
            }
            virtualInput.UnRegisterVirtualAxis(_name);
        }


        public static void UnRegisterVirtualButton(string name)
        {
            virtualInput.UnRegisterVirtualButton(name);
        }


        // returns a reference to a named virtual axis if it exists otherwise null
        public static VirtualAxis VirtualAxisReference(string name)
        {
            return virtualInput.VirtualAxisReference(name);
        }


        // returns the platform appropriate axis for the given name
        public static float GetAxis(string name)
        {
            return GetAxis(name, false);
        }


        public static float GetAxisRaw(string name)
        {
            return GetAxis(name, true);
        }


        // private function handles both types of axis (raw and not raw)
        private static float GetAxis(string name, bool raw)
        {
            return virtualInput.GetAxis(name, raw);
        }


        // -- Button handling --
        public static bool GetButton(string name)
        {
            return virtualInput.GetButton(name);
        }


        public static bool GetButtonDown(string name)
        {
            return virtualInput.GetButtonDown(name);
        }


        public static bool GetButtonUp(string name)
        {
            return virtualInput.GetButtonUp(name);
        }


        public static void SetButtonDown(string name)
        {
            virtualInput.SetButtonDown(name);
        }


        public static void SetButtonUp(string name)
        {
            virtualInput.SetButtonUp(name);
        }


        public static void SetAxisPositive(string name)
        {
            virtualInput.SetAxisPositive(name);
        }


        public static void SetAxisNegative(string name)
        {
            virtualInput.SetAxisNegative(name);
        }


        public static void SetAxisZero(string name)
        {
            virtualInput.SetAxisZero(name);
        }


        public static void SetAxis(string name, float value)
        {
            virtualInput.SetAxis(name, value);
        }


        public static Vector3 mousePosition
        {
            get { return virtualInput.MousePosition(); }
        }


        public static void SetVirtualMousePositionX(float f)
        {
            virtualInput.SetVirtualMousePositionX(f);
        }


        public static void SetVirtualMousePositionY(float f)
        {
            virtualInput.SetVirtualMousePositionY(f);
        }


        public static void SetVirtualMousePositionZ(float f)
        {
            virtualInput.SetVirtualMousePositionZ(f);
        }


        // virtual axis and button classes - applies to mobile input
        // Can be mapped to touch joysticks, tilt, gyro, etc, depending on desired implementation.
        // Could also be implemented by other input devices - kinect, electronic sensors, etc
        public class VirtualAxis
        {
            public string name { get; private set; }
            private float m_Value;
            public bool matchWithInputManager { get; private set; }


            public VirtualAxis(string name) : this(name, true)
            {
            }


            public VirtualAxis(string name, bool matchToInputSettings)
            {
                this.name = name;
                matchWithInputManager = matchToInputSettings;
                RegisterVirtualAxis(this);
            }


            // removes an axes from the cross platform input system
            public void Remove()
            {
                UnRegisterVirtualAxis(name);
            }


            // a controller gameobject (eg. a virtual thumbstick) should update this class
            public void Update(float value)
            {
                m_Value = value;
            }


            public float GetValue
            {
                get { return m_Value; }
            }


            public float GetValueRaw
            {
                get { return m_Value; }
            }
        }

        // a controller gameobject (eg. a virtual GUI button) should call the
        // 'pressed' function of this class. Other objects can then read the
        // Get/Down/Up state of this button.
        public class VirtualButton
        {
            public string name { get; private set; }
            private int lastPressedFrame = -5;
            private int releasedFrame = -5;
            private bool pressed;
            public bool matchWithInputManager { get; private set; }


            public VirtualButton(string name) : this(name, true)
            {
            }


            public VirtualButton(string name, bool matchToInputSettings)
            {
                this.name = name;
                matchWithInputManager = matchToInputSettings;
              //  RegisterVirtualButton(this);
            }


            // A controller gameobject should call this function when the button is pressed down
            public void Pressed()
            {
                if (pressed)
                {
                    return;
                }
                pressed = true;
                lastPressedFrame = Time.frameCount;
            }


            // A controller gameobject should call this function when the button is released
            public void Released()
            {
                pressed = false;
                releasedFrame = Time.frameCount;
            }


            // the controller gameobject should call Remove when the button is destroyed or disabled
            public void Remove()
            {
                UnRegisterVirtualButton(name);
            }


            // these are the states of the button which can be read via the cross platform input system
            public bool GetButton
            {
                get { return pressed; }
            }


            public bool GetButtonDown
            {
                get
                {
                    return lastPressedFrame - Time.frameCount == 0;
                }
            }


            public bool GetButtonUp
            {
                get
                {
                    return (releasedFrame == Time.frameCount - 0);
                }
            }
        }
    }
}