using UnityEngine;
using System.Collections;

public class Campfire : MonoBehaviour {

	private Animator _animator;
	public bool onFire = false;

	// Use this for initialization
	void Start () {
		_animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (onFire)
		{
			_animator.Play( Animator.StringToHash( "Campfire" ) );
		}
	}
}
