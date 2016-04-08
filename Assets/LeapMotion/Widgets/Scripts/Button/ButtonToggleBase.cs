using UnityEngine;
using System.Collections;

namespace LMWidgets
{
  public abstract class ButtonToggleBase : ButtonBase, BinaryInteractionHandler<bool>, IDataBoundWidget<ButtonToggleBase, bool> {
    protected DataBinderToggle m_dataBinder;

    protected bool m_toggleState = true;

    public abstract void ButtonTurnsOn();
    public abstract void ButtonTurnsOff();

    /// <summary>
    /// Gets or sets the current state of the toggle button.
    /// </summary>
    public bool ToggleState {
      get { return m_toggleState; }
      set {
        if ( m_toggleState == value ) { return; }
        setButtonState(value);
        if ( m_dataBinder != null ) { m_dataBinder.SetCurrentData(m_toggleState); } // Update externally linked data
      }
    }

    protected override void Start() {
      if ( m_dataBinder != null ) {
        setButtonState(m_dataBinder.GetCurrentData(), true); // Initilize widget value
      }
      else {
        setButtonState(false, true);
      }
    }

    public void SetWidgetValue(bool value) {
      if ( State == LeapPhysicsState.Interacting ) { return; } // Don't worry about state changes during interaction.
      setButtonState (value);
    }

    // Stop listening to any previous data binder and start listening to the new one.
    public void RegisterDataBinder(LMWidgets.DataBinder<LMWidgets.ButtonToggleBase, bool> dataBinder) {
      if (dataBinder == null) {
        return;
      }
      
      UnregisterDataBinder ();
      m_dataBinder = dataBinder as DataBinderToggle;
      setButtonState(m_dataBinder.GetCurrentData());
    }
    
    // Stop listening to any previous data binder.
    public void UnregisterDataBinder() {
      m_dataBinder = null;
    }

    private void setButtonState(bool toggleState, bool force = false) {
      if ( toggleState == m_toggleState && !force ) { return; } // Don't do anything if there's no change
      m_toggleState = toggleState;
      if (m_toggleState == true)
        ButtonTurnsOn();
      else
        ButtonTurnsOff();
    }

    protected override void buttonReleased()
    {
      base.FireButtonEnd(m_toggleState);
      if ( m_dataBinder != null ) {
        setButtonState(m_dataBinder.GetCurrentData()); // Update once we're done interacting
      }
    }

    protected override void buttonPressed()
    {
      if (m_toggleState == false)
        ButtonTurnsOn();
      else
        ButtonTurnsOff();
      ToggleState = !ToggleState;
      base.FireButtonStart(m_toggleState);
    }
  }
}
