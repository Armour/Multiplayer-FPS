using UnityEngine;
using UnityEngine.EventSystems;

public class LevelReset : MonoBehaviour , IPointerClickHandler
{

	public void OnPointerClick (PointerEventData data) {

        // reload the scene
        Application.LoadLevelAsync(Application.loadedLevelName);
	}

    private void Update()
    {
    }
}
