using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlPairScript : MonoBehaviour{
  public GameObject cat;

  public void shoot(){
    cat.GetComponent<controlShowScript>().shootProjectile();
  }
  public void dash(){
    cat.GetComponent<controlShowScript>().dash();
  }
}
