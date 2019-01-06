using UnityEngine;
using System.Collections;

namespace UnitySampleAssets.Utility
{

    public class ParticleSystemDestroyer : MonoBehaviour
    {

        // allows a particle system to exist for a specified duration,
        // then shuts off emission, and waits for all particles to expire
        // before destroying the gameObject

        public float minDuration = 8;
        public float maxDuration = 10;
        private float maxLifetime;
        private bool earlyStop = false;

        private IEnumerator Start()
        {
            var systems = GetComponentsInChildren<ParticleSystem>();

            // find out the maximum lifetime of any particles in this effect
            foreach (var system in systems)
            {
                maxLifetime = Mathf.Max(system.startLifetime, maxLifetime);
            }

            // wait for random duration

            float stopTime = Time.time + Random.Range(minDuration, maxDuration);

            while (Time.time < stopTime || earlyStop)
            {
                yield return null;
            }
            Debug.Log("stopping " + name);

            // turn off emission
            foreach (var system in systems)
            {
                system.enableEmission = false;
            }
            BroadcastMessage("Extinguish", SendMessageOptions.DontRequireReceiver);

            // wait for any remaining particles to expire
            yield return new WaitForSeconds(maxLifetime);

            Destroy(gameObject);

        }

        public void Stop()
        {
            // stops the particle system early
            earlyStop = true;

        }
    }
}