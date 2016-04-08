using UnityEngine;
using System.Collections;
using LMWidgets;

public class ButtonDemoToggle : ButtonToggleBase 
{
  public ButtonDemoGraphics onGraphics;
  public ButtonDemoGraphics offGraphics;
  public ButtonDemoGraphics midGraphics;
  public ButtonDemoGraphics botGraphics;
  
  public Color MidGraphicsOnColor = new Color(0.0f, 0.5f, 0.5f, 1.0f);
  public Color BotGraphicsOnColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);
  public Color MidGraphicsOffColor = new Color(0.0f, 0.5f, 0.5f, 0.1f);
  public Color BotGraphicsOffColor = new Color(0.0f, 0.25f, 0.25f, 1.0f);
	

  public override void ButtonTurnsOn()
  {
    TurnsOnGraphics();
  }

  public override void ButtonTurnsOff()
  {
    TurnsOffGraphics();
  }

  private void TurnsOnGraphics()
  {
    onGraphics.SetActive(true);
    offGraphics.SetActive(false);
	midGraphics.SetColor(MidGraphicsOnColor);
	botGraphics.SetColor(BotGraphicsOnColor);
  }

  private void TurnsOffGraphics()
  {
    onGraphics.SetActive(false);
    offGraphics.SetActive(true);
	midGraphics.SetColor(MidGraphicsOffColor);
	botGraphics.SetColor(BotGraphicsOffColor);
  }

  private void UpdateGraphics()
  {
    Vector3 position = transform.localPosition;
    position.z = Mathf.Min(position.z, m_localTriggerDistance);
    onGraphics.transform.localPosition = position;
    offGraphics.transform.localPosition = position;
    Vector3 bot_position = position;
    bot_position.z = Mathf.Max(bot_position.z, m_localTriggerDistance - m_localCushionThickness);
    botGraphics.transform.localPosition = bot_position;
    Vector3 mid_position = position;
    mid_position.z = (position.z + bot_position.z) / 2.0f;
    midGraphics.transform.localPosition = mid_position;
  }

  protected override void Start()
  {
    base.Start();
  }

  protected override void FixedUpdate()
  {
    base.FixedUpdate();
    UpdateGraphics();
  }
}
