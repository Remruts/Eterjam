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

	Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
				if (ManagerScript.coso != null){
					ManagerScript.coso.addPlayer(this);
				}
				anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
				if (!isDashing){
					move();
	        jump();
					if (cooldown == 0f) {
	            shoot();
	        } else if (cooldown > 0f)
	        {
	            cooldown -= Time.deltaTime;
	            if (cooldown < 0f)
	            {
	                cooldown = 0f;
	            }
	        }
				}

				dash();

				rb.velocity = new Vector2(movement * speed + (isDashing ? dashForce * Mathf.Sign(movement) : 0f), rb.velocity.y);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed + (isDashing ? dashForce : 0f))
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed + (isDashing ? dashForce * Mathf.Sign(transform.localScale.x)  : 0f), rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("piso")) {
						var state = anim.GetCurrentAnimatorStateInfo(0);
						if (state.IsName("Jump")){
							anim.Play("Idle");
						}
            canJump = true;
        } else if (col.gameObject.CompareTag("projectile")){
            Debug.Log("stuff");
            ManagerScript.coso.onPlayerDeath(this);
            Destroy(gameObject);
        }
    }

    void shoot()
    {
        // Shooting
        Vector2 flick = new Vector2(Input.GetAxisRaw("P" + id.ToString() +"FlickX"), -Input.GetAxisRaw("P" + id.ToString() + "FlickY"));

        if (isHolding)
        {

						Vector2 forceBarPos = forceBar.anchoredPosition;
						forceBarPos.x = -1.5f + 1.5f * realFlick.magnitude/4.24f;
						forceBar.anchoredPosition = forceBarPos;

            if (flick.magnitude > 0.1f)
            {
								//Esto es para cambiar de lado el sprite dependiendo del flick
								Vector3 prevScale = transform.localScale;
								prevScale.x = -Mathf.Sign(-flick.x);
								transform.localScale = prevScale;

								// y esto es para darle el poder al flick
                realFlick += flick * flickSpeed * Time.deltaTime;
                realFlick.x = Mathf.Clamp(realFlick.x, -3f, 3f);
                realFlick.y = Mathf.Clamp(realFlick.y, -3f, 3f);
            }

            // Esto dispararía el objeto
            if (flick.magnitude < 0.5f)
            {
                Vector2 projectilePos = -realFlick.normalized;
                GameObject aProjectile = Instantiate(projectilePrefab, transform.position + new Vector3(projectilePos.x, projectilePos.y, 0f) * 1.2f, Quaternion.identity) as GameObject;
                Rigidbody2D projectileRb = aProjectile.GetComponent<Rigidbody2D>();
                projectileRb.AddForce(-realFlick * projectileSpeed);
                isHolding = false;

                cooldown = maxCooldown;
								forceBar.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            if (flick.magnitude > 0.5f)
            {
                isHolding = true;
                realFlick = flick;
								forceBar.parent.gameObject.SetActive(true);
            }
        }
    }

    void move()
    {
        movement = Input.GetAxis("P" + id.ToString() + "Horizontal");
        if (Mathf.Abs(movement) > 0.5)
        {
						anim.SetBool("standing", false);
            RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, new Vector2(movement, 0), 0.5f, solidMask.value);
            if (hit)
            {
                movement = 0f;
            }
        } else {
					anim.SetBool("standing", true);
				}
    }

    void jump()
    {
        if (canJump)
        {
            if (Input.GetButton("P" + id.ToString() + "Jump"))
            {
								anim.Play("Jump");
                rb.AddForce(new Vector2(0f, jumpForce));
                canJump = false;
            }
        }
    }

		void dash(){
			dashCooldown -= Time.deltaTime;

			if (dashCooldown <= maxDashCooldown - 0.1f){
					isDashing = false;
			}
			if (dashCooldown <= 0f){
				dashCooldown = 0f;
			}

			if (dashCooldown <= 0f){
				if (Input.GetButton("P" + id.ToString() + "Dash")){
					
					isDashing = true;
					dashCooldown = maxDashCooldown;
				}
			}
		}
}
