using UnityEngine;


namespace UnitySampleAssets.Effects
{
    public class ExtinguishableParticleSystem : MonoBehaviour
    {

        public float multiplier = 1;
        private ParticleSystem[] systems;

        private void Start()
        {
            systems = GetComponentsInChildren<ParticleSystem>();
        }

        public void Extinguish()
        {
            foreach (var system in systems)
            {
                system.enableEmission = false;
            }
        }
    }
}