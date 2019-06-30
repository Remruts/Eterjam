using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
  public static CamScript screen;  

  bool shaking = false;
  float intensity = 0f;
  float shakeIncrement = 1f;

  Vector3 currentPos;
  Vector3 shakenPos;
  public Transform target;
  public Vector3 offset;

  void Awake(){
    screen = this;
    if (target != null){
        transform.position = target.position;
    }
    currentPos = transform.position;
  }

  // Update is called once per frame
  void LateUpdate(){
    if (shaking){
      intensity += shakeIncrement * Time.deltaTime;
    }

    if (!shaking && intensity > 0){
      intensity -= shakeIncrement * Time.deltaTime;

      if (intensity < 0){
        intensity = 0;
      }
    }

    if (target != null){
      currentPos += (target.position - currentPos) * Time.deltaTime * 10f;
    }

    currentPos.z = -10f;

    if (intensity > 0){
      shakenPos.x = currentPos.x + getShake();
      shakenPos.y = currentPos.y + getShake();
      shakenPos.z = currentPos.z;

      transform.position = shakenPos + offset;
    } else {
      transform.position = currentPos + offset;
    }
  }

  public void jumpTo(Vector3 pos){
    currentPos = pos;
    currentPos.z = -10f;
    transform.position = currentPos + offset;
  }
  float getShake(){
    float shakingNumber = Random.Range(-intensity / 2f, intensity / 2f);
    shakingNumber += (Mathf.Sign(shakingNumber) * intensity / 2f);

    return shakingNumber;
  }

  public void shake(float time, float factor){
    shaking = true;

    if (time > 0){
      Invoke("stopShaking", time);
    }

    shakeIncrement = factor;
  }

  public void stopShaking(){
    shaking = false;
  }

}
