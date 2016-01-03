using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class CubeExtra : Photon.MonoBehaviour
{
    Vector3 latestCorrectPos = Vector3.zero;
    Vector3 lastMovement = Vector3.zero;
    float lastTime = 0;

    public void Awake()
    {
        if (photonView.isMine)
        {
            this.enabled = false;   // Only enable inter/extrapol for remote players
        }

        latestCorrectPos = transform.position;
    }

    // this method is called by PUN when this script is being "observed" by a PhotonView (setup in inspector)
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Always send transform (depending on reliability of the network view)
        if (stream.isWriting)
        {
            Vector3 pos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        // When receiving, buffer the information
        else
        {
            // Receive latest state information
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);

            lastMovement = (pos - latestCorrectPos) / (Time.time - lastTime);

            lastTime = Time.time;
            latestCorrectPos = pos;

            transform.position = latestCorrectPos;
        }
    }

    // This only runs where the component is enabled, which is only on remote peers (server/clients)
    public void Update()
    {
        transform.localPosition += lastMovement * Time.deltaTime;
    }
}
