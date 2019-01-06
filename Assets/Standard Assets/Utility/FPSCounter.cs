using UnityEngine;

namespace UnitySampleAssets.Utility
{
    [RequireComponent(typeof (GUIText))]
    public class FPSCounter : MonoBehaviour
    {
        private float fpsMeasurePeriod = 0.5f;
        private int fpsAccumulator = 0;
        private float fpsNextPeriod = 0;
        private int currentFps;
        private string display = "{0} FPS";

        private void Start()
        {
            fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        }

        private void Update()
        {
            // measure average frames per second
            fpsAccumulator++;
            if (Time.realtimeSinceStartup > fpsNextPeriod)
            {
                currentFps = (int) (fpsAccumulator/fpsMeasurePeriod);
                fpsAccumulator = 0;
                fpsNextPeriod += fpsMeasurePeriod;
                GetComponent<GUIText>().text = string.Format(display, currentFps);
            }
        }
    }
}
