﻿using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	private GameObject player;

	public int maxHealth;
	public int curHealth;

	public Texture2D bgImage; 
	public Texture2D fgImage; 

	public float healthBarLength;
	public float x = 0f;
	public float y = 0f;
	public float height = 32f;

	// Use this for initialization
	void Start () {   
		healthBarLength = Screen.width /2;    

		maxHealth = 100;
		curHealth = 100;

		OnGUI ();
	}

	// Update is called once per frame
	void Update () {
		AdjustCurrentHealth(0);
		curHealth = player.curHealth;
		OnGUI ();
	}

	void OnGUI () {
		// Create one Group to contain both images
		// Adjust the first 2 coordinates to place it somewhere else on-screen
		GUI.BeginGroup (new Rect (x,y, healthBarLength,height));

		// Draw the background image
		GUI.Box (new Rect (x,y, healthBarLength,height), bgImage);

		// Create a second Group which will be clipped
		// We want to clip the image and not scale it, which is why we need the second Group
		GUI.BeginGroup (new Rect (x,y, curHealth / maxHealth * healthBarLength, height));

		// Draw the foreground image
		GUI.Box (new Rect (x,y,healthBarLength,height), fgImage);

		// End both Groups
		GUI.EndGroup ();

		GUI.EndGroup ();
	}

	public void AdjustCurrentHealth(int adj){

		curHealth += adj;

		if(curHealth <0)
			curHealth = 0;

		if(curHealth > maxHealth)
			curHealth = maxHealth;

		if(maxHealth <1)
			maxHealth = 1;

		healthBarLength =(Screen.width /2) * (curHealth / (float)maxHealth);
	}
}