using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureScript : MonoBehaviour
{   
    public GameObject smokePrefab;
    void OnCollisionEnter2D(Collision2D col)
    {
        float probability = Random.Range(0.0f,1.0f);
        if (col.gameObject.CompareTag("projectile") && probability > 0.9f)
        {
            //Debug.Log("stuff");
            Destroy(gameObject);
            Instantiate(smokePrefab, transform.position, Quaternion.identity);
            CamScript.screen.shake(0.1f, 0.8f);
        }
    }
}
