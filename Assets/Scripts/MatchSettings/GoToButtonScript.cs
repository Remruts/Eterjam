using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToButtonScript : ButtonScript{
  public string scene = "SpritesScene";
  override protected void Start(){
    base.Start();
  }

  override protected void Update(){
      base.Update();
  }

  override public void pressAction(){
    ManagerScript.coso.goTo(scene);
  }
}
