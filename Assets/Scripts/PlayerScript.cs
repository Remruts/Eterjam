﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
  Rigidbody2D rb;
  bool isHolding = false;
[Header("Identificación")]
  public int team = 0;
  public int id = 0;

[Header("Movimiento")]
  public float speed = 5f;
  public float maxSpeed = 5f;
  float speedTimer = 0f;
  public float projectileSpeed = 300f;
  public float jumpForce = 300f;
  public float dashSpeed = 600f;
  public float maxCooldown = 1f;
  public float dashLength = 0.5f;
  public int maxDashes = 1;
  float friction = 0.8f;

  public float baseGravity = 3f;
  float currentGravity = 3f;
  float gravityTimer = 0f;
  bool grounded = false;
  
  public int shotsToOverheat = 10;
  int currentShotsLeft;
  float reloadTimer = 0f;
  public float flickSpeed = 10f;
  Vector2 realFlick = Vector2.zero;
  float cooldown = 0f;

  Vector2 movement;  
  float faceDir = 1f;
  bool canJump = false;
  bool isDashing = false;
  float dashTimer = 0f;
  float dashBounceTimer = 0f;
  Vector2 dashDirection = Vector2.zero;
  int availableDashes;

[Header("Colisiones")]  
  public LayerMask solidMask;

[Header("Objetos del Player")]
  public GameObject projectilePrefab;
  public GameObject fantasmitaPrefab;
  public GameObject arrowBar;
  SpriteRenderer arrowSprite;

  public GameObject playerGraphics;
  public Collider2D floorChecker;
  CircleCollider2D bodyCollider;
  Animator anim;
  SpriteRenderer spr;
  Vector2 startingScale;

[Header("Audio")]
  AudioSource audioSource;
  public AudioClip dashSound;
  public AudioClip fireSound;
  public AudioClip jumpSound;

