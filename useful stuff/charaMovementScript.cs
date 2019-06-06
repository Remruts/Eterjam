using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


/*
- Script para movimiento de personaje en cuatro direcciones + manejo de animaciones.
- Las colisiones las maneja Unity (en teoría) mientras el objeto tenga un Collider2D (Box, Circle, etc.), ya que usa el rigidbody, no el transform para moverse.
- Aunque necesita del ManagerScript, se pueden borrar esas líneas.
- Utiliza una forma especial de trabajar con el Animator y Blend Trees.
- NPCs pueden hacer su propio script que hereda de este y sobrecargar Update de la forma:

	override protected void Update(){
		startUpdate();
		followTarget();
		endUpdate();
	}

*/
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class charaMovementScript : MonoBehaviour {

	public float maxSpeed = 0.05f;
	public LayerMask interactableMask = -1;

	protected Animator anim;
	protected AnimatorStateInfo state;
	protected Rigidbody2D rb;
	protected SpriteRenderer spr;

	Vector2 speed = Vector2.zero;

	protected bool canMove = true;
	public float friction = 0.9f;

	Vector3 target;
	bool isMovingTowardsTarget = false;

	// Use this for initialization
	virtual protected void Start () {
		anim = GetComponent<Animator>();
		anim.Play("idle");
		anim.SetFloat("FaceX", 0);
		anim.SetFloat("FaceY", -1);

		rb = GetComponent<Rigidbody2D>();
		spr = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	virtual protected void Update () {
		if (ManagerScript.man.isPaused){
			return;
		}
		startUpdate();
		followTarget();
		if (canMove){
			controlChar();
		}
		endUpdate();
	}

	virtual public void startUpdate(){
		state = anim.GetCurrentAnimatorStateInfo(0);
	}

	public void controlChar(){
		if (ManagerScript.man.isInCutscene || ManagerScript.man.isInMenu){
			stopMoving();
			return;
		}

		if (Input.GetKey("up") && Input.GetKey("down")){
			stopMoving();
			if (state.IsName("walk")){
				anim.Play("idle");
			}
			return;
		}
		if (Input.GetKey("left") && Input.GetKey("right")){
			stopMoving();
			if (state.IsName("walk")){
				anim.Play("idle");
			}
			return;
		}

		if (Input.GetKey("right")){
			if (!Input.GetKey("left")){
				move(1, 0);
			}
		}
		if (Input.GetKey("left")){
			if (!Input.GetKey("right")){
				move(-1, 0);
			}
		}
		if (Input.GetKey("down")){
			if (!Input.GetKey("up")){
				move(0, -1);
			}
		}
		if (Input.GetKey("up")){
			if (!Input.GetKey("down")){
				move(0, 1);
			}
		}

		if (!Input.GetKey("up") && !Input.GetKey("down") && !Input.GetKey("left") && !Input.GetKey("right")){
			if (state.IsName("walk")){
				anim.Play("idle");
			}
		}

		if (Input.GetKeyDown("z")){
			//interact();
		}

		if (!Input.GetKey("left") && !Input.GetKey("right")){
			speed.x = 0;
		}

		if (!Input.GetKey("up") && !Input.GetKey("down")){
			speed.y = 0;
		}
	}

	protected void followTarget(){
		if (isMovingTowardsTarget){
			Vector3 toTarget = target - transform.position;

			if (toTarget.magnitude > 0.1f){
				Vector3 newDirection = (target - transform.position).normalized;
				move(newDirection.x, newDirection.y);
			} else {
				isMovingTowardsTarget = false;
				stopMoving();
			}
		}
	}

	public void moveTowards(Vector3 point){
		target = point;
		isMovingTowardsTarget = true;
	}

	public void interact(){
		if (state.IsName("idle") || state.IsName("walk")){
			//stuff
			stopMoving();
		}
	}

	public void stopMoving(){
		speed.x = 0;
		speed.y = 0;
	}

	public Vector2 getFacingDirection(){
		Vector2 res = Vector2.zero;

		res.x = anim.GetFloat("FaceX");
		res.y = anim.GetFloat("FaceY");

		return res;
	}

	virtual public void endUpdate(){
		if (state.normalizedTime >= 1){
			if (state.IsName("walk") && (Mathf.Abs(speed.magnitude) < 0.01f)){
				anim.Play("idle");
			}
		}

		if (speed.magnitude > maxSpeed){
			speed = speed.normalized * maxSpeed;
		}
		if (speed.magnitude > 0){
			speed *= friction;
			if (speed.magnitude < 0.001f){
				stopMoving();
			}
		}
		if (canMove){
			float faceX = anim.GetFloat("FaceX");
			float faceY = anim.GetFloat("FaceY");

			rb.MovePosition(rb.position + speed * Time.deltaTime);
		}
	}

	//Expects normalized input!
	public void move(float faceX, float faceY){
		Assert.AreApproximatelyEqual((new Vector2(faceX, faceY)).magnitude, 1f);

		if (!canMove){
			return;
		}
		if (state.IsName("idle") || state.IsName("walk")){
			if (Mathf.Abs(faceX) > 0.5){
				if (Mathf.Abs(faceY) > 0.5){
					anim.SetFloat("FaceY", Mathf.Sign(faceY));
					anim.SetFloat("FaceX", 0);

					Vector3 currentScale = transform.localScale;
					currentScale.x = Mathf.Abs(currentScale.x) * Mathf.Sign(faceX) * Mathf.Sign(faceY);
					transform.localScale = currentScale;
				} else {
					anim.SetFloat("FaceX", Mathf.Sign(faceX));
					anim.SetFloat("FaceY", 0);

					Vector3 currentScale = transform.localScale;
					currentScale.x = Mathf.Abs(currentScale.x) * Mathf.Sign(faceX);
					transform.localScale = currentScale;
				}
			} else if (Mathf.Abs(faceY) > 0.5){
				anim.SetFloat("FaceY", Mathf.Sign(faceY));
				anim.SetFloat("FaceX", 0);
			}

			if (faceX != 0){
				speed.x += maxSpeed * faceX;
			}
			if (faceY != 0){
				speed.y += maxSpeed * faceY;
			}

			if (state.IsName("idle")){
				anim.Play("walk");
			}
		}
	}

	public virtual void OnCollisionStay2D(Collision2D col){
		/*
		if (col.collider.CompareTag("interactable")){
			Debug.Log("Can interact!");
		}
		*/
	}

	public virtual void OnTriggerEnter2D(Collider2D col){
		//More Stuff
		/*
		if (col.CompareTag("key")){
			if (ManagerScript.man.currentChar == charaIndex){
				Destroy(col.gameObject);
				ManagerScript.man.keyNumber += 1;
			}
		}
		*/
	}
}
