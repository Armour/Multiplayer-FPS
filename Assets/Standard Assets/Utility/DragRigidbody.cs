using System.Collections;
using UnityEngine;

namespace UnitySampleAssets.Utility
{
    public class DragRigidbody : MonoBehaviour
    {
        private float spring = 50.0f;
        private float damper = 5.0f;
        private float drag = 10.0f;
        private float angularDrag = 5.0f;
        private float distance = 0.2f;
        private bool attachToCenterOfMass = false;

        private SpringJoint springJoint;

        private void Update()
        {
            // Make sure the user pressed the mouse down
            if (!Input.GetMouseButtonDown(0))
                return;

            var mainCamera = FindCamera();

            // We need to actually hit an object
            RaycastHit hit = new RaycastHit();
            if (
                !Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition).origin,
                                 mainCamera.ScreenPointToRay(Input.mousePosition).direction, out hit, 100,
                                 Physics.DefaultRaycastLayers))
                return;
            // We need to hit a rigidbody that is not kinematic
            if (!hit.rigidbody || hit.rigidbody.isKinematic)
                return;

            if (!springJoint)
            {
                var go = new GameObject("Rigidbody dragger");
                Rigidbody body = go.AddComponent<Rigidbody>() as Rigidbody;
                springJoint = (SpringJoint) go.AddComponent<SpringJoint>();
                body.isKinematic = true;
            }

            springJoint.transform.position = hit.point;
            if (attachToCenterOfMass)
            {
                var anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
                anchor = springJoint.transform.InverseTransformPoint(anchor);
                springJoint.anchor = anchor;
            }
            else
            {
                springJoint.anchor = Vector3.zero;
            }

            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.maxDistance = distance;
            springJoint.connectedBody = hit.rigidbody;

            StartCoroutine("DragObject", hit.distance);
        }

        private IEnumerator DragObject(float distance)
        {
            var oldDrag = springJoint.connectedBody.drag;
            var oldAngularDrag = springJoint.connectedBody.angularDrag;
            springJoint.connectedBody.drag = drag;
            springJoint.connectedBody.angularDrag = angularDrag;
            var mainCamera = FindCamera();
            while (Input.GetMouseButton(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                springJoint.transform.position = ray.GetPoint(distance);
                yield return null;
            }
            if (springJoint.connectedBody)
            {
                springJoint.connectedBody.drag = oldDrag;
                springJoint.connectedBody.angularDrag = oldAngularDrag;
                springJoint.connectedBody = null;
            }
        }

        private Camera FindCamera()
        {
            if (GetComponent<Camera>())
                return GetComponent<Camera>();
            
                return Camera.main;
        }
    }
}