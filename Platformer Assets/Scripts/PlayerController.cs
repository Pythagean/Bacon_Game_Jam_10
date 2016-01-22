using UnityEngine;
using System.Collections;
using Prime31;


public class PlayerController : MonoBehaviour
{
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
  
	//Weapon config
	public int currentWeapon;
	public Transform[] weapons;
  
  	//Health and Stamina config
	//public GameObject health;
	public float maxHealth = 100;
  	public float maxStamina = 100;
  
  	public float curHealth;
	public float curStamina;
  
  	public float healthRegen = 1f;
	public float staminaRegen = 1f;
  	private float staminaUsageJump = 15f;
  
  
  

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
 
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
    	curStamina = maxStamina;
    

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
		if( _controller.isGrounded )
			_velocity.y = 0;
    
      	//Stamina Regenetration
		 if(curStamina > maxStamina - 1)
			curStamina = maxStamina;
		else
			curStamina += staminaRegen * Time.deltaTime;

		//Health Regeneration
		if (curHealth > maxHealth - 1)
			curHealth = maxHealth;
		else
			curHealth += staminaRegen * Time.deltaTime;


		if( Input.GetKey( KeyCode.D ))
		{
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Run" ) );
		}
		else if(Input.GetKey( KeyCode.A ))
		{
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Run" ) );
		}
		else
		{
			normalizedHorizontalSpeed = 0;
			if( _controller.isGrounded )
				_animator.Play( Animator.StringToHash( "Idle" ) );
		}
    
    	//Shooting Projectile
		if (Input.GetKeyDown (KeyCode.Mouse0)) {

			Vector3 mousePositionVector = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
			Vector3 mousePositionWorldVector = Camera.main.ScreenToWorldPoint (mousePositionVector);
			Vector3 playerPosition = new Vector3 (_controller.transform.position.x, transform.position.y);
			Debug.DrawRay (playerPosition, (mousePositionWorldVector - playerPosition), Color.green, 1);

			GameObject projectileInstance = (GameObject)Instantiate (projectile, playerPosition, Quaternion.identity);
			Rigidbody2D projectileRigidbody = projectileInstance.GetComponent<Rigidbody2D> ();
			projectileRigidbody.AddForce ((mousePositionWorldVector - playerPosition) * projectileSpeed, ForceMode2D.Force);

			Vector3 diff = Camera.main.ScreenToWorldPoint (Input.mousePosition) - projectileRigidbody.transform.position;
			diff.Normalize ();
			float zRotation = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			projectileRigidbody.transform.rotation = Quaternion.Euler (0f, 0f, zRotation);

    		}
    
		if (Input.GetKeyDown(KeyCode.Keypad1))
    		{
			changeWeapon(1);
		 } 
		else if (Input.GetKeyDown(KeyCode.Keypad2))
    		{
      			changeWeapon(2);
    		} 
		else if (Input.GetKeyDown(KeyCode.Keypad3))
    		{
      			changeWeapon(3);
    		}


		// we can only jump whilst grounded
  		if( _controller.isGrounded && (Input.GetKeyDown( KeyCode.Space ) || Input.GetKeyDown( KeyCode.W )) && curStamina > staminaUsageJump)
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
			_animator.Play( Animator.StringToHash( "Jump" ) );
			curStamina -= staminaUsageJump;
		}
 

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

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
  
  	public void changeWeapon(int num) {
     		currentWeapon = num;
     		for(int i = 0; i < weapons.Length; i++) {
         		if(i == num)
             			weapons[i].gameObject.SetActive(true);
         		else
             			weapons[i].gameObject.SetActive(false);
     		}
 	}

}
