using UnityEngine;
using System.Collections;

public class DamageText : MonoBehaviour {


	public bool destroy;
	// Use this for initialization
	void Start () {
		//if (destroy)
			Destroy (this.gameObject,0.5f);	
	}
	
	// Update is called once per frame
	void Update () {
		if (destroy)
			this.transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, transform.position.y + 1f), 1 * Time.deltaTime);
	
	}

}
