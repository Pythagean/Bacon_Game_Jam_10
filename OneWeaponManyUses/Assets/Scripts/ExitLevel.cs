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

	private bool displayingPrompt = false;

	private RaycastHit2D raycastBottomMiddle;
	private Vector2 bottomMiddle;

	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer> ();
		//spriteRenderer.sprite = closed;

	}

	void Update(){
		bottomMiddle = new Vector2 (transform.position.x, transform.position.y - 0.45f);
		raycastBottomMiddle = Physics2D.Raycast (bottomMiddle, Vector2.up, 0.05f, playerMask);
		Debug.DrawRay (bottomMiddle, Vector2.up, Color.blue, 0.05f);

		if (raycastBottomMiddle) {

			//displayingPrompt = false;
			if (!displayingPrompt) 
			{
				displayText (Color.white, "Press 'E'");
			}

			//spriteRenderer.sprite = half;
			if (Input.GetKey (KeyCode.E)) {
				//spriteRenderer.sprite = open;
				LoadLevel ();
			}
		} else
		{
			displayingPrompt = false;
		}

		//if (!raycastBottomMiddle)
		//	spriteRenderer.sprite = closed;
	}

	public void LoadLevel(){
		Application.LoadLevel (levelToLoad);
	}

	void displayText(Color color, string text)
	{
		var damageIndicator = new GameObject();
		var textMesh = damageIndicator .AddComponent<TextMesh>();
		damageIndicator.AddComponent<DamageText> ();
		damageIndicator.GetComponent<DamageText> ().destroy = false;

		textMesh.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);

		textMesh.color = color;
		textMesh.text = text;

		var currentPosition = GetComponent<Rigidbody2D> ().transform.position;
		damageIndicator.transform.position = new Vector3(currentPosition.x-0.1f,currentPosition.y+0.75f);
		displayingPrompt = true;

		}



}
