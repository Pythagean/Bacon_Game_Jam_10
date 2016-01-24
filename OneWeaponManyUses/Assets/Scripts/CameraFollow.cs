using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float smoothing = 5f;
	public bool camera = true;

	Vector3 offset; 

	void Start()
	{
		offset = transform.position - target.position;
	}

	void LateUpdate ()
	{



		Vector3 targetCamPos = new Vector3(target.position.x + offset.x, transform.position.y, target.position.z + offset.z);


		if (camera)
		{
			if (targetCamPos.x > 0)
			{
				targetCamPos.x = 0;
			}

			transform.position = Vector3.Lerp (transform.position, new Vector3 (targetCamPos.x , -1.98f , targetCamPos.z) , smoothing * Time.deltaTime);
		} else
		{
			if (targetCamPos.x > 0)
			{
				targetCamPos.x = -1.9f;
			}

			transform.position = Vector3.Lerp (transform.position, new Vector3 (targetCamPos.x-1.9f , 0.4f , targetCamPos.z) , smoothing * Time.deltaTime);
		}

	}
}
