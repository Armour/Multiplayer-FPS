using UnityEngine;

namespace UnitySampleAssets.Effects
{
    [RequireComponent(typeof (SphereCollider))]
    public class AfterburnerPhysicsForce : MonoBehaviour
    {

        public float effectAngle = 15;
        public float effectWidth = 1;
        public float effectDistance = 10;
        public float force = 10;
        private Collider[] cols;
        private float r;

        private SphereCollider sphere;

        private void Start()
        {
            sphere = (GetComponent<Collider>() as SphereCollider);
        }

        private void FixedUpdate()
        {
            cols = Physics.OverlapSphere(transform.position + sphere.center, sphere.radius);
            for (int n = 0; n < cols.Length; ++n)
            {
                if (cols[n].attachedRigidbody != null)
                {
                    Vector3 localPos = transform.InverseTransformPoint(cols[n].transform.position);
                    localPos = Vector3.MoveTowards(localPos, new Vector3(0, 0, localPos.z), effectWidth*0.5f);
                    float angle = Mathf.Abs(Mathf.Atan2(localPos.x, localPos.z)*Mathf.Rad2Deg);
                    float falloff = Mathf.InverseLerp(effectDistance, 0, localPos.magnitude);
                    falloff *= Mathf.InverseLerp(effectAngle, 0, angle);
                    Vector3 delta = cols[n].transform.position - transform.position;
                    cols[n].attachedRigidbody.AddForceAtPosition(delta.normalized*force*falloff,
                                                                 Vector3.Lerp(cols[n].transform.position,
                                                                              transform.TransformPoint(0, 0, localPos.z),
                                                                              0.1f));
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            (GetComponent<Collider>() as SphereCollider).radius = effectDistance*.5f;
            (GetComponent<Collider>() as SphereCollider).center = new Vector3(0, 0, effectDistance*.5f);
            Vector3[] directions = new Vector3[] {Vector3.up, -Vector3.up, Vector3.right, -Vector3.right};
            Vector3[] perpDirections = new Vector3[] {-Vector3.right, Vector3.right, Vector3.up, -Vector3.up};
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            for (int n = 0; n < 4; ++n)
            {
                Vector3 origin = transform.position + transform.rotation*directions[n]*effectWidth*0.5f;

                Vector3 direction =
                    transform.TransformDirection(Quaternion.AngleAxis(effectAngle, perpDirections[n])*Vector3.forward);

                Gizmos.DrawLine(origin, origin + direction*(GetComponent<Collider>() as SphereCollider).radius*2);
            }


        }

    }
}