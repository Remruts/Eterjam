using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class landingPartsScript : MonoBehaviour
{
    public float startAngle = 0f;
    public float maxAngle = 180f;
    
    public int minPartCount = 5;
    public int maxPartCount = 7;

    public float minSpeed = 20f;
    public float maxSpeed = 20f;
    // Start is called before the first frame update
    void Start()
    {
      var landingParts = GetComponent<ParticleSystem>();
      int partCount = Random.Range(minPartCount, maxPartCount);
      
      for (int i = 0; i < partCount; ++i){
        float angle = startAngle + i * (maxAngle / (partCount-1));
        angle += Random.Range(-2f, 2f);
        angle = angle * Mathf.Deg2Rad;
        float speed = Random.Range(minSpeed, maxSpeed);
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * speed;
        landingParts.Emit(emitParams, 1);
      }
    }
}
