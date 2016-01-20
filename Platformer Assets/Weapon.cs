using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {
	
	//public float arrowSpeed = 5f;
	//public float arrowLifetime = 4.0f;
	//public Vector3 arrowDirection = new Vector3(0,0);

	public float collisionDistance = 0.5f;
	public LayerMask platformMask = 0;
	
	public string weaponType = "ranged";
  public bool stickToWall = true;
	private GameObject player;

	//Rigidbody2D arrowRigidBody = getComponent<Rigidbody2D>();
	public Vector3 velocity;
	private RaycastHit2D _raycast;
	//public 

	void Update () 
	{
		if (weaponType == "ranged" && stickToWall)
		{
      //Detect Collisions
      velocity = GetComponent<Rigidbody2D>().velocity;
      _raycast = Physics2D.Raycast(transform.position,velocity, collisionDistance, platformMask);
      Debug.DrawRay (transform.position, velocity / 20, Color.red, 2);

      //If collision detected
      if (_raycast) 
      {
        //Stops projectile and sets gravity to 0 (Sticking it to wall)
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().gravityScale = 0;
      }
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
		
	}

	public void Initialize()
	{

	}

	public void setDirection()
	{
		
	}


}