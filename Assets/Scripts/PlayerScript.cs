using System.Collections;
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
  bool gravityCancelFlag = true;
  bool grounded = false;
  
  public int shotsToOverheat = 10;
  int currentShotsLeft;  
  public float flickSpeed = 10f;  
  float flickMagnitude = 0f;
  public float flickAngle = 0f;
  float cooldown = 0f;

  float flickRotationSensibility = 0.1f; // (0f, 1f];
    
  Vector2 movement;  
  float faceDir = 1f;
  bool canJump = false;
  bool isDashing = false;
  float dashTimer = 0f;
  float dashCooldownTimer = 0f;
  float dashBounceTimer = 0f;
  Vector2 dashDirection;
  int availableDashes;

  bool invincible = false;

[Header("Colisiones")]  
  public LayerMask solidMask;
  public LayerMask jumpingMask;

[Header("Objetos del Player")]
  public GameObject projectilePrefab;
  public GameObject fantasmitaPrefab;
  public GameObject arrowBar;
  SpriteRenderer arrowSprite;

  public GameObject playerShadow;
  Vector3 shadowStartPos;

  public GameObject playerGraphics;
  public Collider2D floorChecker;
  CircleCollider2D bodyCollider;
  Animator anim;
  SpriteRenderer spr;
  Vector2 startingScale;

[Header("Audio")]
  AudioSource audioSource;
  float currentAudioPoint;
  bool audioIsLocked = false;
  public AudioClip[] dashSounds;
  public AudioClip[] dashVoices;
  public AudioClip[] holdingSounds;
  public AudioClip[] fireSounds;
  public AudioClip[] jumpSounds;
  public AudioClip[] victorySounds;

