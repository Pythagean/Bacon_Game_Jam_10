using UnityEngine;
using System.Collections;
//using Prime31;

public class PlayerHealth : MonoBehaviour {

	private PlayerController player;

	public float maxHealth;
	public float curHealth;

	public Texture2D bgImage; 
	public Texture2D fgImage; 
	public Texture2D heart;

	public float healthBarLength;
	public float x;
	public float y;
	public float height;

	// Use this for initialization
	void Start () {   
		x = (float) (heart.width + 10);
		y = 5f;
		healthBarLength = bgImage.width;    
		height = bgImage.height;
		
		player = GetComponent<PlayerController>();

		maxHealth = player.maxHealth;
		curHealth = player.curHealth;

	}

	// Update is called once per frame
	void Update () {
		AdjustCurrentHealth(player.healthRegen);
		curHealth = player.curHealth;
		
	}

	void OnGUI () {
		//Draws heart
		GUI.DrawTexture (new Rect (x - 5 - heart.width, y, heart.width, heart.height), heart);

		// Create one Group to contain both images
		//GUI.BeginGroup (new Rect (x,y, healthBarLength,height));

		// Draw the background image
		GUI.DrawTexture (new Rect (x,y, bgImage.width,height), bgImage);

		// Create a second Group which will be clipped
		// We want to clip the image and not scale it, which is why we need the second Group
		GUI.BeginGroup (new Rect (x,y, (curHealth / maxHealth) * bgImage.width, height));

		// Draw the foreground image
		GUI.DrawTexture (new Rect (0f,0f,bgImage.width,height), fgImage);

		// End both Groups
		//GUI.EndGroup ();

		GUI.EndGroup ();
	}

	public void AdjustCurrentHealth(float adj){

		curHealth += adj;

		if(curHealth <0)
			curHealth = 0;

		if(curHealth > maxHealth - 1)
			curHealth = maxHealth;

		if(maxHealth <1)
			maxHealth = 1;

		healthBarLength =(bgImage.width) * (curHealth / maxHealth);
	}
}
