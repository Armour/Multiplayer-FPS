using System;

namespace LMWidgets { 
  // Interface to define the expected events provided by a widget with analog, or continuous, interactions (ex. a slider).
  public interface AnalogInteractionHandler<T> : BinaryInteractionHandler<T> {
    //Fires while the widget is being interacted with.
    event EventHandler<LMWidgets.EventArg<T>> ChangeHandler;
  }
}