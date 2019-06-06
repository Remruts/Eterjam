using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(SpriteRenderer))]
public class paddleScript : MonoBehaviour {

	int playerId;
	public float radius = 6.5f;
	public float maxSpeed = 7f;
	public float acceleration = 0.7f;

	[Range(0f, 1f)]
	public float drag = 0.13f;

	float speed = 0f;
	float angle = 0f;
	float x, y;

	void Awake() {
		speed = 0f;
		angle = 0f;

		x = radius;
		y = 0f;
	}

	void Start(){
		playerId = GetComponent<paddleSettingsScript>().playerId;
	}

	void FixedUpdate () {
		if (!managerScript.manager.isPaused ()) {
			angle += speed;

			if (angle < 0f)
				angle += 360f;
			if (angle >= 360f)
				angle -= 360f;

			speed *= (1 - drag);

			float angle2Rad = angle * Mathf.Deg2Rad;
			x = Mathf.Cos(angle2Rad) * radius;
			y = Mathf.Sin(angle2Rad) * radius;


			if (inputManager.inputman.CCW(playerId-1)){
				if (speed < maxSpeed)
					speed += acceleration;
			}

			if (inputManager.inputman.CW(playerId-1)){
				if (speed > -maxSpeed)
					speed -= acceleration;
			}

			if (inputManager.inputman.CCWSlow(playerId-1)){
				if (speed < maxSpeed)
					speed += acceleration/2f;
			}

			if (inputManager.inputman.CWSlow(playerId-1)){
				if (speed > -maxSpeed)
					speed -= acceleration/2f;
			}

			if (inputManager.inputman.CW(playerId-1) && inputManager.inputman.CCW(playerId-1)) {
				// TODO: Agregar shader a paleta, tal vez.
				// O sea, una forma de identificar que neutraliza
			}

			transform.position = new Vector2 (x, y);
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		}
	}

	public void setAngle(float a){
		angle = a;
	}

}
