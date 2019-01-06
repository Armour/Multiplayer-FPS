using UnityEngine;

namespace UnitySampleAssets.Effects
{
    public class SmokeParticles : MonoBehaviour
    {

        public AudioClip[] extinguishSounds;

        private void Start()
        {
            GetComponent<AudioSource>().clip = extinguishSounds[Random.Range(0, extinguishSounds.Length)];
            GetComponent<AudioSource>().Play();
        }
    }
}