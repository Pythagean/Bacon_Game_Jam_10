using UnityEngine;
using System.Collections;
using Prime31;


public class PlayerController : MonoBehaviour
{
  private CharacterController2D _controller;
  private Animator _animator;
  private Vector3 _velocity;
  
  private bool movingLeft;
  private bool movingRight;
  private float waitBeforeNextMove;
  private float timeToSpendMoving;
  
  private bool wandering;
  
  void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
    
    waitBeforeNextMove = Random.value * 500;
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

    if( _controller.isGrounded )
      _velocity.y = 0;
    
    
    if (wandering)
    {
      if (waitBeforeNextMove == 0)
      {
        //All movement has been done, recalculate random variables
        if (timeToSpendMoving == 0)
        {
          //Decide which direction to move
          if(Random.value > 0.5)
          {
            movingLeft = true;
          } else
          {
            movingRight = true;
          }
          
          timeToSpendMoving = Random.value * 500;
          waitBeforeNextMove = Random.value * 500;
        } else
        {
          
          if (movingLeft)
          {
            normalizedHorizontalSpeed = -1;
            if( transform.localScale.x > 0f )
              transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

            if( _controller.isGrounded )
              _animator.Play( Animator.StringToHash( "Run" ) );
            
          } else if (movingRight)
          {
            normalizedHorizontalSpeed = 1;
            if( transform.localScale.x < 0f )
              transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );

            if( _controller.isGrounded )
              _animator.Play( Animator.StringToHash( "Run" ) );
          } else
          {
            normalizedHorizontalSpeed = 0;
            if( _controller.isGrounded )
          _animator.Play( Animator.StringToHash( "Idle" ) );
          }
          
          var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

        // apply gravity before moving
        _velocity.y += gravity * Time.deltaTime;
        
        _controller.move( _velocity * Time.deltaTime );
        
        _velocity = _controller.velocity;
          
          timeToSpendMoving--;
        }
      } else
      {
        waitBeforeNextMove--;
      }
    } else //Not wandering
    {
      
    }
    
    
    
    
    
    
  }
  
  
  
  
  
  
}












