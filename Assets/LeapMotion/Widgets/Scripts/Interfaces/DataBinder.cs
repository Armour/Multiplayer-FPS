using UnityEngine;
using System;
using System.Collections.Generic;

namespace LMWidgets {

  // Interface to define an object that can be a data provider to a widget.
  public abstract class DataBinder<WidgetType, PayloadType> : MonoBehaviour where WidgetType : IDataBoundWidget<WidgetType, PayloadType> {
    [SerializeField]
    private List<WidgetType> m_widgets;

    private PayloadType m_lastDataValue;

    // Fires when the data is updated with the most recent data as the payload
    public event EventHandler<EventArg<PayloadType>> DataChangedHandler;

    /// <summary>
    /// Returns the current system value of the data.
    /// </summary>
    /// <remarks>
    /// In the default implementation of the data-binder this is called every frame (in Update) so it's best to keep
    /// this implementation light weight.
    /// </remarks>
    abstract public PayloadType GetCurrentData();

    /// <summary>
    /// Set the current system value of the data.
    /// </summary>
    abstract protected void setDataModel(PayloadType value);

    // Directly set the current value of the data-model and send out the relevant updates.
    public void SetCurrentData(PayloadType value) {
      setDataModel (value);
      updateLinkedWidgets ();
      fireDataChangedEvent (GetCurrentData ());
      m_lastDataValue = GetCurrentData ();
    }

    // Itterate through the linked widgets and update their values.
    private void updateLinkedWidgets() {
      foreach(WidgetType widget in m_widgets) {
        widget.SetWidgetValue(GetCurrentData());
      }
    }

    // Register all assigned widgets with the data-binder.
    virtual protected void Awake() {
      foreach (WidgetType widget in m_widgets) {
        widget.RegisterDataBinder(this);
      }
    }

    // Grab the inital value for GetCurrentData
    virtual protected void Start() {
      m_lastDataValue = GetCurrentData();
    }

    // Checks for change in data.
    // We need this in addition to SetCurrentData as the data we're linked to 
    // could be modified by an external source.
    void Update() {
      PayloadType currentData = GetCurrentData();
      if (!compare (m_lastDataValue, currentData)) {
        updateLinkedWidgets ();
        fireDataChangedEvent (currentData);
      }
      m_lastDataValue = currentData;
    }

    // Fire the data changed event. 
    // Wrapping this in a function allows child classes to call it and fire the event.
    protected void fireDataChangedEvent(PayloadType currentData) {
      EventHandler<EventArg<PayloadType>> handler = DataChangedHandler;
      if ( handler != null ) { handler(this, new EventArg<PayloadType>(currentData)); }
    }

    // Handles proper comparison of generic types.
    private bool compare(PayloadType x, PayloadType y)
    {
      return EqualityComparer<PayloadType>.Default.Equals(x, y);
    }
  }

  public abstract class DataBinderSlider : DataBinder<SliderBase, float> {};
  public abstract class DataBinderToggle : DataBinder<ButtonToggleBase, bool> {};
  public abstract class DataBinderDial : DataBinder<DialGraphics, string> {};
}