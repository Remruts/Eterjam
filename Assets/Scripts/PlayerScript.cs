using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
  Rigidbody2D rb;
  bool isHolding = false;

  public float speed = 5f;
  public float maxSpeed = 5f;
  public float projectileSpeed = 300f;
  public float jumpForce = 300f;
  public float dashSpeed = 600f;
  public float maxCooldown = 1f;
  public float dashLength = 0.5f;
  public int maxDashes = 1;
  float friction = 0.8f;

  bool canJump = false;
  bool isDashing = false;
  float dashTimer = 0f;
  Vector2 dashDirection = Vector2.zero;
  int availableDashes;
  
  public LayerMask solidMask;

  public GameObject projectilePrefab;
  public GameObject fantasmitaPrefab;
  public GameObject arrowBar;
  SpriteRenderer arrowSprite;
  public int team = 0;
  public int id = 0;

  public float flickSpeed = 1f;
  Vector2 realFlick = Vector2.zero;
  float cooldown = 0f;

  Vector2 movement;
  float faceDir = 1f;

  Animator anim;
  SpriteRenderer spr;

  AudioSource audioSource;
  public AudioClip dashSound;
  public AudioClip fireSound;
  public AudioClip jumpSound;

  public GameObject dashEffect;
  public GameObject landingPartsPrefab;
  public GameObject collisionPartsPrefab;

  // Start is called before the first frame update
  void Start(){
    availableDashes = maxDashes;
    faceDir = transform.localScale.x;

    arrowSprite = arrowBar.GetComponent<SpriteRenderer>();
    
    rb = GetComponent<Rigidbody2D>();
    if (ManagerScript.coso != null){
      ManagerScript.coso.addPlayer(this);
    }

    anim = GetComponent<Animator>();
    spr = GetComponent<SpriteRenderer>();
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
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
  }

  void FixedUpdate(){    
    
    RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(0.5f, 0.5f), 0f, movement, 0.5f, solidMask.value);
    Debug.DrawRay(transform.position, movement);
    
    if (hit){
        //Debug.Log(hit.point);
        if (isDashing) {            
            // Si está dasheando y se choca, debería reflejarse
            if (hit.collider.CompareTag("furniture")){
              Debug.Log("Collision!");
              hit.collider.attachedRigidbody.AddForceAtPosition(dashDirection * 2500f, hit.point);
            }
            if (availableDashes >= 0){
              dashDirection = Vector2.Reflect(dashDirection, hit.normal) * friction;
              dash();

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
      scr.flip = (Mathf.Sign(transform.localScale.x) > 0f) ? spr.flipX : !spr.flipX;
    }
  }

  void shoot(){
    // Shooting
    Vector2 flick = new Vector2(Input.GetAxisRaw("P" + id.ToString() +"FlickX"), -Input.GetAxisRaw("P" + id.ToString() + "FlickY"));

    if (isHolding){
      // FIXME!! GRAVEDAD!
      //movement.y = 0f;

      if (flick.magnitude > 0.15f){
        //Esto es para cambiar de lado el sprite dependiendo del flick
        Vector3 prevScale = transform.localScale;
        prevScale.x = Mathf.Sign(flick.x) * (spr.flipX ? -1f : 1f);
        transform.localScale = prevScale;

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
      if (flick.magnitude < 0.5f){
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
        //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        canJump = false;
        audioSource.PlayOneShot(jumpSound, 2f);
      }
    }
  }

  void rotateWhenInDash(){
      if (isDashing){
        //Calculamos la rotación del personaje dependiendo de la dirección del dash

        bool estaFlipeado = (spr.flipX && transform.localScale.x >= 0f) || (!spr.flipX && transform.localScale.x < 0f);
        float angle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg + (estaFlipeado ? 0f : 180f);

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

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
      }
  }

  void checkForDashes(){
    if (availableDashes > 0){            
      if (Input.GetButtonDown("P" + id.ToString() + "Dash")) {
          dashDirection = new Vector2(Input.GetAxis("P" + id.ToString() + "Horizontal"), -Input.GetAxis("P" + id.ToString() + "Vertical")).normalized;
          dash();
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

    dashTimer = dashLength;
    movement = dashDirection * dashSpeed;

    availableDashes -= 1;
  }

  void stopDashing() {        
        //Debug.Log("terminó el dash");        
        dashTimer = 0f;
        isDashing = false;
        GetComponent<Collider2D>().enabled = true;
        Vector3 prevScale = transform.localScale;
        prevScale.x = (spr.flipX ? Mathf.Sign(movement.x) : -Mathf.Sign(movement.x));
        transform.localScale = prevScale;
        movement = movement * 0.5f;

        spr.flipY = false;
        transform.rotation = Quaternion.identity;
  }

  void checkFloor(){
    //RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, new Vector2(0f, -1f), 0.8f, solidMask.value);
    Collider2D col = Physics2D.OverlapBox(rb.transform.position + Vector3.down * 0.75f, new Vector2(0.5f, 0.2f), 0f, solidMask.value);
    if (col != null){
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
        movement.y -= 3f;
      }
    }
  }  

  void OnDrawGizmos(){    
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(transform.position + Vector3.down * 0.75f, new Vector3(0.5f, 0.2f, 1f));    
  }
}