[Header("Efectos")]
  public GameObject dashEffect;
  public GameObject landingPartsPrefab;
  public GameObject jumpingPartsPrefab;
  public GameObject collisionPartsPrefab;

  // Start is called before the first frame update
  void Start(){
    availableDashes = maxDashes;
    faceDir = playerGraphics.transform.localScale.x;

    currentGravity = baseGravity;
    currentShotsLeft = shotsToOverheat;
    
    rb = GetComponent<Rigidbody2D>();
    if (ManagerScript.coso != null){
      ManagerScript.coso.addPlayer(this);
    }

    bodyCollider = GetComponent<CircleCollider2D>();

    arrowSprite = arrowBar.GetComponent<SpriteRenderer>();
    anim = playerGraphics.GetComponent<Animator>();
    spr = playerGraphics.GetComponent<SpriteRenderer>();
    audioSource = GetComponent<AudioSource>();

    startingScale = playerGraphics.transform.localScale;
  }

  // Update is called once per frame
  void Update()
  {
    if (ManagerScript.coso != null && ManagerScript.coso.paused){
        return;
    }
    checkFloor();
    checkForDashes();
    if (!isDashing){
        move();
        jump();
        if (cooldown == 0f){
            shoot();
        } else if (cooldown > 0f){
            cooldown -= Time.deltaTime;
            if (cooldown < 0f){
                cooldown = 0f;
            }
        }
    } else {
        arrowBar.gameObject.SetActive(false);
        //movement = dashDirection * dashSpeed;
        rotateWhenInDash();
    }

    dashBounceTimer -= Time.deltaTime;
    if (dashBounceTimer <= 0f){
        dashBounceTimer = 0f;
    }

    gravityTimer -= Time.deltaTime;
    if (gravityTimer <= 0f){
      currentGravity = baseGravity;
      gravityTimer = 0f;
    }

    speedTimer -= Time.deltaTime;
    if (speedTimer <= 0f){
      speed = maxSpeed;
      speedTimer = 0f;
    }

    reloadTimer -= Time.deltaTime;
    if (reloadTimer <= 0f){
      if (currentShotsLeft < shotsToOverheat){
        currentShotsLeft += 1;
        reloadTimer = 0.5f - 0.5f * ((float)currentShotsLeft / (float)shotsToOverheat);
      }
    }
  }

  void FixedUpdate(){    
    
    Vector3 castOffset = movement.normalized * bodyCollider.radius; 
    RaycastHit2D hit = Physics2D.BoxCast(transform.position + castOffset, new Vector2(0.5f, 0.5f), 0f, movement, 0.5f, solidMask.value);
    Debug.DrawRay(transform.position, movement);
    
    if (hit){
        //Debug.Log(hit.point);
        if (isDashing) {            
            // Si está dasheando y se choca, debería reflejarse
            if (hit.collider.CompareTag("furniture")){
              //Debug.Log("Collision!");
              hit.collider.attachedRigidbody.AddForceAtPosition(dashDirection * 2500f, hit.point);
            }
            if (dashBounceTimer <= 0f){
              dashDirection = Vector2.Reflect(dashDirection, hit.normal) * friction;
              
              //Pequeño screenshake
              CamScript.screen.shake(0.05f, 0.5f);

              dash();
              dashBounceTimer = 0.3f;

              GameObject spikeyParts = Instantiate(collisionPartsPrefab, hit.point, Quaternion.identity);
              float spikeyPartsAngle = Mathf.Atan2(transform.position.y - hit.point.y, transform.position.x - hit.point.x) * Mathf.Rad2Deg;
              landingPartsScript spikeyPartsScr = spikeyParts.GetComponent<landingPartsScript>();
              spikeyPartsScr.startAngle = spikeyPartsAngle - 120f;
              spikeyPartsScr.maxAngle = 180f;
            } else {
              movement = Vector2.zero;
            }

            GameObject parts = Instantiate(landingPartsPrefab, hit.point, Quaternion.identity);
            float partsAngle = Mathf.Atan2(transform.position.y - hit.point.y, transform.position.x - hit.point.x) * Mathf.Rad2Deg;
            landingPartsScript partsScr = parts.GetComponent<landingPartsScript>();
            partsScr.startAngle = partsAngle - 120f;
            partsScr.maxAngle = partsAngle + 120f;
        } else { 
            movement.x = 0f;
        }
    }

    if (!isDashing){
      movement *= friction;
    }
    
    rb.velocity = movement;

    /*if (Mathf.Abs(rb.velocity.x) > movementX)
    rb.velocity = new Vector2(movementX, rb.velocity.y);
    */
  }

  void OnCollisionEnter2D(Collision2D col) {
    if (col.gameObject.CompareTag("projectile")){
      if (col.gameObject.GetComponent<ProjectileScript>().team == team){
        return;
      }
      ManagerScript.coso.onPlayerDeath(this);
      Destroy(gameObject);
      GameObject fantasmita = Instantiate(fantasmitaPrefab, transform.position, Quaternion.identity);
      var scr = fantasmita.GetComponent<fantasmitaScript>();
      scr.team = team;
      scr.flip = (Mathf.Sign(playerGraphics.transform.localScale.x) > 0f) ? spr.flipX : !spr.flipX;
    }
  }

  public void changeGravity(float percentage, float time){
      gravityTimer = time;
      currentGravity = baseGravity * percentage;
  }

  public void changeSpeed(float percentage, float time){
      speedTimer = time;
      speed = maxSpeed * percentage;
  }

  void shoot(){
    // Shooting
    Vector2 flick = new Vector2(Input.GetAxisRaw("P" + id.ToString() +"FlickX"), -Input.GetAxisRaw("P" + id.ToString() + "FlickY"));    

    if (isHolding){
      // FIXME!! GRAVEDAD!
      if (!grounded){
        movement.x = 0f;
      }

      Debug.Log($"palanca: {flick}, realFlick, {realFlick}");

      // TODO: SENSIBILIDAD FLICK
      if (flick.magnitude > 0.15f){
        //Esto es para cambiar de lado el sprite dependiendo del flick
        Vector3 prevScale = playerGraphics.transform.localScale;
        prevScale.x = startingScale.x * Mathf.Sign(flick.x) * (spr.flipX ? -1f : 1f);
        playerGraphics.transform.localScale = prevScale;

        // y esto es para darle el poder al flick
        realFlick += flick * flickSpeed * Time.deltaTime;
        realFlick.x = Mathf.Clamp(realFlick.x, -3f, 3f);
        realFlick.y = Mathf.Clamp(realFlick.y, -3f, 3f);
      }
      
      
      // Esta es la flechita (Mover a una función de dibujar flechita?)
      arrowBar.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(-realFlick.y, -realFlick.x) * Mathf.Rad2Deg);
      arrowBar.transform.position = transform.position 
        + new Vector3(-realFlick.normalized.x, -realFlick.normalized.y, 0f);
      Vector3 dummyScale = arrowBar.transform.localScale;
      dummyScale.x = -Mathf.Sign(transform.localScale.x);
      arrowBar.transform.localScale = dummyScale;
      arrowSprite.size = new Vector2(1.0f + 1.5f * realFlick.magnitude / 4.24f, 0.7f);

      // Esto dispararía el objeto
      if (flick.magnitude < 0.5f && currentShotsLeft > 0){
        currentShotsLeft -= 1;
        reloadTimer = 0.5f;
        if (!grounded){
          changeGravity(0.05f, 0.15f);
          changeSpeed(0f, 0.15f);
        }
        anim.Play("Throw");
        audioSource.PlayOneShot(fireSound);
        Vector2 projectilePos = -realFlick.normalized;
        GameObject aProjectile = Instantiate(projectilePrefab, transform.position + new Vector3(projectilePos.x, projectilePos.y, 0f) * 1.5f, Quaternion.identity) as GameObject;
        Rigidbody2D projectileRb = aProjectile.GetComponent<Rigidbody2D>();
        aProjectile.GetComponent<ProjectileScript>().team = team;

        Vector2 laFuerza = -realFlick * projectileSpeed;
        laFuerza = laFuerza + laFuerza.normalized * 100f;
        projectileRb.AddForce(laFuerza);

        isHolding = false;

        cooldown = maxCooldown;
        arrowBar.gameObject.SetActive(false);
      }
    } else {
      if (flick.magnitude > 0.5f){
        if (!grounded){
          changeGravity(0.01f, 0.75f);
          changeSpeed(0f, 0.75f);
        }
        isHolding = true;
        realFlick = flick;
        arrowBar.gameObject.SetActive(true);
      }
    }
  }

  void move(){
    float inputX = Input.GetAxis("P" + id.ToString() + "Horizontal");        
    if (Mathf.Abs(inputX) > 0.5f)
    {
      movement.x = inputX * speed;
      faceDir = Mathf.Sign(movement.x);
      anim.SetBool("standing", false);
    } else {            
      //movementX = movementX * friction;      
      anim.SetBool("standing", true);
    }    
  }

  void jump(){
    if (canJump){
      if (Input.GetButton("P" + id.ToString() + "Jump")){
        anim.Play("Jump");
        //rb.AddForce(new Vector2(0f, jumpForce));
        movement.y = jumpForce;
        GameObject jumpParts = Instantiate(jumpingPartsPrefab, transform.position + Vector3.down * 0.75f, Quaternion.identity);
        landingPartsScript jumpPartsScr = jumpParts.GetComponent<landingPartsScript>();
        jumpPartsScr.startAngle = 0f;
        jumpPartsScr.maxAngle = 180f;
        //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        canJump = false;
        audioSource.PlayOneShot(jumpSound, 2f);
      }
    }
  }

  void rotateWhenInDash(){
      if (isDashing){
        //Calculamos la rotación del personaje dependiendo de la dirección del dash

        bool estaFlipeado = (spr.flipX && playerGraphics.transform.localScale.x >= 0f) || (!spr.flipX && playerGraphics.transform.localScale.x < 0f);

        float nonFlipAngle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;
        float angle = nonFlipAngle + (estaFlipeado ? 0f : 180f);

        if (angle < 0){
            angle += 360;
        } else if (angle > 360){
            angle -= 360;
        }

        if (angle > 90 && angle < 270){
            spr.flipY = true;
        } else {
            spr.flipY = false;
        }

        playerGraphics.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        dashEffect.transform.rotation = Quaternion.AngleAxis(nonFlipAngle + 90f, Vector3.forward);
      }
  }

  void checkForDashes(){
    if (availableDashes > 0){            
      if (Input.GetButtonDown("P" + id.ToString() + "Dash")) {
          dashDirection = new Vector2(Input.GetAxis("P" + id.ToString() + "Horizontal"), -Input.GetAxis("P" + id.ToString() + "Vertical")).normalized;
          
          dash();
          
          availableDashes -= 1;
      }
    }

    if (isDashing) {            
        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0f){
          stopDashing();  
        }
        
    }
  }

  void dash(){
    isDashing = true;
    anim.Play("Dash");
    GetComponent<Collider2D>().enabled = false;
    dashEffect.GetComponent<particleScript>().Emit();

    GameObject jumpParts = Instantiate(jumpingPartsPrefab, transform.position, Quaternion.identity);
    landingPartsScript jumpPartsScr = jumpParts.GetComponent<landingPartsScript>();
    jumpPartsScr.startAngle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg + 90f;
    jumpPartsScr.maxAngle = 180f;

    dashTimer = dashLength;
    movement = dashDirection * dashSpeed;
  }

  void stopDashing() {        
        //Debug.Log("terminó el dash");        
        dashTimer = 0f;
        isDashing = false;
        GetComponent<Collider2D>().enabled = true;
        Vector3 prevScale = playerGraphics.transform.localScale;
        prevScale.x = startingScale.x * (spr.flipX ?  Mathf.Sign(movement.x) : -Mathf.Sign(movement.x));
        playerGraphics.transform.localScale = prevScale;
        movement = movement * 0.5f;

        spr.flipY = false;
        playerGraphics.transform.rotation = Quaternion.identity;
  }

  void checkFloor(){    
    //Collider2D col = Physics2D.OverlapBox(rb.transform.position + Vector3.down * 0.75f, new Vector2(0.5f, 0.2f), 0f, solidMask.value);
    grounded = floorChecker.IsTouchingLayers(solidMask.value);
    if (grounded){
      if (movement.y <= 0f && !canJump){
        //partículas de cuando cae al piso        
        Instantiate(landingPartsPrefab, transform.position + Vector3.down * 0.75f, Quaternion.identity);

        //rb.velocity = new Vector2(rb.velocity.x, 0f);
        /*
        if (!isDashing) {
            movement.y = 0f;
        }
        */
        
        canJump = true;
        var state = anim.GetCurrentAnimatorStateInfo(0); 
        if (state.IsName("Jump")){
          anim.Play("Idle");
        }
      }
      //rb.gravityScale = 0f;
      if (!isDashing){
        availableDashes = maxDashes;
      }      
    } else {      
      //rb.gravityScale = 4f;
      if (!isDashing){
        movement.y -= currentGravity;
      }
    }
  }
}
