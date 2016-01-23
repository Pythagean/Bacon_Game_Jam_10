using UnityEngine;
using System.Collections;

public class ExitLevel : MonoBehaviour {

	public float delay = 1f;
	public string levelToLoad;
	public LayerMask playerMask = 0;

	private SpriteRenderer spriteRenderer;
	public Sprite closed;
	public Sprite half;
	public Sprite open;

	private RaycastHit2D raycastBottomMiddle;
	private Vector2 bottomMiddle;

	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer> ();
		spriteRenderer.sprite = closed;

	}

	void Update(){
		bottomMiddle = new Vector2 (transform.position.x, transform.position.y - 0.45f);
		raycastBottomMiddle = Physics2D.Raycast (bottomMiddle, Vector2.up, 0.05f, playerMask);
		Debug.DrawRay (bottomMiddle, Vector2.up, Color.blue, 0.05f);

		if (raycastBottomMiddle) {
			spriteRenderer.sprite = half;
			if (Input.GetKey (KeyCode.E)) {
				spriteRenderer.sprite = open;
				LoadLevel ();
			}
		}

		if (!raycastBottomMiddle)
			spriteRenderer.sprite = closed;
	}

	public void LoadLevel(){
		Application.LoadLevel (levelToLoad);
	}

}
