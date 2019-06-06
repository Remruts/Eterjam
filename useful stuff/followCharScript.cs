using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Script para que un personaje siga a un objeto, como si fueran las parties de rpgs antiguos.
Va en línea recta, no tiene interpolación, ni evita obstáculos.
Para fancy A* usar el script de eso.
*/
public class followCharScript : MonoBehaviour {
	public Transform target;
	List<Vector3> positionsList;

	charaMovementScript scr;

	public float proximityToTarget = 2f;
	public float pointRadius = 0.6f;
	public int maxLength = 4;
	public bool isFollowing = true;

	void Start () {
		positionsList = new List<Vector3>();
		scr = GetComponent<charaMovementScript>();
	}

	void Update () {
		if (target == null || !isFollowing){
			return;
		}
		if (positionsList.Count > 0){

			Vector3 toTarget = target.position - transform.position;
			Vector3 newDirection = positionsList[0] - transform.position;

			if (newDirection.magnitude <= pointRadius*2.0 || positionsList.Count > maxLength){
				positionsList.RemoveAt(0);
			} else {
				if ((positionsList[positionsList.Count - 1] - target.position).magnitude > pointRadius){
					positionsList.Add(target.position);
				}

				if (toTarget.magnitude > proximityToTarget){
					newDirection = (positionsList[0] - transform.position).normalized;
					scr.move(newDirection.x, newDirection.y);
				} else {
					scr.stopMoving();
				}
			}
		} else {
			positionsList.Add(target.position);
		}
	}
	/*
	// Visualizar el recorrido
	void OnDrawGizmos(){
		if (positionsList != null){
			foreach (Vector3 v in positionsList){
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(v, Vector3.one * 0.5f);
			}
		}
	}
	*/
}
