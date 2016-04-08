using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace LMWidgets
{
	public class DialGraphics : MonoBehaviour, AnalogInteractionHandler<int>, IDataBoundWidget<DialGraphics, string>
  {
    // Events to implement for AnalogInteraction Handler.
    public event EventHandler<EventArg<int>> ChangeHandler;
    public event EventHandler<EventArg<int>> StartHandler;
    public event EventHandler<EventArg<int>> EndHandler;

    protected DataBinderDial m_dataBinder;

		private string m_currentDialValue; 
		public string CurrentDialValue   
		{
			get 
			{
				return m_currentDialValue; 
			}
			set
			{
        int dialIndex = 0;

        try { 
          dialIndex = parseDialString(value); 
        }
        catch (ArgumentException e) { // Thrown if 'value' isn't a valid label;
          Debug.LogException(e);
          return;
        }

        m_currentDialValue = value;
        CurrentDialInt = dialIndex;
				DebugDisplayString = value;
			}
		}

    private int m_currentDialInt;
    public int CurrentDialInt
    {
      get 
      {
        return m_currentDialInt; 
      }
			set
			{
				if(m_currentDialInt != value){
					SetPhysicsStep(value);
				}
				m_currentDialInt = value;
				DebugDisplayInt = value;

        try { 
          m_currentDialValue = DialLabels[m_currentDialInt];
        }
        catch (System.ArgumentOutOfRangeException e ){
          Debug.LogException(e);
        }
			}
		}
		
		public string DebugDisplayString;
		public int DebugDisplayInt;
		
		public List<string> DialLabels;
    [SerializeField]
		private float DialRadius = .07f;
    [SerializeField]
		private float LabelAngleRangeStart = 0f;
    [SerializeField]
		private float LabelAngleRangeEnd = 360f;
    [SerializeField]
		private Transform LabelPrefab;
    [SerializeField]
		private Transform DialPhysicsOffset;
    /// <summary>
    /// How far forward should the dial protrude from the panel.
    /// </summary>
    public float DialProtrudenceDistance;
    [SerializeField]
		private Transform DialPhysics;
    [SerializeField]
		private DialModeBase m_dialModeBase;
    [SerializeField]
		private Transform DialCenter;
		private List<float> LabelAngles = new List<float>();
		public Dictionary<string, float> DialLabelAngles = new Dictionary<string, float>();

   		private bool m_isEngaged = false;
		
		public Color PickerColorInActive;
		public Color PickerColorActive;
		public Image PickerBoxImage;
		
		public HilightTextVolume hilightTextVolume;
		public Color TextColor;

    private int parseDialString (string valueString){
      int index = -1;
      
	  index = DialLabels.IndexOf( valueString);

      if (index == -1) {
        throw new System.ArgumentException("valueString \"" + valueString + "\" is not a valid label.");
      }

      return index;
	}

    // Wrapper on top of setting value for IDataBoundWidget implementation.
    public void SetWidgetValue(string value) {
      if ( m_isEngaged ) { return; } // Don't update if it's being interacted with.
      CurrentDialValue = value;
    }


    // Stop listening to any previous data binder and start listening to the new one.
    public void RegisterDataBinder(DataBinder<DialGraphics, string> dataBinder) {
      if (dataBinder == null) {
        return;
      }
      
      UnregisterDataBinder ();
      m_dataBinder = dataBinder as DataBinderDial;
      CurrentDialValue = m_dataBinder.GetCurrentData ();
    }
    
    // Stop listening to any previous data binder.
    public void UnregisterDataBinder() {
      m_dataBinder = null;
    }

    void Awake() {
      if (m_dialModeBase == null) {
        m_dialModeBase = DialPhysics.GetComponent<DialModeBase> ();
      }

      if (m_dialModeBase == null) {
        throw new System.NullReferenceException("Could not find DialModeBase on DialPhysics Object.");
      }
    }

    /// <summary>
    /// Move the physics and graphics components into the proper positions.
    /// </summary>
    private void setInitialPositions() {

      /// This is a bit hacky, but we're ignoring the positions of the dial elements 
      /// in the editor and assigning them programatically.
      /// 
      /// This is how I found it, but I'm tempted to take a 
      /// "respect the editor" viewpoint for initial positions
      /// when it comes to widgets in the future. - @Daniel

      DialCenter.localPosition = new Vector3(0f, 0f, DialRadius + DialProtrudenceDistance);
      DialPhysicsOffset.localPosition = new Vector3(-(DialRadius + DialProtrudenceDistance) * 10f, 0f, 0f);
      hilightTextVolume.transform.localPosition += new Vector3(0, 0, DialProtrudenceDistance);
    }
    		
		void Start () {
      setInitialPositions();
			
		    generateAndLayoutLabels ();

			if( m_dataBinder != null ) {
				//Set the Dial value based on a string
        CurrentDialValue = m_dataBinder.GetCurrentData();
				SetPhysicsStep(CurrentDialInt);
			}
		}

    private void generateAndLayoutLabels() {
      float currentLayoutXAngle = LabelAngleRangeStart;
      
      for( int i=1; i<=DialLabels.Count; i++ ) {
        Transform labelPrefab = Instantiate(LabelPrefab, DialCenter.transform.position, transform.rotation) as Transform;
        labelPrefab.Rotate(currentLayoutXAngle, 0f, 0f);
        LabelAngles.Add (-currentLayoutXAngle);     
        labelPrefab.parent = DialCenter;
        labelPrefab.localScale = new Vector3(1f, 1f, 1f);
        Text labelText = labelPrefab.GetComponentInChildren<Text>();
        labelText.text = DialLabels[i - 1];
        DialLabelAngles.Add(DialLabels[i - 1], -currentLayoutXAngle);
        labelText.transform.localPosition = new Vector3(0f, 0f, -DialRadius);
        currentLayoutXAngle = ((Mathf.Abs(LabelAngleRangeStart) + Mathf.Abs(LabelAngleRangeEnd))/(DialLabels.Count)) * -i;
      }

      LabelPrefab.gameObject.SetActive(false); // Turn off the original prefab that was copied.
    }
		
		void Update () {
      updateGraphicsFromPhysicsDial ();

			if(m_isEngaged == true){
				if(m_dataBinder != null){
          m_dataBinder.SetCurrentData(CurrentDialValue); //Set the Dial value based on an int
				}

				if(ChangeHandler != null){
					ChangeHandler(this, new EventArg<int>( CurrentDialInt));
				}
			}
		}

    private void updateGraphicsFromPhysicsDial() {
      Vector3 physicsRotation = new Vector3 (DialPhysics.localRotation.eulerAngles.y, 0f, 0f);
      DialCenter.localEulerAngles = physicsRotation;
		CurrentDialInt = m_dialModeBase.CurrentStep;
			
    }

		public void HilightDial () {
			m_isEngaged = true;
			
      if( StartHandler != null )  {	
				StartHandler(this, new EventArg<int>(CurrentDialInt));
			}

			PickerBoxImage.color = PickerColorActive;
		}
		
		public void UpdateDial (){
			CurrentDialInt = m_dialModeBase.CurrentStep;
			
			
      if(m_dataBinder != null){
				//Set the Dial value based on a string
				//make sure we are what the program thinks we are
        m_dataBinder.SetCurrentData(CurrentDialValue);
			}

			if(EndHandler != null){
				EndHandler(this, new EventArg<int>(CurrentDialInt));
      }

			m_isEngaged = false;
			PickerBoxImage.color = PickerColorInActive;
    }
		
		public void SetPhysicsStep(int newInt){
      if (m_dialModeBase == null) {
        m_dialModeBase = DialPhysics.GetComponent<DialModeBase>();
      }
			m_dialModeBase.CurrentStep = newInt;
			
		}
	}
}
