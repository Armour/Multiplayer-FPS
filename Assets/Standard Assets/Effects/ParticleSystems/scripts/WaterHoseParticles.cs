using UnityEngine;

namespace UnitySampleAssets.Effects
{
    public class WaterHoseParticles : MonoBehaviour
    {
        private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];

        public static float lastSoundTime;
        public float force = 1;

        private void OnParticleCollision(GameObject other)
        {

            int safeLength = GetComponent<ParticleSystem>().GetSafeCollisionEventSize();

            if (collisionEvents.Length < safeLength)
            {
                collisionEvents = new ParticleCollisionEvent[safeLength];
            }

            // int numCollisionEvents = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);
            // int i = 0;

            // while (i < numCollisionEvents)
            // {

            //     if (Time.time > lastSoundTime + 0.2f)
            //     {
            //         lastSoundTime = Time.time;
            //     }

            //     var col = collisionEvents[i].colliderComponent;

            //     if (col.attachedRigidbody != null)
            //     {
            //         Vector3 vel = collisionEvents[i].velocity;
            //         col.attachedRigidbody.AddForce(vel*force, ForceMode.Impulse);
            //     }

            //     other.BroadcastMessage("Extinguish", SendMessageOptions.DontRequireReceiver);

            //     i++;
            // }
        }
    }
}
