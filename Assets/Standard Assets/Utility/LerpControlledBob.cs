using System;
using UnityEngine;
using System.Collections;

namespace UnitySampleAssets.Utility
{
    [Serializable]
    public class LerpControlledBob
    {
        public float BobDuration;
        public float BobAmount;
        private float offset = 0f;


        // provides the offset that can be used 
        public float Offset()
        {
            return offset;
        }


        public IEnumerator DoBobCycle()
        {

            // make the camera move down slightly
            float t = 0f;
            while (t < BobDuration)
            {
                offset = Mathf.Lerp(0f, BobAmount, t/BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            // make it move back to neutral 
            t = 0f;
            while (t < BobDuration)
            {
                offset = Mathf.Lerp(BobAmount, 0f, t/BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            offset = 0f;
        }
    }
}