using UnityEngine;
using System.Collections;

/// <summary>
/// This class helps you to synchronize the velocities of a physics RigidBody.
/// Note that only the velocities are synchronized and because Unitys physics
/// engine is not deterministic (ie. the results aren't always the same on all
/// computers) - the actual positions of the objects may go out of sync. If you
/// want to have the position of this object the same on all clients, you should
/// also add a PhotonTransformView to synchronize the position.
/// Simply add the component to your GameObject and make sure that
/// the PhotonRigidbodyView is added to the list of observed components
/// </summary>
[RequireComponent( typeof( PhotonView ) )]
[RequireComponent( typeof( Rigidbody ) )]
[AddComponentMenu("Photon Networking/Photon Rigidbody View")]
public class PhotonRigidbodyView : MonoBehaviour 
{
    [SerializeField]
    bool m_SynchronizeVelocity = true;

    [SerializeField]
    bool m_SynchronizeAngularVelocity = true;

    Rigidbody m_Body;

    void Awake() 
    {
        m_Body = GetComponent<Rigidbody>();
    }

    void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
    {
        if( stream.isWriting == true )
        {
            if( m_SynchronizeVelocity == true )
            {
                stream.SendNext( m_Body.velocity );
            }

            if( m_SynchronizeAngularVelocity == true )
            {
                stream.SendNext( m_Body.angularVelocity );
            }
        }
        else
        {
            if( m_SynchronizeVelocity == true )
            {
                m_Body.velocity = (Vector3)stream.ReceiveNext();
            }

            if( m_SynchronizeAngularVelocity == true )
            {
                m_Body.angularVelocity = (Vector3)stream.ReceiveNext();
            }
        }
    }
}
