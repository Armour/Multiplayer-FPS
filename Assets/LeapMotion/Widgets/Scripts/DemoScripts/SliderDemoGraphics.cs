using UnityEngine;
using System.Collections;

public class SliderDemoGraphics : MonoBehaviour
{
  public void SetActive(bool status)
  {
    Renderer[] renderers = GetComponentsInChildren<Renderer>();
    foreach (Renderer renderer in renderers)
    {
      renderer.enabled = status;
    }
  }

  public void SetBloomGain(float gain)
  {
    Renderer[] renderers = GetComponentsInChildren<Renderer>();
    foreach (Renderer renderer in renderers)
    {
      renderer.material.SetFloat("_Gain", gain);
    }
  }

  public void SetColor(Color color)
  {
    Renderer[] renderers = GetComponentsInChildren<Renderer>();
    foreach (Renderer renderer in renderers)
    {
      renderer.material.color = color;
    }
  }
}
