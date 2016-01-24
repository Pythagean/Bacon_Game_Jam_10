using UnityEngine;
using System.Collections;

public class DamageText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject,0.5f);	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, transform.position.y + 1f), 1 * Time.deltaTime);
	
	}

}
