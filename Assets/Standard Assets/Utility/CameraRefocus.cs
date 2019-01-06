using UnityEngine;

namespace UnitySampleAssets.Utility
{
    public class CameraRefocus
    {
        public Camera Camera;
        public Vector3 Lookatpoint;
        public Transform Parent;
        private Vector3 OrigCameraPos;
        private bool refocus;

        public CameraRefocus(Camera camera, Transform parent, Vector3 origCameraPos)
        {
            OrigCameraPos = origCameraPos;
            Camera = camera;
            Parent = parent;
        }

        public void ChangeCamera(Camera camera)
        {
            Camera = camera;
        }

        public void ChangeParent(Transform parent)
        {
            Parent = parent;
        }

        public void GetFocusPoint()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Parent.transform.position + OrigCameraPos, Parent.transform.forward, out hitInfo,
                                Camera.farClipPlane))
            {
                Lookatpoint = hitInfo.point;
                refocus = true;
                return;
            }
            refocus = false;
        }

        public void SetFocusPoint()
        {
            if (refocus)
                Camera.transform.LookAt(Lookatpoint);
        }
    }
}