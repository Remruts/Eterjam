using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
  Rigidbody2D rb;
  bool isHolding = false;

  public float speed = 5f;
  public float maxSpeed = 5f;
  public float projectileSpeed = 300f;
  public float jumpForce = 300f;
  public float dashForce = 600f;
  public float maxCooldown = 1f;
  public float maxDashCooldown = 1f;

  bool canJump = false;
  bool isDashing = false;
  float dashCooldown = 0f;
  public LayerMask solidMask;

  public GameObject projectilePrefab;
  public RectTransform forceBar;
  public int team = 0;
  public int id = 0;

  public float flickSpeed = 1f;
  Vector2 realFlick = Vector2.zero;
  float cooldown = 0f;

  float movement = 0;
  float faceDir = 1f;

  Animator anim;
  SpriteRenderer spr;

  AudioSource audioSource;
  public AudioClip dashSound;
  public AudioClip fireSound;
  public AudioClip jumpSound;

  public GameObject dashEffect;
  public GameObject landingPartsPrefab;
  
  //InputMaster controls;

/*
  void Awake(){
    //controls = new InputMaster();
    //controls.Player.Jump.performed += _ => jump();
    //controls.Player.Dash.performed += _ => dash();
    //controls.Player.Movement.performed += ctx => move(ctx.ReadValue<float>());
    //controls.Player.Movement.cancelled += ctx => move(float);
    //controls.Player.Flick.performed += ctx => flick();
  }

  void OnEnable(){
    controls.Enable();
  }

  void OnDisable(){
    controls.Disable();
  }
*/

  // Start is called before the first frame update
  void Start(){    

    faceDir = transform.localScale.x;
    
    rb = GetComponent<Rigidbody2D>();
    if (ManagerScript.coso != null){
      ManagerScript.coso.addPlayer(this);
    }

    anim = GetComponent<Animator>();
    spr = GetComponent<SpriteRenderer>();
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void FixedUpdate(){
    
    /*
    dash();
    if (!isDashing){
      move();
      jump();
      if (cooldown == 0f) {
        shoot();
      } else if (cooldown > 0f){
        cooldown -= Time.deltaTime;
        if (cooldown < 0f){
          cooldown = 0f;
        }
      }
    }
    */
    
    dashCooldown -= Time.deltaTime;

    if (dashCooldown <= maxDashCooldown - 0.1f){
      isDashing = false;
      GetComponent<Collider2D>().enabled = true;
    }
    if (dashCooldown <= 0f){
      dashCooldown = 0f;
    }

    if (cooldown > 0f){
      cooldown -= Time.deltaTime;
      if (cooldown < 0f){
        cooldown = 0f;
      }
    }
    checkFloor();

    float distanceMoved = movement + (isDashing ? dashForce * faceDir : 0f);
    RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(0.5f, 0.5f), 0f, new Vector2(distanceMoved, 0f), 0.5f, solidMask.value);
    Debug.DrawRay(transform.position, new Vector2(0.5f * faceDir, 0f));
    
    if (hit){
      //Debug.Log(hit.point);
      distanceMoved = 0f;
    }

    rb.velocity = new Vector2(distanceMoved, rb.velocity.y);

    /*if (Mathf.Abs(rb.velocity.x) > movement)
    rb.velocity = new Vector2(movement, rb.velocity.y);
    */
  }

  void OnCollisionEnter2D(Collision2D col) {
    if (col.gameObject.CompareTag("projectile")){
      ManagerScript.coso.onPlayerDeath(this);
      Destroy(gameObject);
    }
  }

  public void flick(InputAction.CallbackContext context){
    if (cooldown > 0f){
        return;
    }
    // Shooting
    Vector2 flick = context.ReadValue<Vector2>();    
    //Vector2 flick = new Vector2(Input.GetAxisRaw("P" + id.ToString() +"FlickX"), -Input.GetAxisRaw("P" + id.ToString() + "FlickY"));

    if (isHolding){
      Vector2 forceBarPos = forceBar.anchoredPosition;
      forceBarPos.x = -1.5f + 1.5f * realFlick.magnitude/4.24f;
      forceBar.anchoredPosition = forceBarPos;

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

      // Esto dispararía el objeto
      if (flick.magnitude < 0.5f){
        anim.Play("Throw");
        audioSource.PlayOneShot(fireSound);
        Vector2 projectilePos = -realFlick.normalized;
        GameObject aProjectile = Instantiate(projectilePrefab, transform.position + new Vector3(projectilePos.x, projectilePos.y, 0f) * 1.5f, Quaternion.identity) as GameObject;
        Rigidbody2D projectileRb = aProjectile.GetComponent<Rigidbody2D>();
        projectileRb.AddForce(-realFlick * projectileSpeed);
        isHolding = false;

        cooldown = maxCooldown;
        forceBar.parent.gameObject.SetActive(false);
      }
    } else {
      if (flick.magnitude > 0.5f){        
        isHolding = true;
        realFlick = flick/2.0f;
        forceBar.parent.gameObject.SetActive(true);
      }
    }
  }

  public void move(InputAction.CallbackContext context){
    if (!isDashing){      
      movement = context.ReadValue<Vector2>().x * speed;
      if (Mathf.Abs(movement) > 0.5f){
        faceDir = Mathf.Sign(movement);
        anim.SetBool("standing", false);
      } else {
        movement = 0f;
        anim.SetBool("standing", true);   
      }
    }
        
  }

  public void jump(){    
    if (canJump && !isDashing){      
      anim.Play("Jump");        
      rb.velocity = new Vector2(rb.velocity.x, jumpForce);
      canJump = false;
      audioSource.PlayOneShot(jumpSound, 2f);      
    }
  }

  public void dash(){
    if (dashCooldown <= 0f){      
      anim.Play("Dash");

      audioSource.PlayOneShot(dashSound, 3f);
      
      GetComponent<Collider2D>().enabled = false;
      isDashing = true;
      Vector3 dashFlip = dashEffect.transform.localScale;
      dashFlip.y = faceDir * transform.localScale.x;
      dashEffect.transform.localScale = dashFlip;
      dashEffect.GetComponent<particleScript>().Emit();
      
      dashCooldown = maxDashCooldown;
    }
  }

  void checkFloor(){
    //RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, new Vector2(0f, -1f), 0.8f, solidMask.value);
    Collider2D col = Physics2D.OverlapBox(rb.transform.position + Vector3.down * 0.75f, new Vector2(0.5f, 0.2f), 0f, solidMask.value);
    if (col != null){
      if (rb.velocity.y <= 0f && !canJump){
        //partículas de cuando cae al piso        
        Instantiate(landingPartsPrefab, transform.position + Vector3.down * 0.75f, Quaternion.identity);        

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        canJump = true;
        var state = anim.GetCurrentAnimatorStateInfo(0); 
        if (state.IsName("Jump")){
          anim.Play("Idle");
        }
      }
      rb.gravityScale = 0f;
    } else {
      rb.gravityScale = 4f;
    }
  }  

  void OnDrawGizmos(){    
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(transform.position + Vector3.down * 0.75f, new Vector3(0.5f, 0.2f, 1f));    
  }
}
