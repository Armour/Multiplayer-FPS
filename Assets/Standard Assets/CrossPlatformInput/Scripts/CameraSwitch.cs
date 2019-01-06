using UnityEngine;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour {

    public GameObject[] objects;
    private int currentActiveObject;

    public Text text;


    void OnEnable () {
        text.text = objects[currentActiveObject].name;
    }

    public void NextCamera()
    {

        int nextactiveobject = currentActiveObject + 1 >= objects.Length ? 0 : currentActiveObject + 1;

        for (int i = 0; i < objects.Length; i++)
        {

            objects[i].SetActive(i == nextactiveobject);
        }

        currentActiveObject = nextactiveobject;
        text.text = objects[currentActiveObject].name;
    }
}
