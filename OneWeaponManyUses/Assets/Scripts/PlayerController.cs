using UnityEngine;
using System.Collections;
using Prime31;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour
{
	//Control config
	public KeyCode Left = KeyCode.A;
	public KeyCode Right = KeyCode.D;
	public KeyCode Jump = KeyCode.W;
	public KeyCode Down = KeyCode.S;
	public KeyCode ShootArrow = KeyCode.Mouse0;
	public KeyCode ShootSpecialArrow = KeyCode.Mouse1;
	public KeyCode GrapplePull = KeyCode.Q;

	private bool disableHang = false;
	private bool pullingIntoGrapple = false;
	private bool freezeGravity = false;

	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
  
  	//Projectile config
	public GameObject projectile;
  	public float projectileSpeed = 150f;
	private Vector3 projectileTarget;
  
  
  	//Health and Stamina config
	//public GameObject health;
	public float maxHealth = 100;
  	//public float maxStamina = 100;
  
  	public float curHealth;
	//public float curStamina;

	public LayerMask platformMask = 0;
  
  	public float healthRegen = 1f;
	//public float staminaRegen = 1f;
  	//private float staminaUsageJump = 15f;

	public int numberOfGrapples = 5;
	public int numberOfArrows = 20;
	public int numberOfFire = 5;
	public int numberOfPoison = 5;
  
	List<GameObject> arrowList;
	public GameObject ArrowPrefab;

	private RaycastHit2D _raycastTopRight;
	private RaycastHit2D _raycastTopLeft;
	private RaycastHit2D _raycastBottomRight;
	private RaycastHit2D _raycastBottomLeft;
	private Vector2 _topRight;
	private Vector2 _topLeft;
	private Vector2 _bottomRight;
	private Vector2 _bottomLeft;

	private string arrowType = "grapple";
  

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

	//Grapple variables
	private RaycastHit2D _raycastHitGrapple;
	private bool grappling = false;
	private bool holdingLedge = false;
	public float grapplePullSpeed = 0.2f;

	public float height = 0.25f;
	public float width = 0.3f;

	public Font font;
 
	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
		//health = GetComponent<PlayerHealth>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
    
		curHealth = maxHealth;
    	//curStamina = maxStamina;
		arrowList = new List<GameObject>();

	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{

		_topRight = new Vector2(transform.position.x+width, transform.position.y+height);
		_topLeft = new Vector2(transform.position.x-width, transform.position.y+height);
		_bottomRight = new Vector2(transform.position.x+width, transform.position.y-height);
		_bottomLeft = new Vector2(transform.position.x-width, transform.position.y-height);

		if( _controller.isGrounded )
			_velocity.y = 0;
    
      	//Stamina Regenetration
		/* if(curStamina > maxStamina - 1)
			curStamina = maxStamina;
		else
			curStamina += staminaRegen * Time.deltaTime;*/

		//Health Regeneration
		if (curHealth > maxHealth - 1)
			curHealth = maxHealth;
		else
			curHealth += healthRegen * Time.deltaTime;


		if( Input.GetKey( Right ) && pullingIntoGrapple == false)
		{
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Player_Run" ) );
		}
		else if(Input.GetKey( Left ) && pullingIntoGrapple == false)
		{
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Player_Run" ) );
		}
		else
		{
			normalizedHorizontalSpeed = 0;
			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Player_Idle" ) );
		}

		//Pulls player towards the grapple
		if (Input.GetKey (GrapplePull)) 
		{
			pullingIntoGrapple = true;
		}
		if (pullingIntoGrapple)
		{
			var objects = GameObject.FindGameObjectsWithTag("Grapple");
			var objectCount = objects.Length;
			foreach (var obj in objects) {
				var currentDistanceToGrapple = Vector2.Distance(transform.position,obj.transform.position);
				//Debug.Log ("currentDist: " + currentDistanceToGrapple.ToString ());

				freezeGravity = true;
				if(currentDistanceToGrapple < 1)
				{

				} else
				{
					transform.position = Vector3.MoveTowards(transform.position,obj.transform.position,grapplePullSpeed);
				}
			}
		}

		if(Input.GetKeyDown(Down))
		{
			pullingIntoGrapple = false;
			freezeGravity = false;
		}

    
    	//Shooting Projectile
		if (Input.GetKeyDown (ShootArrow) || Input.GetKeyDown(ShootSpecialArrow)) {

			//If grapple button is clicked while already grappling
			if (Input.GetKeyDown (ShootSpecialArrow) && grappling == true && arrowType == "grapple")
			{
				//Remove current grapple
				var objects = GameObject.FindGameObjectsWithTag("Grapple");
				var objectCount = objects.Length;
				foreach (var obj in objects) {
					Destroy (obj);
				}
				pullingIntoGrapple = false;
				freezeGravity = false;
				grappling = false;
			}
			else if (grappling == false && numberOfArrows > 0)
			{
				//Get positions of mouse/player and draw ray
				Vector3 mousePositionVector = new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.main.nearClipPlane);
				var mousePositionWorldVector = Camera.main.ScreenToWorldPoint(mousePositionVector);
				Vector3 controllerPosition = new Vector3(_controller.transform.position.x,_controller.transform.position.y);
				Debug.DrawRay(controllerPosition,(mousePositionWorldVector-controllerPosition),Color.green,1);

				//Create Arrow Object
				GameObject arrowInstance = (GameObject)Instantiate(ArrowPrefab, controllerPosition, Quaternion.identity);
				numberOfArrows -= 1;

				//Shoot the grapple
				if (Input.GetKeyDown (ShootSpecialArrow) && numberOfGrapples > 0 && arrowType == "grapple")
				{

					arrowScript arrowScriptInstance = arrowInstance.GetComponent<arrowScript>();
					arrowScriptInstance.arrowType = "grapple";
					arrowScriptInstance.tag = "Grapple";
					grappling = true;
					numberOfGrapples -= 1;
				}

				if (Input.GetKeyDown (ShootSpecialArrow) && numberOfFire > 0 && arrowType == "fire")
				{
					arrowScript arrowScriptInstance = arrowInstance.GetComponent<arrowScript>();
					arrowScriptInstance.arrowType = "fire";
					arrowScriptInstance.tag = "Fire";
					numberOfFire -= 1;
				}

				if (Input.GetKeyDown (ShootSpecialArrow) && numberOfPoison > 0 && arrowType == "poison")
				{
					arrowScript arrowScriptInstance = arrowInstance.GetComponent<arrowScript>();
					arrowScriptInstance.arrowType = "poison";
					arrowScriptInstance.tag = "Poison";
					numberOfPoison -= 1;
				}


				Debug.Log ("adding force");
				//Add force to arrow object
				Rigidbody2D arrowRigidBody = arrowInstance.GetComponent<Rigidbody2D>();
				arrowRigidBody.AddForce((mousePositionWorldVector-controllerPosition) * projectileSpeed,ForceMode2D.Force);

				//Rotate arrow to face position of mouse click
				Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - arrowRigidBody.transform.position;
				diff.Normalize();
				float zRotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
				arrowRigidBody.transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
			} 


			if (numberOfArrows == 0)
			{
				Debug.Log("Out of Arrows!");
			}
			if (numberOfGrapples == 0)
			{
				Debug.Log("Out of Rope!");
			}


		}

		if (holdingLedge)
		{
			//Debug.Log ("Holding Ledge!");
			//_controller.isGrounded = true;
			_controller.velocity = Vector3.zero;
			if (Input.GetKeyDown (Jump))
			{

				_velocity.y = Mathf.Sqrt( 2f * (jumpHeight * 0.5f)* -gravity );



				_animator.Play( Animator.StringToHash( "Player_Jump" ) );
				//curStamina -= staminaUsageJump;
				disableHang = true;
				holdingLedge = false;
			}
			else if (Input.GetKeyDown (Down))
			{
				Debug.Log("Dropping down from Ledge");
				disableHang = true;
				holdingLedge = false;
			}
		}

		//Check for holding ledge
		if (holdingLedge == false)
		{
			if (disableHang == false)
			{
				_raycastTopLeft = Physics2D.Raycast(_topLeft, Vector2.left, 0.1f, platformMask);
				Debug.DrawRay (_topLeft, Vector2.left, Color.blue, 0.1f);


				_raycastBottomLeft = Physics2D.Raycast(_bottomLeft, Vector2.left, 0.1f, platformMask);
				Debug.DrawRay (_bottomLeft, Vector2.left, Color.blue, 0.1f);

				_raycastTopRight = Physics2D.Raycast(_topRight, Vector2.right, 0.1f, platformMask);
				Debug.DrawRay (_topRight, Vector2.right, Color.blue, 0.1f);

				_raycastBottomRight = Physics2D.Raycast(_bottomRight, Vector2.right, 0.1f, platformMask);
				Debug.DrawRay (_bottomRight, Vector2.right, Color.blue, 0.1f);

				if (((!_raycastTopRight && _raycastBottomRight) || (!_raycastTopLeft && _raycastBottomLeft)) && _controller.velocity.y <= 0)
				{
					//Debug.Log ("y: " + _velocity.y);
					//Debug.Log ("_controller.y: " + _controller.velocity.y);
					holdingLedge = true;
					_animator.Play( Animator.StringToHash( "Player_Hang" ) );
				}

			} else
			{
				if (_controller.isGrounded)
					disableHang = false;

			}
				


			// we can only jump whilst grounded
			if( _controller.isGrounded && Input.GetKeyDown( Jump ))// && curStamina > staminaUsageJump)
			{
				_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );

				_animator.Play( Animator.StringToHash( "Player_Jump" ) );
				//curStamina -= staminaUsageJump;
			}

			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				//Grapple
				arrowType = "grapple";
			} else if (Input.GetKeyDown(KeyCode.Alpha2) && grappling == false)
			{
				
				//Fire
				arrowType = "fire";
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3) && grappling == false)
			{

				//Fire
				arrowType = "poison";
			}

			



			// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
			var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
			_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

			if(freezeGravity == false)
				_velocity.y += gravity * Time.deltaTime;
			// apply gravity before moving
			

			// if holding down bump up our movement amount and turn off one way platform detection for a frame.
			// this lets uf jump down through one way platforms
			if( _controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
			{
				_velocity.y *= 3f;
				_controller.ignoreOneWayPlatformsThisFrame = true;
			}

			_controller.move( _velocity * Time.deltaTime );

			// grab our current _velocity to use as a base for all calculations
			_velocity = _controller.velocity;


		}
    



	}

	void OnGUI() {
		GUI.skin.font = font;
		GUI.Label (new Rect (5, 30, 200, 25), "Arrows:    " + numberOfArrows);
		GUI.Label (new Rect (5, 40, 200, 25), "Grapples:  " + numberOfGrapples);
		GUI.Label (new Rect (5, 50, 200, 25), "Fire:  " + numberOfFire);
		GUI.Label (new Rect (5, 60, 200, 25), "Poison:  " + numberOfPoison);
		GUI.Label (new Rect (5, 70, 200, 25), "Arrow Type:  " + arrowType);
	}
  


}
