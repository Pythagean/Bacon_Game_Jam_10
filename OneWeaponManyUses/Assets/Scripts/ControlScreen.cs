using UnityEngine;
using System.Collections;

public class ControlScreen : MonoBehaviour {

	//public string lastLevel;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		if (Input.anyKeyDown)
		{
			Application.LoadLevel ("1Level");
		}

	}
}
