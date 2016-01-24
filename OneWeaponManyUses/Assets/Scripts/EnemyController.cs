using UnityEngine;
using System.Collections;
using Prime31;


public class EnemyController : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 2f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	private AudioSource audioSource;
	public AudioClip hitAudio;

	private CharacterController2D _controller;
	private Animator _animator;
	private Vector3 _velocity;

	private int timeBetweenDamagingPlayer = 0;

	public GameObject FirePrefab;
	public GameObject ArrowPrefab;
	public GameObject PoisonPrefab;

	public float randomMultiplier = 150f;
	private bool enemyDead = false;
	private bool movingLeft;
	private bool movingRight;
	private float waitBeforeNextMove;
	private float timeToSpendMoving;

	public int enemyStrength = 25;

	public bool wandering = true;
	public bool agro = false;

	private GameObject player;

	private RaycastHit2D _raycast;
	private RaycastHit2D _raycastBlocking;
	private Vector2 _raycastOrigin;
	public LayerMask playerMask = 0;
	public LayerMask blockingMask = 0;
	public float agroZoneDistance = 2f;

	private float normalizedHorizontalSpeed = 0;

	public float enemyHealth = 50;
	private bool enemyOnFire = false;
	private bool enemyPoisoned = false;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

		waitBeforeNextMove = Random.value * randomMultiplier;
		wandering = true;

		audioSource = GetComponent<AudioSource> ();
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

	void Update()
	{

		_raycastOrigin = new Vector2(transform.position.x, transform.position.y);

		if ( _controller.isGrounded )
			_velocity.y = 0;

		if (wandering)
		{
			
			if (waitBeforeNextMove < 0)
			{

				//Raycast looking for player
				if(movingLeft)
				{
					_raycast = Physics2D.Raycast(_raycastOrigin, Vector2.left, agroZoneDistance, playerMask);
					Debug.DrawRay (_raycastOrigin, Vector2.left, Color.blue, agroZoneDistance);
				} else if (movingRight)
				{
					_raycast = Physics2D.Raycast(_raycastOrigin, Vector2.right, agroZoneDistance, playerMask);
					Debug.DrawRay (_raycastOrigin, Vector2.right, Color.blue, agroZoneDistance);
				}
				if (_raycast)
				{
					agro = true;
					wandering = false;
				}


				//All movement has been done, recalculate random variables
				if (timeToSpendMoving <= 0)
				{

					//Decide which direction to move
					float directionVariable = Random.value;
					if (movingLeft)
					{
						directionVariable += 0.3f;
					} else
					{
						directionVariable -= 0.3f;
					}

					timeToSpendMoving = Random.value * randomMultiplier;
					waitBeforeNextMove = Random.value * randomMultiplier;

					//Debug.Log("directionVariable: " + directionVariable.ToString());
					if (directionVariable < 0.5)
					{
						//Debug.Log("Waiting to move Left " + timeToSpendMoving.ToString() + " in " + waitBeforeNextMove.ToString());
						movingLeft = true;
						movingRight = false;
					} else
					{
						// Debug.Log("Waiting to move Right: " + timeToSpendMoving.ToString() + " in " + waitBeforeNextMove.ToString());
						movingRight = true;
						movingLeft = false;
					}


				} else
				{

					if (movingLeft)
					{
						normalizedHorizontalSpeed = -1;
						if( transform.localScale.x > 0f )
							transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

						if( _controller.isGrounded )
							_animator.Play( Animator.StringToHash( "Enemy_Run" ) );

					} else if (movingRight)
					{
						normalizedHorizontalSpeed = 1;
						if( transform.localScale.x < 0f )
							transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

						if( _controller.isGrounded )
							_animator.Play( Animator.StringToHash("Enemy_Run") );
					} else
					{
						normalizedHorizontalSpeed = 0;
						if( _controller.isGrounded )
						{
							//_animator.Play(Animator.StringToHash("Enemy_Idle"));
							Debug.Log("Enemy_Idle");
						}

					}




					timeToSpendMoving--;
				}
			} else
			{
				normalizedHorizontalSpeed = 0;
				_animator.Play(Animator.StringToHash("Enemy_Idle"));
				waitBeforeNextMove--;
			}

			var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
			_velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);


			// apply gravity before moving
			_velocity.y += gravity * Time.deltaTime;

			_controller.move(_velocity * Time.deltaTime);

			_velocity = _controller.velocity;

		} else if(agro)
		{
			_raycastOrigin = new Vector2(transform.position.x, transform.position.y);

			_animator.Play(Animator.StringToHash("Enemy_Attack"));

			player = GameObject.FindGameObjectWithTag("Player");
			var currentDistanceToPlayer = Vector2.Distance(transform.position,player.transform.position);
			//Debug.Log ("currentDist: " + currentDistanceToPlayer.ToString ());

			if (currentDistanceToPlayer > 2.5)
			{
				transform.localRotation = Quaternion.Euler (0, 0, 0);
				wandering = true;
				agro = false;
			}

			if (transform.position.x > player.transform.position.x) {
				transform.localRotation = Quaternion.Euler (0, 180, 0);
				_raycastBlocking = Physics2D.Raycast(_raycastOrigin, Vector2.left, 0.1f, blockingMask);
			} else 
			{
				transform.localRotation = Quaternion.Euler (0, 0, 0);
				_raycastBlocking = Physics2D.Raycast(_raycastOrigin, Vector2.right, 0.1f, blockingMask);
			}

			if (!_raycastBlocking)
			{
				var targetPoint = new Vector3 (player.transform.position.x, transform.position.y);
				transform.position = Vector3.MoveTowards(transform.position, targetPoint, 0.05f);
			}

			if (currentDistanceToPlayer < 1 && transform.position.y >= (player.transform.position.y - 0.1f))
			{
				//Debug.Log ("DAMAGE");
				if (timeBetweenDamagingPlayer < 0)
				{
					//Debug.Log ("DAMAGE");
					player.GetComponent<PlayerController> ().damagePlayer (enemyStrength);
					timeBetweenDamagingPlayer = 60;
				} else {
					//Debug.Log ("timeBetweenDamagingPlayer: " + timeBetweenDamagingPlayer.ToString ());
					timeBetweenDamagingPlayer -= 1;
				}
			}
		}


		//Debug.Log ("Health: " + enemyHealth.ToString ());
		if (enemyHealth <= 0 && enemyDead == false)
		{
			wandering = false;
			agro = false;
			_animator.Play(Animator.StringToHash("Enemy_Death"));

			var enemyRigidbody = GetComponent<Rigidbody2D> ();

			enemyRigidbody.transform.Rotate(0,0,90);
			enemyRigidbody.transform.position = new Vector3 (enemyRigidbody.transform.position.x, enemyRigidbody.transform.position.y - 0.3f);
			enemyDead = true;

			if (enemyOnFire)
			{
				Vector3 controllerPosition = new Vector3(_controller.transform.position.x,_controller.transform.position.y);
				GameObject fireInstance = (GameObject)Instantiate(FirePrefab, controllerPosition, Quaternion.identity);
			}


		}

		if (enemyPoisoned)
		{
			
		}
			

		if (enemyDead)
		{
			CancelInvoke ("poisonDamage");
			_animator.Play(Animator.StringToHash("Enemy_Death"));

		}

	}

	public void poisonDamage()
	{
		enemyHealth -= 5;
		displayDamageText (Color.magenta, "2");
	}


	public void HitByArrow(string arrowType)
	{

//		Vector3 controllerPosition = new Vector3(_controller.transform.position.x,_controller.transform.position.y);
//		GameObject arrowInstance = (GameObject)Instantiate(ArrowPrefab, controllerPosition, Quaternion.identity);
//		var arrowAnimator = arrowInstance.GetComponent<Animator> ();
//		arrowAnimator.Play ("arrow_hit");

		if (enemyHealth > 0)
		{
			Color textColor = Color.red;
			var damageToTake = 0;
			Debug.Log ("Hit by a " + arrowType);
			switch(arrowType)
			{
			case "normal":
				damageToTake = 25;
				break;
			case "fire":
				damageToTake = 50;
				enemyOnFire = true;
				break;
			case "grapple":
				damageToTake = 25;
				break;
			case "poison":
				damageToTake = 25;
				textColor = Color.magenta;
				createPoisonSprite ();
				enemyPoisoned = true;
				InvokeRepeating ("poisonDamage", 1, 1);
				break;
			}

			enemyHealth -= damageToTake;

			displayDamageText (textColor, damageToTake.ToString ());

			//Debug.Log ("Ebeny Health: " + enemyHealth.ToString ());
		}
		




	}

	public void createPoisonSprite()
	{
		Vector3 controllerPosition = new Vector3(_controller.transform.position.x,_controller.transform.position.y);
		GameObject poisonInstance = (GameObject)Instantiate(PoisonPrefab, controllerPosition, Quaternion.identity);
		poisonInstance.GetComponent<Poison> ().enemy = this.gameObject;
		//oisonInstance.Ini
		//poisonInstance.GetComponent<Poison>().enemy = this.gameObject;
	}

	void displayDamageText(Color color, string text)
	{
		audioSource.clip = hitAudio;
		audioSource.Play ();

		var damageIndicator = new GameObject();
		var textMesh = damageIndicator .AddComponent<TextMesh>();
		damageIndicator.AddComponent<DamageText> ();
		damageIndicator.GetComponent<DamageText> ().destroy = true;

		textMesh.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);

		textMesh.color = color;
		textMesh.text = text;

		var currentPosition = GetComponent<Rigidbody2D> ().transform.position;
		damageIndicator.transform.position = new Vector3(currentPosition.x-0.1f,currentPosition.y+0.75f);
	}





}












