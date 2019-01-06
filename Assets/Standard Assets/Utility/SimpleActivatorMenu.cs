using UnityEngine;

namespace UnitySampleAssets.Utility
{

    public class SimpleActivatorMenu : MonoBehaviour
    {

        // An incredibly simple menu which, when given references
        // to gameobjects in the scene

        public GUIText camSwitchButton;
        public GameObject[] objects;
        private int currentActiveObject;

        private void OnEnable()
        {
            // active object starts from first in array
            currentActiveObject = 0;
            camSwitchButton.text = objects[currentActiveObject].name;
        }

        public void NextCamera()
        {

            int nextactiveobject = currentActiveObject + 1 >= objects.Length ? 0 : currentActiveObject + 1;

            for (int i = 0; i < objects.Length; i++)
            {

                objects[i].SetActive(i == nextactiveobject);
            }

            currentActiveObject = nextactiveobject;
            camSwitchButton.text = objects[currentActiveObject].name;

        }
    }
}
