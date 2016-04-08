using UnityEngine;
using System;
using System.Collections;

namespace LMWidgets
{
  public abstract class ButtonBase : LeapPhysicsSpring, BinaryInteractionHandler<bool>
  {
    // Binary Interaction Handler - Fires when interaction with the widget starts.
    public event EventHandler<LMWidgets.EventArg<bool>> StartHandler;
    // Binary Interaction Handler - Fires when interaction with the widget ends.
    public event EventHandler<LMWidgets.EventArg<bool>> EndHandler;

    public float triggerDistance = 0.025f;
    public float cushionThickness = 0.005f;

    protected float scaled_spring_;
    protected float scaled_trigger_distance_;
    protected float scaled_cushion_thickness_;

    protected float min_distance_;
    protected float max_distance_;

    protected float m_localTriggerDistance;
    protected float m_localCushionThickness;
    protected bool m_isPressed = false;

    protected virtual void buttonReleased ()
    {
      FireButtonEnd ();
    }

    protected virtual void buttonPressed ()
    {
      FireButtonStart ();
    }

    protected void FireButtonStart (bool value = true)
    {
      EventHandler<LMWidgets.EventArg<bool>> handler = StartHandler;
      if (handler != null) {
        handler (this, new LMWidgets.EventArg<bool> (value));
      }
    }

    protected void FireButtonEnd (bool value = false)
    {
      EventHandler<LMWidgets.EventArg<bool>> handler = EndHandler;
      if (handler != null) {
        handler (this, new LMWidgets.EventArg<bool> (value));
      }
    }


    /// <summary>
    /// Returns the fraction of position of the button between rest and trigger. 0.0 = At Rest. 1.0 = At Triggered Distance.
    /// </summary>
    /// <returns>fraction</returns>
    public float GetFraction()
    {
      if (triggerDistance == 0.0f)
        return 0.0f;
      else
      {
        float fraction = transform.localPosition.z / m_localTriggerDistance;
        return Mathf.Clamp(fraction, 0.0f, 1.0f);
      }
    }

    /// <summary>
    ///  Constrain the button to the z-axis
    /// </summary>
    protected override void ApplyConstraints()
    {
      Vector3 localPosition = transform.localPosition;
      localPosition.x = 0.0f;
      localPosition.y = 0.0f;
      localPosition.z = Mathf.Max(localPosition.z, 0.0f);
      transform.localPosition = localPosition;
    }

    /// <summary>
    /// Check if the button is being pressed or not
    /// </summary>
    private void CheckTrigger()
    {
      float scale = transform.lossyScale.z;
      m_localTriggerDistance = triggerDistance / scale;
      m_localCushionThickness = Mathf.Clamp(cushionThickness / scale, 0.0f, m_localTriggerDistance - 0.001f);
      if (m_isPressed == false)
      {
        if (transform.localPosition.z > m_localTriggerDistance)
        {
          m_isPressed = true;
          buttonPressed();
        }
      }
      else if (m_isPressed == true)
      {
        if (transform.localPosition.z < (m_localTriggerDistance - m_localCushionThickness))
        {
          m_isPressed = false;
          buttonReleased();
        }
      }
    }

    protected virtual void Start()
    {
      cushionThickness = Mathf.Clamp(cushionThickness, 0.0f, triggerDistance - 0.001f);
    }

    protected virtual void Update()
    {
      CheckTrigger();
    }
  }
}
