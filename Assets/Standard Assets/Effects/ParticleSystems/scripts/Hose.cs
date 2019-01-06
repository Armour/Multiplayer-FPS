using UnityEngine;


namespace UnitySampleAssets.Effects
{
    public class Hose : MonoBehaviour
    {

        public float maxPower = 20;
        public float minPower = 5;
        public float changeSpeed = 5;

        private float power;
        public ParticleSystem[] hoseWaterSystems;

        // Update is called once per frame
        private void Update()
        {

            if (Input.GetMouseButton(0))
            {
                power = Mathf.Lerp(power, maxPower, Time.deltaTime*changeSpeed);
            }
            else
            {
                power = Mathf.Lerp(power, minPower, Time.deltaTime*changeSpeed);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var sys = transform.Find("Callback Particles").GetComponent<ParticleSystem>();
                sys.GetComponent<Renderer>().enabled = !sys.GetComponent<Renderer>().enabled;
            }

            //audio.volume = Mathf.InverseLerp(0,maxPower,power);

            foreach (var system in hoseWaterSystems)
            {
                system.startSpeed = power;
                system.enableEmission = (power > minPower*1.1f);
            }
        }
    }
}