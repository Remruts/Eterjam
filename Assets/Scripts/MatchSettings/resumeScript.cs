using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resumeScript : ButtonScript{  
  override protected void Start(){
    base.Start();
  }

  override protected void Update(){
    base.Update();
  }

  override public void pressAction(){
    MatchManager.match.pauseGame();
  }
}
