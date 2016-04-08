using System;

namespace LMWidgets {
  // Interface to define the expected events provided by a widget with binary interactions (ex. a button).
  public interface BinaryInteractionHandler<T>  {
    // Fires when interaction with the widget starts.
    event EventHandler<LMWidgets.EventArg<T>> StartHandler;
    
    //Fires when interaction with the widget ends.
    event EventHandler<LMWidgets.EventArg<T>> EndHandler;
  }
}
