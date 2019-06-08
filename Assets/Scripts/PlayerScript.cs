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
    bool canJump = false;
    public LayerMask solidMask;

    public GameObject projectilePrefab;
    public int team = 0;

    Vector2 previousFlick;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float movement = Input.GetAxis("P1Horizontal");
        if (Mathf.Abs(movement) > 0.5)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, new Vector2(movement, 0), 1f, solidMask.value);
            if (!hit)
            {
                Debug.Log("podemos movernos");
                rb.velocity = new Vector2(movement * speed, rb.velocity.y);
            }
            
        }

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);

        if (canJump)
        {
            if (Input.GetButton("P1Jump"))
            {                
                rb.AddForce(new Vector2(0f, jumpForce));
                canJump = false;
            }
        }

        Vector2 flick = new Vector2(Input.GetAxis("P1FlickX"), -Input.GetAxis("P1FlickY"));

        if (isHolding){
            if (flick.magnitude > 0.3f)
            {
                previousFlick = flick;
            }

            if (flick.magnitude < 0.1f)
            {
                GameObject aProjectile = Instantiate(projectilePrefab, transform.position + Vector3.right, Quaternion.identity) as GameObject;
                Rigidbody2D projectileRb = aProjectile.GetComponent<Rigidbody2D>();
                projectileRb.AddForce(-previousFlick * projectileSpeed);
                isHolding = false;
            }            
        } else {
            if (flick.magnitude > 0.1f){
                isHolding = true;
            }
        }
        
    }

    void OnCollisionEnter2D(Collision2D col){
        if (col.gameObject.CompareTag("piso")){
            canJump = true;
        }
    }
}
