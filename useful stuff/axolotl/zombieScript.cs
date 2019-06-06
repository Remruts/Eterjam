using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieScript : enemyScript {

	Vector3 pivotPosition;
	Vector3 patrolTarget;

	float currentPatrolTime;
	float patrolTime;
	bool isACoward = false;
	float oldSpeed;

	// Use this for initialization
	override protected void Start () {
		base.Start();
		actionState = "patrolling";
		pivotPosition = transform.position;
		patrolTarget = getPatrolTarget();
		patrolTime = Random.Range(3f, 5f);
		currentPatrolTime = Time.time;
		oldSpeed = maxSpeed;

		/*
		if (Random.Range(0, 10) < 2){
			isACoward = true;
		}
		*/
	}

	// Update is called once per frame
	override protected void Update () {
		base.startUpdate();
		if (!dead){
			if (!hurt){
				Vector3 target = managerScript.man.playerChar.transform.position;

				if (isACoward){
					if ((target -transform.position).magnitude < 5f && managerScript.man.numberOfEnemiesAround(transform.position, 6f) == 1){
						actionState = "runningAway";
					}
				}

				switch(actionState){
				case "attacking":
					maxSpeed = oldSpeed * 2f;
					if (target == transform.position){
						actionState = "patrolling";
						break;
					}
					if (state.IsName("idle") || state.IsName("walk")){
						moveToTarget(target);
					}

					if ((target -transform.position).magnitude >= 7f){
						patrol();
					}
				break;
				case "patrolling":
					maxSpeed = oldSpeed;
					if (Time.time - currentPatrolTime > patrolTime){
						patrol();
					} else {
						if ((patrolTarget - transform.position).magnitude > 0.8f){
							moveToTarget(patrolTarget);
						} else {
							stopMoving();
						}
					}

					if ((target -transform.position).magnitude < 5f){
						actionState = "attacking";
					}
				break;
				case "runningAway":
					//maxSpeed = oldSpeed*1.5f;
					moving = true;
					speed = (transform.position - target).normalized * maxSpeed;
					if (managerScript.man.numberOfEnemiesAround(transform.position, 6f) > 1){
						patrol();
					}
				break;
				}

				updateMovingAnimations();
			}
			avoidObstacles();
		}

		base.endUpdate();
	}

	void patrol(){
		actionState = "patrolling";
		patrolTarget = getPatrolTarget();
		patrolTime = Random.Range(3f, 5f);
		currentPatrolTime = Time.time;
		maxSpeed = oldSpeed;
	}

	Vector3 getPatrolTarget(){
		float distance = Random.Range(1f, 4f);
		float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

		return pivotPosition + new Vector3(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle), 0f);
	}

}
