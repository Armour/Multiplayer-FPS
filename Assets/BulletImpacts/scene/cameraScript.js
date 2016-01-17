#pragma strict


static var startCamPosX = 0.025;
static var startCamPosZ = 0.0;
var myFX;
var myFX_Brick;
var myFX_Concrete;
var myFX_Flesh;
var myFX_Glass;
var myFX_Metal;
var myFX_Water;
var myFX_Wood;
var ready = true;
var fxTotal = 8;
var fxNum = 0;
var fxToLoad;
var fxString;
var myText = "";



function Start () {
	//transform.Translate((Vector3(0.0, 0.0, startCamPosZ)), Space.World);
	transform.position = Vector3(0.0, 6.0, 0.0);
	myText = "Dirt";
	myFX = GameObject.Instantiate (Resources.Load("effects/impactDirt"), Vector3(0.0, 0.0, 0.0), Quaternion.Euler(-90.0, 0.0, 0.0));
	myFX_Metal = GameObject.Instantiate (Resources.Load("effects/impactMetal"), Vector3(0.0, 0.0, 0.0), Quaternion.Euler(-90.0, 0.0, 0.0));
	myFX_Water = GameObject.Instantiate (Resources.Load("effects/impactWater"), Vector3(0.0, 0.0, 0.0), Quaternion.Euler(-90.0, 0.0, 0.0));
	myFX_Flesh = GameObject.Instantiate (Resources.Load("effects/impactFlesh"), Vector3(0.0, 0.0, 0.0), Quaternion.Euler(-90.0, 0.0, 0.0));
	myFX_Wood = GameObject.Instantiate (Resources.Load("effects/impactWood"), Vector3(0.0, 0.0, 0.0), Quaternion.Euler(-90.0, 0.0, 0.0));
	myFX_Brick = GameObject.Instantiate (Resources.Load("effects/impactBrick"), Vector3(0.0, 0.0, 0.0), Quaternion.Euler(-90.0, 0.0, 0.0));
	myFX_Concrete = GameObject.Instantiate (Resources.Load("effects/impactConcrete"), Vector3(0.0, 0.0, 0.0), Quaternion.Euler(-90.0, 0.0, 0.0));
	myFX_Glass = GameObject.Instantiate (Resources.Load("effects/impactGlass"), Vector3(0.0, 0.0, 0.0), Quaternion.Euler(-90.0, 0.0, 0.0));
	fxNum = 0;

}

function MakeTest() {
	if (fxNum == 0) {
		fxString = ("impactDirt(Clone)");
		myText = "Dirt";
	}
	else if (fxNum == 1) {
		fxString = ("impactMetal(Clone)");
		myText = "Metal";
	}
	else if (fxNum == 2) {
		fxString = ("impactWater(Clone)");
		myText = "Water";
	}
	else if (fxNum == 3) {
		fxString = ("impactFlesh(Clone)");
		myText = "Flesh";
	}
	else if (fxNum == 4) {
		fxString = ("impactWood(Clone)");
		myText = "Wood";
	}
	else if (fxNum == 5) {
		fxString = ("impactBrick(Clone)");
		myText = "Brick";
	}
	else if (fxNum == 6) {
		fxString = ("impactConcrete(Clone)");
		myText = "Concrete";
	}
	else if (fxNum == 7) {
		fxString = ("impactGlass(Clone)");
		myText = "Glass";
	}
	else if (fxNum >= fxTotal) {
		fxNum = 0;
	}
	print (fxNum);
	var curFX : GameObject;
	curFX = GameObject.Find(fxString);
	ready=false;
	curFX.GetComponent.<ParticleSystem>().Stop();
	yield WaitForSeconds(0.5);
	curFX.GetComponent.<ParticleSystem>().Play();
	yield WaitForSeconds(0.5);
	ready=true;
}


function Update () {
	var newTime = (0.33);
	var moveXMath = ((Mathf.Sin(Time.time * newTime)) * 2.75);
	var moveZMath = ((Mathf.Cos(Time.time * newTime)) * 2.75);
	var rotYMath = (((Time.time * 57.5) * newTime) + 180.0);
	//print (moveZMath);
	transform.position = ((Vector3(moveXMath, 1.75, moveZMath)));
	transform.rotation = (Quaternion.Euler(35.0, rotYMath, 0.0));
	
	//var myFX = GameObject.Find("effects/impactDust");
	var curFX : GameObject;
	curFX = GameObject.Find(fxString);
	
	if (ready) {
		StartCoroutine(MakeTest());
	}


}

function OnGUI () {
	var viewNextBoxOffset = Vector2(10, 50);
	GUI.Box (Rect (viewNextBoxOffset[0],viewNextBoxOffset[1],120,90), "Effect Options");
	
	GUI.Label (Rect(25, 110, 200, 40),(myText));
	
	var viewOptionsBoxOffset = Vector2(10, 160);
	GUI.Box (Rect (viewOptionsBoxOffset[0],viewOptionsBoxOffset[1],120,90), "View Options");

	if (GUI.Button (Rect ((viewNextBoxOffset[0] + 10),(viewNextBoxOffset[1] + 30),100,20), "Toggle Next")) {
		fxNum = (fxNum + 1);
	}

	if (GUI.Button (Rect ((viewOptionsBoxOffset[0] + 10),(viewOptionsBoxOffset[1] + 30),100,20), "Toggle Floor")) {
		var iPlane : GameObject;
		iPlane = GameObject.Find("impactPlane");
		iPlane.GetComponent.<Renderer>().enabled = !iPlane.GetComponent.<Renderer>().enabled;
	}

	if (GUI.Button (Rect ((viewOptionsBoxOffset[0] + 10),(viewOptionsBoxOffset[1] + 60),100,20), "Toggle Skybox")) {
		var cam : GameObject;
		cam = GameObject.Find("Main Camera");
		if (cam.GetComponent.<Camera>().clearFlags == 2) {
			cam.GetComponent.<Camera>().clearFlags = 1;
		}
		else if (cam.GetComponent.<Camera>().clearFlags == 1) {
			cam.GetComponent.<Camera>().clearFlags = 2;
			cam.GetComponent.<Camera>().backgroundColor = Vector4(0.5, 0.5, 0.5, 1.0);
		}
	}
}