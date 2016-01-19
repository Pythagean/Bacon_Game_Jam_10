using UnityEngine;
using System.Collections;


public class PlayerController: MonoBehaviour
{
  
  //Animation States
  private int idleState = Animator.StringToHash("Base Layer.Idle");
  private int runState = Animator.StringToHash("Base Layer.Run");
  private int jumpState = Animator.StringToHash("Base Layer.Jump");
  
  private CharacterController2D _controller;
  private Animator _animator;
  
   void Awake()
  {
    _controller = GetComponent
  }

  void Update()
  {
      var velocity = _controller.velocity;
      
      if(_controller.isGrounded)
        velocity.y = 0;
      
      if (Input.GetKey( KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
      {
        velocity.x = runSpeed;
        goRight();
        
        if(_controller.isGrounded)
          _animator.goToStateIfNotAlreadyThere(runState);
        
      }
      else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
      {
        velocity.x = -runSpeed;
        goLeft;
        
        if(_controller.isGrounded)
          _animator.goToStateIfNotAlreadyThere(runState);
        
      }
      else
      {
        velocity.x = 0;
        
        if(_controller.isGrounded)
          _animator.goToStateIfNotAlreadyThere(idleState);
      }
      
      //My code - Shooting towards click position
      if (Input.GetKeyDown (KeyCode.Mouse0)) {
       Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
       Vector3 dir = (Input.mousePosition - sp).normalized;
       rigidbody2D.AddForce (dir * amount);

      }
      
      if(Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
      {
        var targetJumpHeight = 2f;
        velocity.y = Mathf.Sqrt(2f * targetJumpHeight * -gravity);
        _animator.goToStateIfNotAlreadyThere(jumpState);
      }
      
      //apply gravity before moving
      velocity.y += gravity * Time.deltaTime;
      
      //move character
      _controller.move(velocity * Time.deltaTime);
  }

  private void goLeft()
  {
    if(transform.localScale.x > 0f)
      transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
  }

  private void goRight()
  {
    if(transform.localScale.x < 0f)
      transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
  }

}

