using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class arrowScript : MonoBehaviour {
	
	//public float arrowSpeed = 5f;
	//public float arrowLifetime = 4.0f;
	//public Vector3 arrowDirection = new Vector3(0,0);

	public float arrowCollision = 1/2;
	public LayerMask platformMask = 0;
	
	public string arrowType = "normal";
	private LineRenderer line;
	private GameObject player;

	// Line start width
	private float grappleStartWidth = 0.1f;
	private float grappleEndWidth = 0.1f;

	//Rigidbody2D arrowRigidBody = getComponent<Rigidbody2D>();
	public Vector3 velocity;
	private RaycastHit2D _raycast;
	//public 

	void Update () 
	{
		if (arrowType == "grapple")
		{
			player = GameObject.Find("Player");
			//Debug.Log(player.ToString());
			//set starting point of line to this object, in this case the arrow prefab
			line.SetPosition(0, transform.position);
			//set the ending point of the line to the player
			line.SetPosition(1, player.transform.position);
		}
		
		//Detect Collisions
		velocity = GetComponent<Rigidbody2D>().velocity;
		_raycast = Physics2D.Raycast(transform.position,velocity, arrowCollision, platformMask);
		Debug.DrawRay (transform.position, velocity / 20, Color.red, 2);

		//If collision detected
		if (_raycast) 
		{
			//Stops arrow and sets gravity to 0
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			GetComponent<Rigidbody2D>().gravityScale = 0;
		}
	}

	void Create()
	{

	}
	
	void OnBecameInvisible () {
		this.gameObject.SetActive(false);
	}

	void Awake ()
	{
		//Debug.Log ("Arrow created");
		line = this.gameObject.AddComponent<LineRenderer>();
		line.SetWidth(grappleStartWidth, grappleEndWidth);
		line.SetVertexCount(2);
		Color lineColor = new Color ();
		//line.material.color = ColorUtility.TryParseHtmlString ("#F00", out lineColor);
		//line.material.color = new Color32(redValue,blueValue,greenValue,alphaValue);
		line.material.color = Color.gray;

		line.SetColors(Color.black, Color.black);
		//we need to see the line... 
		//line.GetComponent.<Renderer>().enabled = true;
		line.GetComponent<Renderer>().enabled = true;
		
		
		
	}



	public void Initialize()
	{

	}

	public void setDirection()
	{
		
	}


}