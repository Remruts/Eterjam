using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Este es un ejemplo de cómo heredar de charaMovementScript para un personaje principal que puede interactuar con objetos en frente suyo en una máscara "interactableMask" para diferentes layers.
*/
public class juliaScript : charaMovementScript {

	// Update is called once per frame
	override protected void Update () {
		if (ManagerScript.man.isPaused){
			return;
		}

		startUpdate();
		if (canMove){
			controlChar();
			checkInteractions();
		}
		endUpdate();
	}

	public void checkInteractions(){
		Vector2 facingDir = getFacingDirection();

		RaycastHit2D hit = Physics2D.Raycast(rb.position, facingDir, 1.5f, interactableMask.value);
		Debug.DrawRay(rb.position, facingDir * 1.5f, Color.green);

		if (hit.collider != null){
			if (Input.GetButtonDown("Interact")){
				hit.collider.GetComponent<interactionScript>().interact();
			}
		}
	}

	/*
	public override void OnTriggerEnter2D(Collider2D col){
		base.OnTriggerEnter2D(col);
		if (col.CompareTag("transporter")){
			Debug.Log("Transitioning to =>");
		}
	}
	*/
}
