using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tiempoVidasButtonScript : ButtonScript{
  typeOfMatch tipoMatch = typeOfMatch.vidas;
  override protected void Start(){
    base.Start();
    actualizarTexto();
  }

  override protected void Update(){
    tipoMatch = ManagerScript.coso.matchType;
    base.Update();
    actualizarTexto();
  }
  override public void rightAction(){    
    switch (tipoMatch){
    case typeOfMatch.tiempo:
      ManagerScript.coso.addStartingTime(30f);
    break;
    case typeOfMatch.vidas:
      ManagerScript.coso.addStartingLives(1);
    break;
    }    
  }
  override public void leftAction(){
    switch (tipoMatch){
    case typeOfMatch.tiempo:
      ManagerScript.coso.addStartingTime(-30f);
    break;
    case typeOfMatch.vidas:
      ManagerScript.coso.addStartingLives(-1);
    break;
    }
  }

  void actualizarTexto(){
    switch (tipoMatch){
    case typeOfMatch.tiempo:
      var timespan = 
        System.TimeSpan.FromSeconds(ManagerScript.coso.startingTime);

      string theText = timespan.ToString(@"mm\:ss");
      buttonText.text = $"Duración: {theText}";
    break;
    case typeOfMatch.vidas:
      buttonText.text = $"Vidas: {ManagerScript.coso.startingLives}";
    break;
    }
    
  }
}
