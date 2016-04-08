using UnityEngine;
using System.Collections;

namespace LMWidgets {
  public interface IDataBoundWidget <WidgetType, PayloadType> where WidgetType : IDataBoundWidget<WidgetType, PayloadType>{
    // Stop listening to any previous data binder and start listening to the new one.
    void RegisterDataBinder (DataBinder<WidgetType, PayloadType> dataBinder);
    
    // Stop listening to any previous data binder.
    void UnregisterDataBinder ();

    void SetWidgetValue(PayloadType value);
  }
}