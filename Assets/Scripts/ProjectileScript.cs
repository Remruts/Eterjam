using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
  float initialTime;
  float lifeSpam;
  public GameObject explosionPrefab;
  public GameObject trail;
  public AudioClip bounceSound;
  public GameObject sparks;
  public Gradient[] teamGradients;  
  public int team = 0;
  
  Rigidbody2D theBody;
  AudioSource audioSource;
  GameObject aTrail;
  SpriteRenderer spr;

  public CircleCollider2D attractorField;
  
  
  // Start is called before the first frame update
  void Start(){
    initialTime = Time.time;
    lifeSpam = 1.5f;
    
    theBody = GetComponent<Rigidbody2D>();
    theBody.AddTorque(Random.Range(-10f, 10f));

    spr = GetComponent<SpriteRenderer>();

    audioSource = GetComponent<AudioSource>();

    aTrail = Instantiate(trail, transform.position, transform.rotation);
    aTrail.GetComponent<TrailRenderer>().colorGradient = teamGradients[team];
    aTrail.transform.SetParent(transform);
    
    UpdateShader();
  }

  // Update is called once per frame
  void Update(){
    UpdateShader();
    if (Time.time - initialTime > lifeSpam){
      morir();
    }

    attractToClosestProjectile();
  }

  void attractToClosestProjectile(){
    RaycastHit2D[] results = new RaycastHit2D[20];
    attractorField.Cast(Vector2.zero, results, 0f, true);
    GameObject closest = null;
    float minDistance = Mathf.Infinity;
    
    foreach (var result in results){
      if (!result){
        break;
      }
      var col = result.collider;
      if (col.CompareTag("projectile")){
        if (col.gameObject.GetComponent<ProjectileScript>().team != team){
          Vector3 p = result.point;
          float newDistance = (p - transform.position).sqrMagnitude;
          if (closest == null){
            closest = col.gameObject;
            minDistance = newDistance;
          } else {              
            if (newDistance < minDistance){
              closest = col.gameObject;
              minDistance = newDistance;
            }
          }
        }
      }
    }

    if (closest != null){
      Vector2 transPos = (closest.transform.position - transform.position);
      theBody.velocity = (transPos / attractorField.radius) * 50f;
    }
  }

  void OnCollisionEnter2D(Collision2D col){

    Instantiate(sparks, transform.position, Quaternion.identity);

    if (audioSource != null && audioSource.enabled){
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

    void UpdateShader()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spr.GetPropertyBlock(mpb);
        //mpb.SetFloat("_Outline", outline ? 1f : 0f);        
        mpb.SetColor("_OutlineColor", teamGradients[team].Evaluate(0f));
        mpb.SetFloat("_OutlineSize", 16.0f);
        spr.SetPropertyBlock(mpb);
    }

  void morir(){
    if (aTrail != null){
        aTrail.transform.SetParent(null);
    }
    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    CamScript.screen.shake(0.1f, 0.5f);
    Destroy(gameObject);    
  }
}
