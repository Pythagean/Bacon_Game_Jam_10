using UnityEngine;
using System.Collections;

public class Poison : MonoBehaviour {

	public GameObject enemy;

	// Use this for initialization
	void Start () {
		//Destroy (this.gameObject,2f);	
	}
	
	// Update is called once per frame
	void Update () {
		//this.transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, transform.position.y + 1f), 1 * Time.deltaTime);
		this.transform.position = enemy.transform.position;
		//Destroy (this,0.5f);	
	}
//	public void test(){
//		
//	}
//
//	public void Initialize(GameObject enemyObject)
//	{
//		enemy = enemyObject;
//	}
}
