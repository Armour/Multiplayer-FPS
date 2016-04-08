using UnityEngine;
using System.Collections;
using LMWidgets;

public class SliderActivationManager : DataBinderToggle {
  [SerializeField]
  SliderBase slider;

  override public bool GetCurrentData() {
    return slider.Interactable;
  }

  override protected void setDataModel(bool value) {
    slider.Interactable = value;
  }
}
