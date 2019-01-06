using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnitySampleAssets.Utility
{

    public class ObjectResetter : MonoBehaviour
    {
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private List<Transform> originalStructure;

        // Use this for initialization
        private void Start()
        {
            originalStructure = new List<Transform>(GetComponentsInChildren<Transform>());
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        public void DelayedReset(float delay)
        {
            StartCoroutine(ResetCoroutine(delay));
        }

        public IEnumerator ResetCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            // remove any gameobjects added (fire, skid trails, etc)
            foreach (var t in GetComponentsInChildren<Transform>())
            {
                if (!originalStructure.Contains(t))
                {
                    t.parent = null;
                }
            }

            transform.position = originalPosition;
            transform.rotation = originalRotation;
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }

            SendMessage("Reset");

        }
    }
}
