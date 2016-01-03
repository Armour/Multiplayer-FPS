using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleTester : MonoBehaviour {
	private enum SystemType {None, Explosions, Flares, Flames};
	private SystemType systemType;
	public Object[] particleSystems;
	public Object[] loadFlareSystems;
	public Object[] loadDirectionalSystems;
	private List <FlareSystem> flareSystems;
	private List <FlareSystem> directionalSystems;
	private bool expBool;
	private bool flrBool;
	private bool flmBool;
	private Vector2 scrollPosition;
	private SystemType savedSystemType;
	
	private class FlareSystem {
		public string name;
		public GameObject particleObject;
		public ParticleSystem[] particleSystems;
		public bool toggleFlag;
		public bool savedToggleFlag;
	}
	
	void Awake () {
		expBool = flrBool = false;
		systemType = SystemType.None;
		savedSystemType = SystemType.None;
		particleSystems = Resources.LoadAll("Explosions", typeof(GameObject));
		loadFlareSystems = Resources.LoadAll("Flares", typeof(GameObject));
		loadDirectionalSystems = Resources.LoadAll("Directional", typeof(GameObject));
		flareSystems = new List <FlareSystem>();
		directionalSystems = new List <FlareSystem>();
		foreach (Object obj in loadFlareSystems) {
			FlareSystem thisFlareSystem = new FlareSystem ();
			thisFlareSystem.name = obj.name;
			thisFlareSystem.particleObject = Instantiate (obj, Vector3.zero, Quaternion.identity) as GameObject;
			thisFlareSystem.particleSystems = thisFlareSystem.particleObject.GetComponentsInChildren<ParticleSystem>();
#if UNITY_3_4 || UNITY_3_5
			thisFlareSystem.particleObject.SetActiveRecursively (false);
#else
			thisFlareSystem.particleObject.SetActive (false);
#endif
			thisFlareSystem.toggleFlag = false;
			thisFlareSystem.particleObject.transform.parent = this.transform;
			flareSystems.Add (thisFlareSystem);
		}
		foreach (Object obj in loadDirectionalSystems) {
			FlareSystem thisFlareSystem = new FlareSystem ();
			thisFlareSystem.name = obj.name;
			thisFlareSystem.particleObject = Instantiate (obj, Vector3.zero, Quaternion.identity) as GameObject;
			thisFlareSystem.particleSystems = thisFlareSystem.particleObject.GetComponentsInChildren<ParticleSystem>();
#if UNITY_3_4 || UNITY_3_5
			thisFlareSystem.particleObject.SetActiveRecursively (false);
#else
			thisFlareSystem.particleObject.SetActive (false);
#endif
			thisFlareSystem.toggleFlag = false;
			thisFlareSystem.particleObject.transform.parent = this.transform;
			directionalSystems.Add (thisFlareSystem);
		}
	}
	
	void OnGUI () {
		GUILayout.BeginHorizontal ();
		if (GUILayout.Toggle (expBool, "Explosions")) {
			expBool = SetBool();
			systemType = SystemType.Explosions;
		}
		if (GUILayout.Toggle (flrBool, "Flares")) {
			flrBool = SetBool();
			systemType = SystemType.Flares;
		}
		if (GUILayout.Toggle (flmBool, "Flames")) {
			flmBool = SetBool();
			systemType = SystemType.Flames;
		}
		GUILayout.EndHorizontal ();
		GUILayout.Space (20);
		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.Width (250), GUILayout.Height (550));
		switch (systemType) {
			case SystemType.Explosions:
				foreach (GameObject ps in particleSystems) {
					if (GUILayout.Button (ps.name)) {
						GameObject go = Instantiate (ps, Vector3.zero, Quaternion.identity) as GameObject;
						Destroy (go, 10);
					}
				}
			break;
			case SystemType.Flares:
				foreach (FlareSystem fs in flareSystems) {
					fs.toggleFlag = GUILayout.Toggle (fs.toggleFlag, fs.name);
					if (fs.toggleFlag != fs.savedToggleFlag) {
#if UNITY_3_4 || UNITY_3_5
						fs.particleObject.SetActiveRecursively (fs.toggleFlag);
#else
						fs.particleObject.SetActive (fs.toggleFlag);
#endif
						if (fs.toggleFlag) {
							foreach (ParticleSystem pSys in fs.particleSystems) {
								pSys.Clear ();
								pSys.Play ();
							}
							fs.toggleFlag = SetFlareBool ();
						}
						fs.savedToggleFlag = fs.toggleFlag;
					}
				}
			break;
			case SystemType.Flames:
				foreach (FlareSystem fs in directionalSystems) {
					fs.toggleFlag = GUILayout.Toggle (fs.toggleFlag, fs.name);
					if (fs.toggleFlag != fs.savedToggleFlag) {
#if UNITY_3_4 || UNITY_3_5
						fs.particleObject.SetActiveRecursively (fs.toggleFlag);
#else
						fs.particleObject.SetActive (fs.toggleFlag);
#endif
						if (fs.toggleFlag) {
							foreach (ParticleSystem pSys in fs.particleSystems) {
								pSys.Clear ();
								pSys.Play ();
							}
							fs.toggleFlag = SetDirBool ();
						}
						fs.savedToggleFlag = fs.toggleFlag;
					}
				}
			break;
		}
	    GUILayout.EndScrollView ();
	}
	
	void Update () {
		if (systemType != savedSystemType) {
			SetNoFlares ();
			savedSystemType = systemType;
		}
	}
	
	bool SetBool () {
		expBool = flrBool = flmBool = false;
		return true;
	}
	
	bool SetFlareBool () {
		foreach (FlareSystem fs in flareSystems) {
			fs.toggleFlag = false;
		}
		return true;
	}
	
	bool SetDirBool () {
		foreach (FlareSystem fs in directionalSystems) {
			fs.toggleFlag = false;
		}
		return true;
	}
	
	void SetNoFlares () {
		foreach (FlareSystem fs in flareSystems) {
			fs.toggleFlag = false;
			fs.savedToggleFlag = false;
#if UNITY_3_4 || UNITY_3_5
			fs.particleObject.SetActiveRecursively (false);
#else
			fs.particleObject.SetActive (false);
#endif
		}
		foreach (FlareSystem fs in directionalSystems) {
			fs.toggleFlag = false;
			fs.savedToggleFlag = false;
#if UNITY_3_4 || UNITY_3_5
			fs.particleObject.SetActiveRecursively (false);
#else
			fs.particleObject.SetActive (false);
#endif
		}
	}
		
}
