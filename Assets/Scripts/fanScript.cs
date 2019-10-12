using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fanScript : MonoBehaviour{

  float maxWiggleAngle = 10f;
  float wiggleAngle = 0f;

  float wiggleSpeed = 0f;

  bool isWiggling = false;

  float life = 10f;
  public float startingLife = 10f;
  CustomTimer reviveTimer;
  
  Collider2D fanCollider;
  Animator anim;

  void Start(){
    fanCollider = GetComponent<Collider2D>();
    anim = GetComponent<Animator>();
    reviveTimer = new CustomTimer(5f, 
      ()=> {
        fanCollider.enabled = true; 
        life = startingLife; 
        anim.enabled = true;
      }
    );
  }

  void Update(){

    if (Input.GetKeyDown(KeyCode.Space)){
      isWiggling = true;
      wiggleSpeed = -100f;
    }

    if (isWiggling){
      wiggleAngle += wiggleSpeed * Time.deltaTime;
      if (Mathf.Abs(wiggleAngle) > maxWiggleAngle){
        wiggleAngle = maxWiggleAngle * Mathf.Sign(wiggleAngle);
        wiggleSpeed = -wiggleSpeed;
      }
      if (wiggleAngle > 0.01f){
        wiggleSpeed -= 10f;
      } else if (wiggleAngle < -0.01f){
        wiggleSpeed += 10f;
      } else {
        isWiggling = false;
      }
      wiggleSpeed = wiggleSpeed * 0.9f;
    } else {
      wiggleAngle = Mathf.LerpAngle(wiggleAngle, 0f, Time.deltaTime);
    }
    transform.rotation = Quaternion.Euler(0f, 0f, wiggleAngle);

    if (!fanCollider.enabled){
      reviveTimer.tick(reviveTimer.getDuration);
    }
  }

  void OnCollisionEnter2D(Collision2D col){
    if (col.gameObject.CompareTag("projectile")){
      float theForce = col.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
      wiggle(col.contacts[0].point, theForce);

      life -= Mathf.Min(Random.Range(1f, Mathf.Max(theForce/10f, 1f)), 3f); 
      if (life <= 0f){
        fanCollider.enabled = false;
        anim.enabled = false;
      }
    }
  }

  public void wiggle(Vector2 contactPoint, float force){
    Vector2 pos = transform.position;
    float sign = 1.0f;
    if ((contactPoint.x < pos.x && contactPoint.y < pos.y) || (contactPoint.x > pos.x && contactPoint.y > pos.y)){
      sign = -1f;
    }
    isWiggling = true;
    wiggleSpeed = sign * 10f * force + Random.Range(-30f, 30f);
  }
}
