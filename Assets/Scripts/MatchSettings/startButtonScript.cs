using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startButtonScript : ButtonScript{
  override protected void Start(){
    base.Start();
  }

  override protected void Update(){
      base.Update();
  }

  override public void pressAction(){
    ManagerScript.coso.goTo(ManagerScript.coso.battleScene);
  }
}
