using System;
using UnityEngine;


namespace UnitySampleAssets.Utility
{
    [Serializable]
    public class CurveControlledBob
    {
        private float _bobBaseInterval;
        private Vector3 _originalCameraPosition;
        private float _time;

        public float HorizontalBobRange = 0.33f;
        public float VerticalBobRange = 0.33f;

        public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                                                            new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                                                            new Keyframe(2f, 0f)); // sin curve for head bob

        private float _cyclePositionX;
        private float _cyclePositionY;

        public float VerticaltoHorizontalRatio = 1f;

        public void Setup(Camera camera, float bobBaseInterval)
        {
            _bobBaseInterval = bobBaseInterval;
            _originalCameraPosition = camera.transform.localPosition;

            // get the length of the curve in time
            _time = Bobcurve[Bobcurve.length - 1].time;
        }

        public Vector3 DoHeadBob(float speed)
        {

            float xPos = _originalCameraPosition.x + (Bobcurve.Evaluate(_cyclePositionX)*HorizontalBobRange);
            float yPos = _originalCameraPosition.y + (Bobcurve.Evaluate(_cyclePositionY)*VerticalBobRange);

            _cyclePositionX += (speed*Time.deltaTime)/_bobBaseInterval;
            _cyclePositionY += ((speed*Time.deltaTime)/_bobBaseInterval)*VerticaltoHorizontalRatio;

            if (_cyclePositionX > _time) _cyclePositionX = _cyclePositionX - _time;
            if (_cyclePositionY > _time) _cyclePositionY = _cyclePositionY - _time;

            return new Vector3(xPos, yPos, 0f);
        }
    }
}