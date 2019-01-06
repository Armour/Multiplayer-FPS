using UnityEngine;

namespace UnitySampleAssets.Utility
{

    public class TimedObjectDestructor : MonoBehaviour
    {
        private float timeOut = 1.0f;
        private bool detachChildren = false;

        private void Awake()
        {
            Invoke("DestroyNow", timeOut);
        }

        private void DestroyNow()
        {
            if (detachChildren)
            {
                transform.DetachChildren();
            }
            DestroyObject(gameObject);
        }
    }
}