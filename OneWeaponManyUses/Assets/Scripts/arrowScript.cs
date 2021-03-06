﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class arrowScript : MonoBehaviour {
	
	//public float arrowSpeed = 5f;
	//public float arrowLifetime = 4.0f;
	//public Vector3 arrowDirection = new Vector3(0,0);

	public float arrowCollision = 0.1f;
	public LayerMask platformMask = 0;
	public LayerMask enemyMask = 0;
	public LayerMask campfireMask = 0;

	private Animator _animator;
	private bool damageDisabled = false;
	
	public string arrowType = "normal";
	private LineRenderer line;
	private GameObject player;
	private bool hasHitEnemy;
	private bool hasHitCampfire;

	// Line start width
	private float grappleStartWidth = 0.1f;
	private float grappleEndWidth = 0.1f;

	//Rigidbody2D arrowRigidBody = getComponent<Rigidbody2D>();
	public Vector3 velocity;
	private RaycastHit2D _raycast;
	private RaycastHit2D _raycastEnemy;
	private RaycastHit2D _raycastCampfire;


	private GameObject enemyHitGameObject;
	private Vector3 enemyHitPosition;

	private GameObject fireHitGameObject;
	private Vector3 fireHitPosition;

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

		if (arrowType == "fire")		
		{
			//Debug.Log ("Fired Fire arrow");
			_animator.Play( Animator.StringToHash( "arrow_fire" ) );	
		}

		//Detect Collisions
		velocity = GetComponent<Rigidbody2D>().velocity;
		_raycast = Physics2D.Raycast(transform.position, velocity, arrowCollision, platformMask);

		_raycastEnemy = Physics2D.Raycast(transform.position,velocity, arrowCollision, enemyMask);
		_raycastCampfire = Physics2D.Raycast(transform.position,velocity, arrowCollision, campfireMask);


		//Debug.DrawRay (transform.position, velocity / 20, Color.red, 2);

		//If hit campfire detected
		if (_raycastCampfire && !hasHitCampfire && arrowType == "fire")
		{
			//Stops arrow and sets gravity to 0
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			GetComponent<Rigidbody2D>().gravityScale = 0;

			fireHitGameObject = _raycastCampfire.transform.gameObject;
			//enemyHitPosition = enemyHitGameObject.transform.position;
			hasHitCampfire = true;

			var campfireCampfire = fireHitGameObject.GetComponent<Campfire> ();
			campfireCampfire.onFire = true;

			//Destroy (this.gameObject,2f);
		}

		//If hit enemy detected
		if (_raycastEnemy && !hasHitEnemy) 
		{
			if (!damageDisabled)
			{
				//Stops arrow and sets gravity to 0
				GetComponent<Rigidbody2D>().velocity = Vector3.zero;
				GetComponent<Rigidbody2D>().gravityScale = 0;

				enemyHitGameObject = _raycastEnemy.transform.gameObject;
				//enemyHitPosition = enemyHitGameObject.transform.position;
				hasHitEnemy = true;

				var enemyEnemyController = enemyHitGameObject.GetComponent<EnemyController> ();
				enemyEnemyController.HitByArrow (arrowType);
			}

		}

		if (enemyHitGameObject != null && hasHitEnemy)
		{
			_animator.Play( Animator.StringToHash( "arrow_hit" ) );	
			var currentPosition = GetComponent<Rigidbody2D> ().transform.position;

			//GetComponent<Rigidbody2D> ().transform.position = (currentPosition - enemyHitGameObject.transform.position).normalized * 0.1f + enemyHitGameObject.transform.position;

			if (GetComponent<Rigidbody2D> ().velocity.x < 0)
			{
				GetComponent<Rigidbody2D> ().transform.position = new Vector3 (enemyHitGameObject.transform.position.x-0.1f, enemyHitGameObject.transform.position.y - 0.1f);
			} else 
			{
				GetComponent<Rigidbody2D> ().transform.position = new Vector3 (enemyHitGameObject.transform.position.x+0.1f, enemyHitGameObject.transform.position.y - 0.1f);
			}

			Destroy (this.gameObject,2f);

		}




		//If collision detected
		if (_raycast) 
		{
			//Stops arrow and sets gravity to 0
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			GetComponent<Rigidbody2D>().gravityScale = 0;
			damageDisabled = true;

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

		_animator = GetComponent<Animator>();

		//Debug.Log ("Arrow created");
		line = this.gameObject.AddComponent<LineRenderer>();
		line.SetWidth(grappleStartWidth, grappleEndWidth);
		line.SetVertexCount(2);
		Color lineColor = new Color ();
		//line.material.color = ColorUtility.TryParseHtmlString ("#F00", out lineColor);
		//line.material.color = new Color32(redValue,blueValue,greenValue,alphaValue);
		line.material.color = Color.white;

		line.SetColors(Color.white, Color.white);
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