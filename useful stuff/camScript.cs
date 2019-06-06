using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Script para una cámara que sigue un target con un cierto offset. Se tiene que agregar a la cámara principal.
Agrega funcionalidad de screen shake, que se puede llamar desde cualquier otro script como:

	camScript.screen.shake(tiempo, intensidad);

*/
public class camScript : MonoBehaviour {

	bool shaking = false;
	float intensity = 0f;
	float shakeIncrement = 1f;

	Vector3 currentPos;
	Vector3 shakenPos;

	//Camera myCam;

	public static camScript screen;
	public Transform target;
	public Vector3 offset;

	void Start () {
		if (screen == null) {
			screen = this;
		} else {
			Destroy (gameObject);
		}

		//myCam = GetComponent<Camera> ();
		transform.position = target.position;
		currentPos = transform.position;
	}

	void LateUpdate () {
		if (shaking) {
			intensity += shakeIncrement * Time.deltaTime;
		}

		if (!shaking && intensity > 0) {
			intensity -= shakeIncrement * Time.deltaTime;

			if (intensity < 0) {
				intensity = 0;
			}
		}

		currentPos += (target.position - currentPos) * Time.deltaTime * 10f;

		currentPos.z = -10f;

		if (intensity > 0){
			shakenPos.x = currentPos.x + getShake();
			shakenPos.y = currentPos.y + getShake();
			shakenPos.z = currentPos.z;

			transform.position = shakenPos + offset;
		} else {
			transform.position = currentPos + offset;
		}

	}

	public void jumpTo(Vector3 pos){
		currentPos = pos;
		currentPos.z = -10f;
		transform.position = currentPos + offset;
	}

	float getShake(){
		int shakingNumber = (int)Random.Range(-intensity/2f, intensity/2f);
		shakingNumber += (int)(Mathf.Sign(shakingNumber) * intensity/2f);

		return shakingNumber;
	}

	public void shake(float time, float factor){
		shaking = true;

		if (time > 0){
			Invoke ("stopShaking", time);
		}

		shakeIncrement = factor;
	}

	public void stopShaking(){
		shaking = false;
	}
}
