using UnityEngine;
using System.Collections;
using Prime31;


public class EnemyController : MonoBehaviour
{
    // movement config
    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;

    private CharacterController2D _controller;
  private Animator _animator;
  private Vector3 _velocity;

    public float randomMultiplier = 150f;
  
  private bool movingLeft;
  private bool movingRight;
  private float waitBeforeNextMove;
  private float timeToSpendMoving;
  
  private bool wandering;

    private float normalizedHorizontalSpeed = 0;

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
       // Debug.Log("wandering: " + wandering.ToString());
        //Debug.Log("waitBeforeNextMove: " + waitBeforeNextMove.ToString());
       // Debug.Log("timeToSpendMoving: " + timeToSpendMoving.ToString());
        //Debug.Log("movingLeft: " + movingLeft.ToString());
       // Debug.Log("movingRight: " + movingRight.ToString());

        if ( _controller.isGrounded )
      _velocity.y = 0;
    
    
    if (wandering)
    {
      if (waitBeforeNextMove < 0)
      {
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
            Debug.Log("Waiting to move Left " + timeToSpendMoving.ToString() + " in " + waitBeforeNextMove.ToString());
            movingLeft = true;
            movingRight = false;
          } else
          {
            Debug.Log("Waiting to move Right: " + timeToSpendMoving.ToString() + " in " + waitBeforeNextMove.ToString());
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

        } else //Not wandering
    {
      
    }
    
    
    
    
    
    
  }
  
  
  
  
  
  
}












