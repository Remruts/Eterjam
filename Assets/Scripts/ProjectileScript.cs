using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public Rigidbody2D theBody;
    float initialTime;
    float lifeSpam;

    // Start is called before the first frame update
    void Start()
    {
        initialTime = Time.time;
        lifeSpam = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - initialTime > lifeSpam)
        {
            Destroy(gameObject);
        }
    }
}
