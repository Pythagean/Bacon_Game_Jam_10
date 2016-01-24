using UnityEngine;
using System.Collections;

public class DeathScreen : MonoBehaviour {

	//public string lastLevel;
	private int disabledScreen = 120;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {



		if (Input.anyKeyDown && disabledScreen <= 0)
		{
			Application.LoadLevel (ApplicationModel.currentLevel);
			ApplicationModel.currentPlayerHealth = 100f;
		}
		else
		{
			disabledScreen -= 1;
		}
	
	}
}
