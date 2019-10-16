using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class numeroJugadoresScript : ButtonScript{
  int jugadores = 2;
  override protected void Start(){
    base.Start();
    if (ManagerScript.coso.cpus[0]){
      jugadores = 0;
    } else if (ManagerScript.coso.cpus[1]){
      jugadores = 1;
    } else {
      jugadores = 2;
    }
    actualizarTexto();
  }

  override protected void Update(){
    base.Update();
    actualizarTexto();
  }
  override public void rightAction(){ 
    jugadores++;
    if (jugadores > 2){
      jugadores = 0;
    }    
    updateManager();
  }
  override public void leftAction(){
    jugadores--;
    if (jugadores < 0){
      jugadores = 2;
    }
    updateManager();
  }

  void updateManager(){
    if (jugadores > 0){
      ManagerScript.coso.cpus[0] = false;
    } else {
      ManagerScript.coso.cpus[0] = true;
    }
    if (jugadores > 1){
      ManagerScript.coso.cpus[1] = false;
    } else {
      ManagerScript.coso.cpus[1] = true;
    }
  }

  void actualizarTexto(){
    // pero desde el manager
    buttonText.text = $"Jugadores: {jugadores}";
  }
}
