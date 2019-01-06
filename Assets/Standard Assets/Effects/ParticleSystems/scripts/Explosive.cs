using UnityEngine;
using System.Collections;
using UnitySampleAssets.Utility;

namespace UnitySampleAssets.Effects
{
    public class Explosive : MonoBehaviour
    {

        public Transform explosionPrefab;
        private bool exploded;
        public float detonationImpactVelocity = 10;
        public float sizeMultiplier = 1;
        public bool reset = true;
        public float resetTimeDelay = 10;


        // implementing one method from monobehviour to ensure that the enable/disable tickbox appears in the inspector
        private void Start()
        {
        }

        private IEnumerator OnCollisionEnter(Collision col)
        {
            if (enabled)
            {
                if (col.contacts.Length > 0) 
                {
                    // compare relative velocity to collision normal - so we don't explode from a fast but gentle glancing collision
                    float velocityAlongCollisionNormal =
                        Vector3.Project(col.relativeVelocity, col.contacts[0].normal).magnitude;

                    if (velocityAlongCollisionNormal > detonationImpactVelocity || exploded)
                    {

                        if (!exploded)
                        {
                            Instantiate(explosionPrefab, col.contacts[0].point,
                                        Quaternion.LookRotation(col.contacts[0].normal));
                            exploded = true;

                            SendMessage("Immobilize");

                            if (reset)
                            {
                                GetComponent<ObjectResetter>().DelayedReset(resetTimeDelay);
                            }
                        }
                    }
                }
            }

            yield return null;
        }

        public void Reset()
        {
            exploded = false;
        }
    }
}
