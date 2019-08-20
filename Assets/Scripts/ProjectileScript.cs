using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
  float initialTime;
  float lifeSpam;
  public GameObject explosionPrefab;
  public AudioClip bounceSound;
  public GameObject sparks;
  
  Rigidbody2D theBody;
  AudioSource audioSource;
  
  
  // Start is called before the first frame update
  void Start(){
    initialTime = Time.time;
    lifeSpam = 1.5f;
    
    theBody = GetComponent<Rigidbody2D>();
    theBody.AddTorque(Random.Range(-10f, 10f));

    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update(){
    if (Time.time - initialTime > lifeSpam){
      morir();      
    }
  }

  void OnCollisionEnter2D(Collision2D col){

    Instantiate(sparks, transform.position, Quaternion.identity);

    if (audioSource != null){
        audioSource.PlayOneShot(bounceSound);
    }
    if (col.gameObject.CompareTag("fan")){ 
      if (theBody == null){
        return;
      }
      Vector3 velocity = theBody.velocity;
      Vector3 newDirection = new Vector3();
      
      float rand = Random.Range(0.0f, 1.0f);
      float sameDirectionProbability = 0.5f;
      float multiplier = 3.0f * Random.Range(0.0f, 1.0f) + 1.0f;
      
      if (rand < sameDirectionProbability){
        newDirection.x = Random.Range(0f, 50f) * -1.0f;
        newDirection.y = Random.Range(0f, 50f) * -1.0f;
        newDirection.z = velocity.z;
      } else {
        newDirection.x = Random.Range(0f, 50f);
        newDirection.y = Random.Range(0f, 50f) * -1.0f;
        newDirection.z = velocity.z;
      }

      theBody.velocity = newDirection;
      
    } else if (col.gameObject.CompareTag("Player")) {      
      morir();
    } else if (col.gameObject.CompareTag("projectile")){
      morir();
    }
  }

  void morir(){
    Destroy(gameObject);    
    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    CamScript.screen.shake(0.1f, 0.5f);
  }
}
