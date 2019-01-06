using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UnitySampleAssets.CrossPlatformInput
{
    // helps with managing tilt input on mobile devices
    public class TiltInput : MonoBehaviour
    {
        // options for the various orientations
        public enum AxisOptions
        {
            ForwardAxis,
            SidewaysAxis,
        }

        public AxisMapping mapping;
        //public string axisName = "Horizontal";
        public AxisOptions tiltAroundAxis = AxisOptions.ForwardAxis;
        public float fullTiltAngle = 25;
        public float centreAngleOffset = 0;
        private CrossPlatformInputManager.VirtualAxis steerAxis;

        // Use this for initialization
        private void OnEnable()
        {
            if (mapping.type == AxisMapping.MappingType.NamedAxis)
            {
                steerAxis = new CrossPlatformInputManager.VirtualAxis(mapping.axisName);
            }
        }


        // Update is called once per frame
        private void Update()
        {
            float angle = 0;
            if (Input.acceleration != Vector3.zero)
            {
                switch (tiltAroundAxis)
                {
                    case AxisOptions.ForwardAxis:
                        angle = Mathf.Atan2(Input.acceleration.x, -Input.acceleration.y)*Mathf.Rad2Deg +
                                centreAngleOffset;
                        break;
                    case AxisOptions.SidewaysAxis:
                        angle = Mathf.Atan2(Input.acceleration.z, -Input.acceleration.y)*Mathf.Rad2Deg +
                                centreAngleOffset;
                        break;
                }
            }

            float axisValue = Mathf.InverseLerp(-fullTiltAngle, fullTiltAngle, angle)*2 - 1;
            switch (mapping.type)
            {
                case AxisMapping.MappingType.NamedAxis:
                    steerAxis.Update(axisValue);
                    break;
                case AxisMapping.MappingType.MousePositionX:
                    CrossPlatformInputManager.SetVirtualMousePositionX(axisValue*Screen.width);
                    break;
                case AxisMapping.MappingType.MousePositionY:
                    CrossPlatformInputManager.SetVirtualMousePositionY(axisValue*Screen.width);
                    break;
                case AxisMapping.MappingType.MousePositionZ:
                    CrossPlatformInputManager.SetVirtualMousePositionZ(axisValue*Screen.width);
                    break;
            }
        }


        private void OnDisable()
        {
            steerAxis.Remove();
        }


        [System.Serializable]
        public class AxisMapping
        {
            public enum MappingType
            {
                NamedAxis,
                MousePositionX,
                MousePositionY,
                MousePositionZ
            };

            public MappingType type;
            public string axisName;
        }
    }
}

namespace UnitySampleAssets.CrossPlatformInput.Inspector
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof (TiltInput.AxisMapping))]
    public class TiltInputAxisStylePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float x = position.x;
            float y = position.y;
            float inspectorWidth = position.width;

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            string[] props = new string[] {"type", "axisName"};
            float[] widths = new float[] {.4f, .6f};
            if (property.FindPropertyRelative("type").enumValueIndex > 0)
            {
                // hide name if not a named axis
                props = new string[] {"type"};
                widths = new float[] {1};
            }
            float lineHeight = 18;
            for (int n = 0; n < props.Length; ++n)
            {
                float w = widths[n]*inspectorWidth;

                // Calculate rects
                Rect rect = new Rect(x, y, w, lineHeight);
                x += w;

                EditorGUI.PropertyField(rect, property.FindPropertyRelative(props[n]), GUIContent.none);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
#endif
}