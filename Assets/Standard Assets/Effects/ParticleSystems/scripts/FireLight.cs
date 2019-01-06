using UnityEngine;

namespace UnitySampleAssets.Effects
{
    public class FireLight : MonoBehaviour
    {
        private float rnd;
        private bool burning = true;

        private void Start()
        {
            rnd = Random.value*100;
        }

        private void Update()
        {
            if (burning)
            {
                GetComponent<Light>().intensity = 2*Mathf.PerlinNoise(rnd + Time.time, rnd + 1 + Time.time*1);
                float x = Mathf.PerlinNoise(rnd + 0 + Time.time*2, rnd + 1 + Time.time*2) - 0.5f;
                float y = Mathf.PerlinNoise(rnd + 2 + Time.time*2, rnd + 3 + Time.time*2) - 0.5f;
                float z = Mathf.PerlinNoise(rnd + 4 + Time.time*2, rnd + 5 + Time.time*2) - 0.5f;
                transform.localPosition = Vector3.up + new Vector3(x, y, z)*1;
            }
        }

        public void Extinguish()
        {
            burning = false;
            GetComponent<Light>().enabled = false;
        }
    }
}