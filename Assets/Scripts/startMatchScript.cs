using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startMatchScript : MonoBehaviour
{
  void Update(){
    if (Input.GetButtonDown("P1Start") || Input.GetButtonDown("P2Start")){
      ManagerScript.coso.goTo(ManagerScript.coso.battleScene);
    }
    if (Input.GetButtonDown("P1Dash") || Input.GetButtonDown("P2Dash")){
      ManagerScript.coso.goTo(ManagerScript.coso.titleScene);
    }
  }
}
