using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showQuestion : ButtonScript{
  public string scene = "SpritesScene";
  public bool reset = false;
  public string question = "¿volver al menú?";
  public GameObject pauseSettings;
  override protected void Start(){
    base.Start();
  }

  override protected void Update(){
    base.Update();
  }

  override public void pressAction(){
    if (reset){
      scene = ManagerScript.coso.battleScene;
    }
    pauseSettings.GetComponent<pauseSettingsScript>().showQuestion(question, scene);
  }
}
