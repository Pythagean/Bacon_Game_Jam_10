using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float smoothing = 5f;

	Vector3 offset; 

	void Start()
	{
		offset = transform.position - target.position;
	}

	void FixedUpdate ()
	{
		Vector3 targetCamPos = new Vector3(target.position.x + offset.x, transform.position.y, target.position.z + offset.z);
		transform.position = Vector3.Lerp (transform.position, new Vector3 (targetCamPos.x , 0.0f , targetCamPos.z) , smoothing * Time.deltaTime);
	}
}
