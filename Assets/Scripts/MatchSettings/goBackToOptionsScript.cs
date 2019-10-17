using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goBackToOptionsScript : ButtonScript{
  public GameObject pauseSettings;
  override protected void Start(){
    base.Start();
  }

  override protected void Update(){
    base.Update();
  }

  override public void pressAction(){
    pauseSettings.GetComponent<pauseSettingsScript>().resetOptions();
  }
}
