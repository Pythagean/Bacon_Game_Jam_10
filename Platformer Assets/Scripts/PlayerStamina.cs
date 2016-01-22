using UnityEngine;
using System.Collections;

public class PlayerStamina : MonoBehaviour {

	private PlayerController player;

	public float maxStamina;
	public float curStamina;
	public float staminaRegen;

	public Texture2D bgImage; 
	public Texture2D fgImage; 
	public Texture2D stamina;

	public float staminaBarLength;
	public float x;
	public float y;
	public float height;

	// Use this for initialization
	void Start () {   
		x = (float) (stamina.width + 10);
		y = 30f;
		staminaBarLength = bgImage.width;    
		height = bgImage.height;

		player = GetComponent<PlayerController>();

		maxStamina = player.maxStamina;
		curStamina = player.curStamina;
		staminaRegen = player.staminaRegen;

		OnGUI ();
	}

	// Update is called once per frame
	void Update () {
		curStamina = player.curStamina;
		OnGUI ();
	}

	void OnGUI () {
		//Draws icon
		GUI.DrawTexture (new Rect (x - 5 - stamina.width, y, stamina.width, stamina.height), stamina);

		// Draw the background image
		GUI.DrawTexture (new Rect (x,y, bgImage.width,height), bgImage);

		// Create a Group which will be clipped
		// We want to clip the image and not scale it, which is why we need the second Group
		GUI.BeginGroup (new Rect (x,y, (curStamina / maxStamina) * bgImage.width, height));

		// Draw the foreground image
		GUI.DrawTexture (new Rect (0f,0f,bgImage.width,height), fgImage);

		GUI.EndGroup ();
	}

}
