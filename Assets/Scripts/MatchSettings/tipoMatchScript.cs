using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tipoMatchScript : ButtonScript{

  override protected void Start(){
    base.Start();
    actualizarTexto();
  }

  override protected void Update(){
    base.Update();
  }
  override public void rightAction(){
    ManagerScript.coso.matchType = (ManagerScript.coso.matchType == typeOfMatch.tiempo ? typeOfMatch.vidas : typeOfMatch.tiempo);
    actualizarTexto();
  }
  override public void leftAction(){
    ManagerScript.coso.matchType = (ManagerScript.coso.matchType == typeOfMatch.tiempo ? typeOfMatch.vidas : typeOfMatch.tiempo);
    actualizarTexto();
  }

  void actualizarTexto(){
    buttonText.text = $"Tipo: {ManagerScript.coso.matchType.ToString()}";
  }
}
