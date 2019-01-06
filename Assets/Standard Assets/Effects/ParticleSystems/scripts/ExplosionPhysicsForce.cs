using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnitySampleAssets.Effects
{
    public class ExplosionPhysicsForce : MonoBehaviour
    {

        public float explosionForce = 4;

        private IEnumerator Start()
        {
            // wait one frame because some explosions instantiate debris which should then
            // be pushed by physics force
            yield return null;

            float multiplier = GetComponent<ParticleSystemMultiplier>().multiplier;

            float r = 10*multiplier;
            Collider[] cols = Physics.OverlapSphere(transform.position, r);
            List<Rigidbody> rigidbodies = new List<Rigidbody>();
            foreach (var col in cols)
            {
                if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
                {
                    rigidbodies.Add(col.attachedRigidbody);
                }
            }
            foreach (var rb in rigidbodies)
            {
                rb.AddExplosionForce(explosionForce*multiplier, transform.position, r, 1*multiplier, ForceMode.Impulse);
            }
        }
    }
}