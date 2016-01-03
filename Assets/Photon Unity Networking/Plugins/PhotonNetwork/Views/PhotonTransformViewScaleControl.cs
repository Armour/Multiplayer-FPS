using UnityEngine;
using System.Collections;

public class PhotonTransformViewScaleControl 
{
    PhotonTransformViewScaleModel m_Model;
    Vector3 m_NetworkScale = Vector3.one;

    public PhotonTransformViewScaleControl( PhotonTransformViewScaleModel model )
    {
        m_Model = model;
    }

    public Vector3 GetScale( Vector3 currentScale )
    {
        switch( m_Model.InterpolateOption )
        {
        default:
        case PhotonTransformViewScaleModel.InterpolateOptions.Disabled:
            return m_NetworkScale;
        case PhotonTransformViewScaleModel.InterpolateOptions.MoveTowards:
            return Vector3.MoveTowards( currentScale, m_NetworkScale, m_Model.InterpolateMoveTowardsSpeed * Time.deltaTime );
        case PhotonTransformViewScaleModel.InterpolateOptions.Lerp:
            return Vector3.Lerp( currentScale, m_NetworkScale, m_Model.InterpolateLerpSpeed * Time.deltaTime );
        }
    }

    public void OnPhotonSerializeView( Vector3 currentScale, PhotonStream stream, PhotonMessageInfo info )
    {
        if( m_Model.SynchronizeEnabled == false )
        {
            return;
        }

        if( stream.isWriting == true )
        {
            stream.SendNext( currentScale );
            m_NetworkScale = currentScale;
        }
        else
        {
            m_NetworkScale = (Vector3)stream.ReceiveNext();
        }
    }
}
