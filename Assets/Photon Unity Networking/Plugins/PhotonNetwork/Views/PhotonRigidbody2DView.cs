using UnityEngine;
using System.Collections;

/// <summary>
/// This class helps you to synchronize the velocities of a 2d physics RigidBody.
/// Note that only the velocities are synchronized and because Unitys physics
/// engine is not deterministic (ie. the results aren't always the same on all
/// computers) - the actual positions of the objects may go out of sync. If you
/// want to have the position of this object the same on all clients, you should
/// also add a PhotonTransformView to synchronize the position.
/// Simply add the component to your GameObject and make sure that
/// the PhotonRigidbody2DView is added to the list of observed components
/// </summary>
[RequireComponent( typeof( PhotonView ) )]
[RequireComponent( typeof( Rigidbody2D ) )]
[AddComponentMenu("Photon Networking/Photon Rigidbody 2D View")]
public class PhotonRigidbody2DView : MonoBehaviour 
{
    [SerializeField]
    bool m_SynchronizeVelocity = true;

    [SerializeField]
    bool m_SynchronizeAngularVelocity = true;

    Rigidbody2D m_Body;

    void Awake() 
    {
        m_Body = GetComponent<Rigidbody2D>();
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
                m_Body.velocity = (Vector2)stream.ReceiveNext();
            }

            if( m_SynchronizeAngularVelocity == true )
            {
                m_Body.angularVelocity = (float)stream.ReceiveNext();
            }
        }
    }
}
