using UnityEngine;

namespace UnitySampleAssets.Cameras
{
    public class HandHeldCam : LookatTarget
    {

        [SerializeField] private float swaySpeed = .5f;
        [SerializeField] private float baseSwayAmount = .5f;
        [SerializeField] private float trackingSwayAmount = .5f;
        [Range(-1, 1)] [SerializeField] private float trackingBias = 0;

        protected override void FollowTarget(float deltaTime)
        {
            base.FollowTarget(deltaTime);

            float bx = (Mathf.PerlinNoise(0, Time.time*swaySpeed) - 0.5f);
            float by = (Mathf.PerlinNoise(0, (Time.time*swaySpeed) + 100)) - 0.5f;

            bx *= baseSwayAmount;
            by *= baseSwayAmount;

            float tx = (Mathf.PerlinNoise(0, Time.time*swaySpeed) - 0.5f) + trackingBias;
            float ty = ((Mathf.PerlinNoise(0, (Time.time*swaySpeed) + 100)) - 0.5f) + trackingBias;

            tx *= -trackingSwayAmount*followVelocity.x;
            ty *= trackingSwayAmount*followVelocity.y;

            transform.Rotate(bx + tx, by + ty, 0);

        }
    }
}