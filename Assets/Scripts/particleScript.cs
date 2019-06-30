using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleScript : MonoBehaviour
{  
  ParticleSystem parts;
  void Start(){
    parts = GetComponent<ParticleSystem>();
  }
  public void Emit(){
    parts.Emit(20);
  }
}
