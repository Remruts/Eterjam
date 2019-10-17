using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goToButtonPause : ButtonScript{
  public string scene = "SpritesScene";
  override protected void Start(){
    base.Start();
  }

  override protected void Update(){
    base.Update();
  }

  override public void pressAction(){
    MatchManager.match.setTimeScale(0.25f, 2f);
    ManagerScript.coso.goTo(scene);
  }
}
