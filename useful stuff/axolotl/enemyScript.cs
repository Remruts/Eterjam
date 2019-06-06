using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour {

	public float maxLives = 3f;
	public float maxSpeed = 0.02f;
	public float knockback = 10f;
	public float hurtTime = 0.5f;
	public GameObject explosion;
	public GameObject heart_pickup;
	public int heartValue = 15;
	public int heartNumber = 5;

	protected float obstacleRadius = 0.5f;

	protected float lives;
	protected bool hurt = false;
	protected bool dead = false;
	protected bool moving = false;
	float prevSpeed;

	protected string actionState;

	protected SpriteRenderer spr;
	protected Rigidbody2D rb;

	protected Animator anim;
	protected AnimatorStateInfo state;

	protected Vector3 speed = Vector3.zero;

	protected List<Vector3> currentPath;
	protected float pathfindingTime;
	protected float pathfindingDeltaTime = 0.1f;

	protected float hurtTimer = 0f;

	public LayerMask solidMask = -1;

	// Use this for initialization
	virtual protected void Start () {
		spr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		lives = maxLives;
		pathfindingTime = Time.time;
	}

	// Update is called once per frame
	virtual protected void Update () {
		startUpdate();
		endUpdate();
	}

	virtual protected void startUpdate(){
		state = anim.GetCurrentAnimatorStateInfo(0);
		if (hurt){
			hurtTimer -= Time.deltaTime;
			if (hurtTimer <= 0){
				hurt = false;
				maxSpeed = prevSpeed;
			}
		}
		if (!moving){
			speed *= 0.8f;
			if (speed.magnitude < 0.01){
				speed.x = 0;
				speed.y = 0;
				if (state.IsName("walk")){
					anim.Play("idle");
				}
			}
		}
	}

	virtual protected void endUpdate(){
		if (!dead){
			if (hurt){
				spr.color = new Color(173f/255f, 94f/255f, 94f/255f);
			} else {
				spr.color = Color.white;
			}

			// Check both
			if (speed.x != 0 || speed.y != 0){
				RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(0.87f, 0.29f), 0f, speed, maxSpeed, solidMask.value);

				if (hit.collider != null){
					if (hit.collider.CompareTag("solid")){
						speed.y = 0f;
						speed.x = 0f;
					}
				}
			}

			//check to see if still colliding
			RaycastHit2D doubleCheck = Physics2D.BoxCast(transform.position, new Vector2(0.87f, 0.29f), 0f, speed, 0, solidMask.value);
			if (doubleCheck.collider != null){
				transform.position += (transform.position - new Vector3(doubleCheck.point.x, doubleCheck.point.y, 0f)).normalized * maxSpeed;
			}

			if (speed.magnitude > maxSpeed){
				speed = speed.normalized * maxSpeed;
			}
			transform.position += speed;

			if (lives <= 0){
				die();
			}

		}

		if (state.normalizedTime >= 1){
			if (state.IsName("attack")){
				anim.Play("idle");
			}
		}
	}

	protected void stopMoving(){
		moving = false;
		speed.x = 0;
		speed.y = 0;
	}

	protected void moveToTarget(Vector3 target) {
		if (!hurt){
			if (Time.time - pathfindingTime >= pathfindingDeltaTime){
				currentPath = AStar.pathfinding.FindPath(transform.position, target);
				pathfindingTime = Time.time;
			}

			if ((target - transform.position).magnitude > 0.5f){
				if (currentPath != null && currentPath.Count != 0){
					Vector3 deltaPos = currentPath[0] - transform.position;
					if (deltaPos.magnitude > 0.1f){
						moving = true;
						speed = deltaPos.normalized * maxSpeed;
					} else {
						currentPath.RemoveAt(0);
					}
				}
			}
		}
	}

	protected void updateMovingAnimations(){
		if (moving){
			Vector3 currentScale = transform.localScale;
			if (Mathf.Abs(speed.x) >= Mathf.Abs(speed.y)){
				currentScale.x = Mathf.Sign(-speed.x) * Mathf.Abs(currentScale.x);
			}
			transform.localScale = currentScale;

			if (state.IsName("idle")){
				anim.Play("walk");
			}
		}
	}

	virtual public void die(){
		dead = true;
		int maxExplosions = Random.Range(1, 4);
		for (int i=0; i < maxExplosions; i++){
			Instantiate(explosion, transform.position + Vector3.up * Random.Range(-1f, 1f) + Vector3.right * Random.Range(-1f, 1f), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
		}
		camScript.screen.shake(0.15f, 0.5f);

		for (int i=0; i < Random.Range(heartNumber, heartNumber + 3); i++){
			GameObject heart =Instantiate(heart_pickup, transform.position, Quaternion.identity) as GameObject;
			heart.GetComponent<heartPickupScript>().changeLives(Random.Range(heartValue, heartValue + 5));
		}

		Destroy(gameObject);
	}

	protected void avoidObstacles(){
		Vector3 nearestPoint = transform.position;
		if (managerScript.man.enemies.Count > 1){

			bool found = false;
			for (int i = 0; i < managerScript.man.enemies.Count; i++){
				if (managerScript.man.enemies[i] == null || managerScript.man.enemies[i] == gameObject){
					continue;
				}
				nearestPoint = managerScript.man.enemies[i].transform.position;
				found = true;
				break;
			}
			if (!found){
				return;
			}

			Vector3 ahead = transform.position + speed.normalized*maxSpeed*2;

			foreach (GameObject obstacle in managerScript.man.enemies){
				if (obstacle == null || obstacle == gameObject){
					continue;
				}
				if (Vector3.Distance(ahead, nearestPoint) > Vector3.Distance(ahead, obstacle.transform.position)){
					nearestPoint = obstacle.transform.position;
				}
			}
			if (Vector3.Distance(nearestPoint, transform.position) <= obstacleRadius){
				Vector3 avoidanceForce = (ahead - nearestPoint).normalized * 0.1f;
				moving = true;
				speed += avoidanceForce;
			}
		}
	}

	virtual protected void OnTriggerEnter2D(Collider2D col){
		if (col.CompareTag("player_hitbox")){
			if (!hurt){
				bubbleScript bscr = col.gameObject.GetComponent<bubbleScript>();
				lives -= (managerScript.man.attack * bscr.bubbleType.attk);
				hurt = true;
				stopMoving();
				hurtTimer = hurtTime;

				Vector3 direction;
				if (col.gameObject.transform.parent != null){
					direction = (transform.position - col.gameObject.transform.parent.position).normalized;
				} else {
					direction = (transform.position - col.gameObject.transform.position).normalized;
				}
				speed = direction * knockback;
				prevSpeed = maxSpeed;
				maxSpeed = 0.3f;
			}
		}
	}
	void OnDrawGizmos(){
		if (managerScript.man == null){
			return;
		}
		if (managerScript.man.debug){
			Gizmos.color = Color.red;
			if (currentPath != null && currentPath.Count > 0){
				foreach(Vector3 p in currentPath){
					Gizmos.DrawCube(p, Vector3.one * 0.2f);
				}
			}
		}
	}
}
