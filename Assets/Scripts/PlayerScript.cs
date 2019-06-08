using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Rigidbody2D rb;
<<<<<<< Updated upstream
    public float speed = 30f;
=======
    Animator animator;
    bool isHolding = false;

    public float speed = 5f;
    public float maxSpeed = 5f;
    public float projectileSpeed = 300f;
    public float jumpForce = 300f;

    public float maxCooldown = 1f;

>>>>>>> Stashed changes
    bool canJump = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float movement = Input.GetAxis("P1Horizontal");
        if (Mathf.Abs(movement) > 0.5)
        {
            rb.MovePosition(rb.transform.position + new Vector3(movement * speed * Time.deltaTime, 0f, 0f));
        }

        if (canJump)
        {
            if (Input.GetButton("P1Jump"))
            {
                rb.AddForce(Vector3.up * speed);
            }
        }
    }

<<<<<<< Updated upstream
    void OnCollisionEnter2D(Collision2D col){
        if (col.gameObject.CompareTag("piso")){
            canJump = true;
=======
    void move()
    {
        float movement = Input.GetAxis("P" + id.ToString() + "Horizontal");
        if (Mathf.Abs(movement) > 0.5)
        {
            animator.SetBool("standing", false);
            RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, new Vector2(movement, 0), 1f, solidMask.value);
            if (!hit)
            {                
                rb.velocity = new Vector2(movement * speed, rb.velocity.y);
            }

        } else {
            animator.SetBool("standing", true);
        }
    }

    void jump()
    {
        if (canJump)
        {
            if (Input.GetButton("P" + id.ToString() + "Jump"))
            {
                rb.AddForce(new Vector2(0f, jumpForce));
                canJump = false;
            }
>>>>>>> Stashed changes
        }
    }
}
