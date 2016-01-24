using UnityEngine;
using System.Collections;

public class DeathScreen : MonoBehaviour {

	//public string lastLevel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.anyKeyDown)
		{
			Application.LoadLevel (ApplicationModel.currentLevel);
		}
	
	}
}
