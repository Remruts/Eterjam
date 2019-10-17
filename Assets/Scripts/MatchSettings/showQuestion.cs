using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showQuestion : ButtonScript{
  public string scene = "SpritesScene";
  public string question = "¿volver al menú?";
  public GameObject pauseSettings;
  override protected void Start(){
    base.Start();
  }

  override protected void Update(){
    base.Update();
  }

  override public void pressAction(){
    pauseSettings.GetComponent<pauseSettingsScript>().showQuestion(question, scene);
  }
}
