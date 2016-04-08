using UnityEngine;
using System;
using System.Collections;

namespace LMWidgets
{
  public abstract class SliderBase : LeapPhysicsSpring, AnalogInteractionHandler<float>, IDataBoundWidget<SliderBase, float>
  {
    protected DataBinderSlider m_dataBinder;

    // Binary Interaction Handler - Fires when interaction with the widget starts.
    public event EventHandler<LMWidgets.EventArg<float>> StartHandler;
    // Analog Interaction Handler - Fires while widget is being interacted with.
    public event EventHandler<LMWidgets.EventArg<float>> ChangeHandler;
    // Binary Interaction Handler - Fires when interaction with the widget ends.
    public event EventHandler<LMWidgets.EventArg<float>> EndHandler;

    public GameObject upperLimit;
    public GameObject lowerLimit;

    protected float m_localTriggerDistance;

    protected virtual void Start() {
      base.StateChangeHandler += onStateChanged;

      if ( m_dataBinder != null ) {
        SetPositionFromFraction(m_dataBinder.GetCurrentData());
      }
    }

    // Stop listening to any previous data binder and start listening to the new one.
    public void RegisterDataBinder(LMWidgets.DataBinder<LMWidgets.SliderBase, float> dataBinder) {
      if (dataBinder == null) {
        return;
      }

      UnregisterDataBinder ();
      m_dataBinder = dataBinder as DataBinderSlider;
      SetPositionFromFraction(m_dataBinder.GetCurrentData());
    }

    // Stop listening to any previous data binder.
    public void UnregisterDataBinder() {
      if (m_dataBinder == null) {
        return;
      }
      m_dataBinder = null;
    }

    protected virtual void sliderPressed() {
      fireSliderStart(GetSliderFraction());
    }

    protected virtual void sliderReleased() {
      fireSliderEnd(GetSliderFraction());
      if ( m_dataBinder != null ) {
        SetPositionFromFraction(m_dataBinder.GetCurrentData()); // Pull latest external data
      }
    }

    protected virtual void fireSliderStart(float value) {
      EventHandler<LMWidgets.EventArg<float>> handler = StartHandler;
      if (handler != null) {
        handler (this, new LMWidgets.EventArg<float> (value));
      }
    }

    protected virtual void fireSliderChanged(float value) {
      EventHandler<LMWidgets.EventArg<float>> handler = ChangeHandler;
      if (handler != null) {
        handler (this, new LMWidgets.EventArg<float> (value));
      }
    }

    protected virtual void fireSliderEnd(float value) {
      EventHandler<LMWidgets.EventArg<float>> handler = EndHandler;
      if (handler != null) {
        handler (this, new LMWidgets.EventArg<float> (value));
      }
    }

    protected override void onInteractionEnabled ()
    {
      if (m_dataBinder != null) {
        SetPositionFromFraction (m_dataBinder.GetCurrentData ()); // Pull latest external data
      }
    }

    /// <summary>
    /// Returns the fraction of the slider between lower and upper limit. 0.0 = At Lower. 1.0 = At Upper
    /// </summary>
    /// <returns></returns>
    public float GetSliderFraction()
    {
      float lowerLimitValue = lowerLimit.transform.localPosition.x;
      float upperLimitValue = upperLimit.transform.localPosition.x;
      if (lowerLimitValue >= upperLimitValue)
        return 0.0f;
      else
        return (transform.localPosition.x - lowerLimitValue) / (upperLimitValue - lowerLimitValue);
    }

    /// <summary>
    /// Set the slider from the fraction of the slider between lower and upper limit. 0.0 = At Lower. 1.0 = At Upper
    /// </summary>
    /// <returns></returns>
    public virtual void SetPositionFromFraction (float fraction)
    {
      fraction = Mathf.Clamp01 (fraction);
      float diff = upperLimit.transform.localPosition.x - lowerLimit.transform.localPosition.x;
      float newOffset = lowerLimit.transform.localPosition.x + (fraction * diff);
      transform.localPosition = new Vector3 (newOffset, transform.localPosition.y, transform.localPosition.z);
    }

    public void SetWidgetValue(float value) {
      if ( State == LeapPhysicsState.Interacting || State == LeapPhysicsState.Disabled ) { return; } // Don't worry about state changes during interaction.
      SetPositionFromFraction (value);
    }

    /// <summary>
    /// Returns the fraction of how much the handle is pressed down. 0.0 = At Rest. 1.0 = At Triggered Distance
    /// </summary>
    /// <returns></returns>
    public float GetHandleFraction()
    {
      if (m_localTriggerDistance == 0.0f)
        return 0.0f;
      else
      {
        float fraction = transform.localPosition.z / m_localTriggerDistance;
        return Mathf.Clamp(fraction, 0.0f, 1.0f);
      }
    }

    /// <summary>
    /// Constrain the slider to y-axis and z-axis
    /// </summary>
    protected override void ApplyConstraints()
    {
      Vector3 localPosition = transform.localPosition;
      localPosition.x = Mathf.Clamp(localPosition.x, lowerLimit.transform.localPosition.x, upperLimit.transform.localPosition.x);
      localPosition.y = 0.0f;
      localPosition.z = Mathf.Max(localPosition.z, 0.0f);
      transform.localPosition = localPosition;
    }

    private void onStateChanged(object sender, EventArg<LeapPhysicsState> arg) {
      if ( arg.CurrentValue == LeapPhysicsState.Interacting ) {
        sliderPressed();
      }
      else if ( arg.CurrentValue == LeapPhysicsState.Reflecting ) {
        sliderReleased();
      }
    }

    /// <summary>
    /// Check if the slider is being pressed or not
    /// </summary>
    private void CheckTrigger()
    {
      if (State == LeapPhysicsState.Interacting) { 
        fireSliderChanged (GetSliderFraction ());
        if (m_dataBinder != null) {
          m_dataBinder.SetCurrentData (GetSliderFraction ());
        }
      }
    }

    protected override void FixedUpdate()  {
      base.FixedUpdate ();
      CheckTrigger();
    }
  }
}

