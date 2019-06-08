using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 30f;
    bool canJump = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

    void OnCollisionEnter2D(Collision2D col){
        if (col.gameObject.CompareTag("piso")){
            canJump = true;
        }
    }
}
