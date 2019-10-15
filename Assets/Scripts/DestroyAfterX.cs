using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Script que agrega funcionalidad de autodestrucción con un timer.
También tiene la opción de spawnear otro objeto cuando se destruye.
Está bueno para efectos y sistemas con partículas.
*/
public class DestroyAfterX : MonoBehaviour {

	public float time = 1f;
	public GameObject objectToSpawn;

	void Start () {
		Invoke("die", time);
	}

	void die(){
		Destroy(this.gameObject);
		if (objectToSpawn != null){
			Instantiate(objectToSpawn, transform.position, Quaternion.identity);
		}
	}
}
