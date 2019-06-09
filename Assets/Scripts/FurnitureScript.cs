using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        float probability = Random.Range(0.0f,1.0f);
        if (col.gameObject.CompareTag("projectile") & probability > 0.9f)
        {
            Debug.Log("stuff");
            Destroy(gameObject);
        }
    }
}
