using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Script que agrega funcionalidad de autodestrucción con un timer.
También tiene la opción de spawnear otro objeto cuando se destruye.
Está bueno para efectos y sistemas con partículas.
*/
public class DestroyAfterX : MonoBehaviour {

  public float time2Die = 1f;
  public GameObject objectToSpawn;

  float startTime;
  ParticleSystem parts;

  void Start () {
    startTime = Time.time;
  }

  void Update(){
    if (Time.time > startTime + time2Die){
      die();
    }
  }

  void die(){
    parts = GetComponent<ParticleSystem>();
    if (parts != null){
      Destroy(parts);
    }
    Destroy(this.gameObject);
    if (objectToSpawn != null){
      Instantiate(objectToSpawn, transform.position, Quaternion.identity);
    }
  }
}