[Header("Efectos")]
  public GameObject dashEffect;
  public GameObject landingPartsPrefab;
  public GameObject jumpingPartsPrefab;
  public GameObject collisionPartsPrefab;

  //TIMERS
  //Dictionary<string, CustomTimer> timers;

  CustomTimer invincibleTimer;
  CustomTimer reloadTimer;

  public bool cpu = false;
  
  // Start is called before the first frame update
  void Start(){

    cpu = ManagerScript.coso.cpus[team];

    currentAudioPoint = Random.Range(0f, 1f);
    availableDashes = maxDashes;

    startingScale = playerGraphics.transform.localScale;
    faceDir = playerGraphics.transform.localScale.x;

    currentGravity = baseGravity;
    currentShotsLeft = shotsToOverheat;
    
    rb = GetComponent<Rigidbody2D>();
    if (MatchManager.match != null){
      MatchManager.match.addPlayer(this);
    }

    bodyCollider = GetComponent<CircleCollider2D>();

    arrowSprite = arrowBar.GetComponent<SpriteRenderer>();
    anim = playerGraphics.GetComponent<Animator>();
    spr = playerGraphics.GetComponent<SpriteRenderer>();
    audioSource = GetComponent<AudioSource>();

    dashDirection = Vector2.right * (spr.flipX ? 1f : -1f);

    shadowStartPos = playerShadow.transform.localPosition;

    initTimers();
  }

  void initTimers(){
    //timers = new Dictionary<string, CustomTimer>();
    invincibleTimer = new CustomTimer(2f, () => invincible = false);
    reloadTimer = new CustomTimer(0f, () => currentShotsLeft += 1);
  }

  // Update is called once per frame
  void Update()
  {
    if (MatchManager.match != null && MatchManager.match.paused){
      return;
    }
    checkFloor();
    if (!cpu){
      dashControl();
    }
    if (!isDashing){
      if (!cpu){
        moveControl();
        jumpControl();
        shootControl();
      }
    } else {
      arrowBar.gameObject.SetActive(false);
      rotateWhenInDash();
    }

    if (cpu){
      GetComponent<AIScript>().addScoreToAI(-Time.deltaTime * (currentShotsLeft/shotsToOverheat) * 2.0f);
    }
    
    updateColor();
    updateShadow();
    tickTimers();
  }

  void updateColor(){
    if (invincible){
      //TODO: Ajustar para que quede lindo
      //Titileando. No me convence
      //spr.color = new Color(1f, 1f, 1f, (Mathf.Abs(Mathf.Sin(Time.time * 18f)) > 0.5f ? 0.8f : 0f));
      spr.color = new Color(1f, 1f, 1f, 0.2f);
    } else {
      float overheatingTime = reloadTimer.getCurrentTime() / reloadTimer.getDuration();
      if (currentShotsLeft < 1){
        spr.color = Color.Lerp(Color.white, new Color(1f, 0.4f, 0.26f, 1f), overheatingTime);
      } else {
        spr.color = new Color(1f, 1f, 1f, 1f);
      }
    }
  }

  void updateShadow(){    
    float shadowScale = 0f;
    
    var hit = Physics2D.Raycast(rb.transform.position, Vector2.down, 12f, solidMask);
    if (hit){
      float newY = hit.point.y;
      shadowScale = 1f - Mathf.Abs(rb.transform.position.y - hit.point.y) / 14f;
      var oldPos = playerShadow.transform.position;
      playerShadow.transform.position = new Vector2(oldPos.x, newY);
      float shadowAngle = new Angle360(Mathf.Atan2(hit.normal.y, hit.normal.x), true) - 90f;
      playerShadow.transform.rotation = Quaternion.Euler(0f, 0f, shadowAngle);
    }

    playerShadow.transform.localScale = Vector2.one * shadowScale;
  }

  void tickTimers(){
    //TestTimer.tick(TestTimer.getDuration);
    invincibleTimer.tick(()=>0f, false, invincible);
    //TODO: convertir en CustomTimers
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

    dashCooldownTimer -= Time.deltaTime;
    if (dashCooldownTimer <= 0f){
      availableDashes = Mathf.Min(availableDashes + 1, maxDashes);
      dashCooldownTimer = 0.5f;
    }

    reloadTimer.tick(
      ()=> 0.5f - 0.5f * ((float)currentShotsLeft / (float)shotsToOverheat), 
      (currentShotsLeft < shotsToOverheat), 
      (currentShotsLeft < shotsToOverheat)
    );

    
    if (isDashing) {
      dashTimer -= Time.deltaTime;
      if (dashTimer <= 0f){
        stopDashing();  
      }
    }

    if (cooldown > 0f){
      cooldown -= Time.deltaTime;
      if (cooldown < 0f){
        cooldown = 0f;
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
    
    // Actualizar el la velocidad del objeto dependiendo de su movimiento
    rb.velocity = movement;
  }

  void OnCollisionEnter2D(Collision2D col) {
    if (col.gameObject.CompareTag("projectile")){
      int projectileTeam = col.gameObject.GetComponent<ProjectileScript>().team;
      if (projectileTeam == team || invincible){
        return;
      }
      MatchManager.match.onPlayerDeath(this, projectileTeam);
      die();
    }
  }

  public void die(){
    MatchManager.match.setTimeScale(0.1f, 0.5f);
    CamScript.screen.shake(0.2f, 1f);
    Destroy(gameObject);
    GameObject fantasmita = Instantiate(fantasmitaPrefab, transform.position, Quaternion.identity);
    var scr = fantasmita.GetComponent<fantasmitaScript>();
    scr.team = team;
    scr.flip = (Mathf.Sign(playerGraphics.transform.localScale.x) > 0f) ? spr.flipX : !spr.flipX;
  }

  void playRandomSound(AudioClip[] audios, float volumeScale = 1f, bool oneShot = true, bool lockAudio = false){    
    if (audios.Length > 0){
      currentAudioPoint += 0.61803398874989f;
      currentAudioPoint = currentAudioPoint % 1.0f;
      int currentAudioIndex = (int)Mathf.Floor(currentAudioPoint * audios.Length);
      if (oneShot){
        audioSource.PlayOneShot(audios[currentAudioIndex], volumeScale);
      } else {
        if (!audioSource.isPlaying){
          audioIsLocked = false;
        }
        if (!audioIsLocked){
          audioSource.pitch = Random.Range(1f, 1.2f);
          audioSource.clip = audios[currentAudioIndex];
          audioSource.Play();
          audioIsLocked = lockAudio;
        }
      }
    }
    //audioSource.pitch = 1f;
  }
  public void changeGravity(float percentage, float duration){
    gravityTimer = duration;
    currentGravity = baseGravity * percentage;
  }

  public void changeSpeed(float percentage, float duration){
    speedTimer = duration;
    speed = maxSpeed * percentage;
  }

  void shootControl(){
    Vector2 flick = new Vector2(Input.GetAxis("P" + id.ToString() +"FlickX"), -Input.GetAxisRaw("P" + id.ToString() + "FlickY"));
    shootLogic(flick);
  }
  public void shootLogic(Vector2 flick){
    if (cooldown > 0f || isDashing){
      return;
    }
    // Shooting
    if (isHolding){
      if (!isDashing){
        arrowBar.gameObject.SetActive(true);
      }
      // Para que no pueda moverse si está apuntando
      if (!grounded){
        movement.x = 0f;
      }

      // TODO: SENSIBILIDAD FLICK
      if (flick.magnitude >= 0.5f){
        calculateShootAngleAndMagnitude(flick);
      }

      Vector3 flickVector = new Vector3(Mathf.Cos(flickAngle * Mathf.Deg2Rad), Mathf.Sin(flickAngle * Mathf.Deg2Rad), 0f);
      
      // Esta es la flechita (Mover a una función de dibujar flechita?)
      arrowBar.transform.rotation = Quaternion.Euler(0f, 0f, flickAngle);
      arrowBar.transform.position = transform.position + flickVector;
      Vector3 dummyScale = arrowBar.transform.localScale;
      dummyScale.x = -Mathf.Sign(transform.localScale.x);
      arrowBar.transform.localScale = dummyScale;
      arrowSprite.size = new Vector2(1.0f + 1.5f * flickMagnitude / 4f, 0.7f);

      // Esto dispararía el objeto
      if (flick.magnitude < 0.5f){
        shoot(flickAngle, flickMagnitude);
      }
    } else {
      if (flick.magnitude > 0.5f){
        startShooting(flick);
      }
    }
  }

  void calculateShootAngleAndMagnitude(Vector2 flick){
    //Calculamos el ángulo del flick basándonos en el ángulo previo
    float newAngle = Mathf.Atan2(-flick.y, -flick.x) * Mathf.Rad2Deg;
    if (newAngle < 0f){
      newAngle += 360f;
    }
    
    updateFlickAngle(newAngle, flickRotationSensibility);

    //Esto es para cambiar de lado el sprite dependiendo del flick
    Vector3 prevScale = playerGraphics.transform.localScale;
    prevScale.x = startingScale.x * Mathf.Sign(flick.x) * (spr.flipX ? -1f : 1f);
    playerGraphics.transform.localScale = prevScale;

    // y esto es para darle el poder al flick
    if (flick.magnitude > 0.85f){
      flickMagnitude += flick.magnitude * flickSpeed * Time.deltaTime;
    } else {
      flickMagnitude += (flick.magnitude - flickMagnitude) * 0.1f;
    }
    flickMagnitude = Mathf.Clamp(flickMagnitude, -4f, 4f);
  }

  void startShooting(Vector2 flick){
    if (!grounded && gravityCancelFlag){
      changeGravity(0.01f, 0.75f);
      changeSpeed(0f, 0.75f);
    }
    isHolding = true;
    flickMagnitude = flick.magnitude;
    flickAngle = Mathf.Atan2(-flick.y, -flick.x) * Mathf.Rad2Deg;
    if (flickAngle < 0f){
      flickAngle += 360f;
    }
    arrowBar.gameObject.SetActive(true);
    //playRandomSound(holdingSounds, 1f, false);
  }


  void shoot(float flickAngle, float magnitude){
    if (currentShotsLeft > 0){
      GetComponent<AIScript>().addScoreToAI(5f);
      Vector3 flickVector = new Vector3(Mathf.Cos(flickAngle * Mathf.Deg2Rad), Mathf.Sin(flickAngle * Mathf.Deg2Rad), 0f);

      currentShotsLeft -= 1;

      reloadTimer.start(0.5f);
      cooldown = maxCooldown;

      if (!grounded && gravityCancelFlag){
        changeGravity(0f, 0.15f);
        movement.y = 0f;
        changeSpeed(0f, 0.15f);
        if (currentShotsLeft <= 0){
          gravityCancelFlag = false;
        }
      }

      anim.Play("Throw");
      playRandomSound(fireSounds, 1f, false);

      GameObject aProjectile = Instantiate(projectilePrefab, transform.position + flickVector * 1.5f, Quaternion.identity) as GameObject;
      Rigidbody2D projectileRb = aProjectile.GetComponent<Rigidbody2D>();
      aProjectile.GetComponent<ProjectileScript>().team = team;

      Vector2 laFuerza = new Vector2(Mathf.Cos(flickAngle * Mathf.Deg2Rad), Mathf.Sin(flickAngle * Mathf.Deg2Rad)) * magnitude * projectileSpeed;
      laFuerza = laFuerza + laFuerza.normalized * 100f;

      projectileRb.AddForce(laFuerza);
    } else {
      GameObject jumpParts = Instantiate(jumpingPartsPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
      landingPartsScript jumpPartsScr = jumpParts.GetComponent<landingPartsScript>();
      jumpPartsScr.minSpeed = 5f;
      jumpPartsScr.startAngle = 0f;
      jumpPartsScr.maxAngle = 180f;
    }

    isHolding = false;
    arrowBar.gameObject.SetActive(false);
  }


  // TODO:
  // Esto sería un lerp angle. Refactorizar armando una clase angle nueva que sólo vaya entre 0 y 360
  void updateFlickAngle(float targetAngle, float rotSpeed = 0.1f){
    // targetAngle tiene que estar entre 0 y 360
    float shortest_angle = ((((targetAngle - flickAngle) % 360) + 540) % 360) - 180;
    flickAngle += (shortest_angle * rotSpeed) % 360;

    // normalizar
    if (flickAngle < 0f){
      flickAngle += 360f;
    } else if (flickAngle >= 360f){
      flickAngle -= 360f;
    }
  }

  void moveControl(){
    float inputX = Input.GetAxis("P" + id.ToString() + "Horizontal");
    move(inputX);
  }

  public void move(float inputX){
    if (isDashing){
      return;
    }
    if (Mathf.Abs(inputX) > 0.5f)
    {
      movement.x = inputX * speed;
      faceDir = Mathf.Sign(movement.x);
      // TODO: updatear lado al que mira si no está disparando
      /*
      if (!isHolding && AGREGAR_UN_TIMER_O_ALGO_DESPUES_DE_DISPARAR){
        Vector3 prevScale = playerGraphics.transform.localScale;
        prevScale.x = startingScale.x * faceDir * (spr.flipX ? 1f : -1f);
        playerGraphics.transform.localScale = prevScale;
      }
      */
      if (grounded){
        anim.SetBool("standing", false);
      }
    } else {            
      //movementX = movementX * friction;
      anim.SetBool("standing", true);
    }
  }

  void jumpControl(){
    if (Input.GetButton("P" + id.ToString() + "Jump")){
      jump();
    }
  }

  public void jump(){
    if (canJump){
      anim.Play("Jump");
      
      movement.y = jumpForce;
      
      GameObject jumpParts = Instantiate(jumpingPartsPrefab, transform.position + Vector3.down * 0.75f, Quaternion.identity);
      landingPartsScript jumpPartsScr = jumpParts.GetComponent<landingPartsScript>();
      jumpPartsScr.startAngle = 0f;
      jumpPartsScr.maxAngle = 180f;
      
      canJump = false;
      playRandomSound(jumpSounds, 1f, false);
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

  void dashControl(){
    if (Input.GetButtonDown("P" + id.ToString() + "Dash")) {
      Vector2 dashInputDirection = new Vector2(Input.GetAxis("P" + id.ToString() + "Horizontal"), -Input.GetAxis("P" + id.ToString() + "Vertical")).normalized;

      doADash(dashInputDirection);
    }
  }

  public void doADash(Vector2 dashInputDirection){
    dashInputDirection.Normalize();
    if (availableDashes > 0){
      if (dashInputDirection.magnitude > 0f){
        dashDirection = dashInputDirection;
      }
      dash();
      availableDashes -= 1;
    }
  }

  void dash(){
    isDashing = true;
    anim.Play("Dash");
    playRandomSound(dashSounds, 0.5f, true);
    playRandomSound(dashVoices, 1f, false);
    
    GetComponent<Collider2D>().enabled = false;
    dashEffect.GetComponent<particleScript>().Emit();

    GameObject jumpParts = Instantiate(jumpingPartsPrefab, transform.position, Quaternion.identity);
    landingPartsScript jumpPartsScr = jumpParts.GetComponent<landingPartsScript>();
    jumpPartsScr.startAngle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg + 90f;
    jumpPartsScr.maxAngle = 180f;

    dashTimer = dashLength;
    dashCooldownTimer = 0.5f;
    movement = dashDirection * dashSpeed;
  }

  public void victoryShout(){
    playRandomSound(victorySounds, 1f, false, true);
  }

  public void MakeInvincible(float duration){
    if (invincibleTimer != null){
      invincibleTimer.start(duration);
    }
    invincible = true;
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
    grounded = floorChecker.IsTouchingLayers(jumpingMask.value);
    if (grounded){
      if (movement.y <= 0f && !canJump){
        // Resetear salto
        canJump = true;        
        // partículas de cuando cae al piso        
        Instantiate(landingPartsPrefab, transform.position + Vector3.down * 0.75f, Quaternion.identity);        
      }
      // Resetear animación de salto
      var state = anim.GetCurrentAnimatorStateInfo(0); 
      if (state.IsName("Jump") || state.IsName("Falling")){
        anim.Play("Idle");
      }
      
      gravityCancelFlag = true;

      //rb.gravityScale = 0f;
      if (!isDashing){
        availableDashes = maxDashes;
      }
    } else {      
      //rb.gravityScale = 4f;
      if (!isDashing){
        var state = anim.GetCurrentAnimatorStateInfo(0);
        if (movement.y <= 0f){
          if (state.IsName("Idle") || state.IsName("Jump")){
            anim.Play("Falling");
          }
        }
        movement.y -= currentGravity;
      }
    }
  }
}
