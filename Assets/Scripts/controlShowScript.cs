using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class controlShowScript : MonoBehaviour
{
  public GameObject arrowBar;
  public GameObject stick;
  public GameObject projectilePrefab;
  public ParticleSystem dashParts;
  public float flickAngle;
  public float flickMagnitude;
  public bool lockStick = false;
  public bool dashing = false;
  SpriteRenderer arrowSprite;
  SpriteRenderer spr;
  Vector3 flickVector;
  Vector3 startingScale;
  // Start is called before the first frame update
  void Start(){
    arrowSprite = arrowBar.GetComponent<SpriteRenderer>();
    spr = GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  void Update()
  {
    flickAngle = new Angle360(flickAngle);
    flickVector = new Vector3(Mathf.Cos(flickAngle * Mathf.Deg2Rad), Mathf.Sin(flickAngle * Mathf.Deg2Rad), 0f);

    if (!dashing){
      Vector3 prevScale = transform.localScale;
      prevScale.x = Mathf.Sign(-flickVector.x);
      transform.localScale = prevScale;

    }

    if (arrowBar.activeSelf){
      arrowBar.transform.rotation = Quaternion.Euler(0f, 0f, flickAngle);
      arrowBar.transform.position = transform.position + flickVector;
      Vector3 dummyScale = arrowBar.transform.localScale;
      dummyScale.x = -Mathf.Sign(transform.localScale.x);
      arrowBar.transform.localScale = dummyScale;
      arrowSprite.size = new Vector2(1.0f + 1.5f * flickMagnitude / 4f, 0.7f);
    }
    if (!lockStick){
      updateStick();
    }
  }

  void updateStick(){
    if (stick != null){
      Vector3 stickVector = (-flickVector * flickMagnitude/5f);
      stickVector.x = Mathf.Clamp(stickVector.x, -1f, 1f);
      stickVector.y = Mathf.Clamp(stickVector.y, -1f, 1f);
      stick.transform.position = stick.transform.parent.position + stickVector;
    }
  }

  public void shootProjectile(){
    GameObject aProjectile = Instantiate(projectilePrefab, transform.position + flickVector * 1.5f, Quaternion.identity) as GameObject;
    Rigidbody2D projectileRb = aProjectile.GetComponent<Rigidbody2D>();
    aProjectile.GetComponent<ProjectileScript>().team = 0;

    Vector2 laFuerza = new Vector2(Mathf.Cos(flickAngle * Mathf.Deg2Rad), Mathf.Sin(flickAngle * Mathf.Deg2Rad)) * flickMagnitude * 300f;
    laFuerza = laFuerza + laFuerza.normalized * 100f;

    projectileRb.AddForce(laFuerza);
  }

  public void dash(){
    dashParts.Emit(20);
    
    if (flickAngle > 90 && flickAngle < 270){
      spr.flipY = true;
    } else {
      spr.flipY = false;
    }

    transform.rotation = Quaternion.AngleAxis(flickAngle, Vector3.forward);
  }
}
