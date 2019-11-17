using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seleccionNivelScript : ButtonScript{
  public string[] niveles = {"SpritesScene", "Outside"};
  int index = 0;
  string nivelActual;
  override protected void Start(){
    base.Start();
    nivelActual = ManagerScript.coso.battleScene;
    for (int i=0; i < niveles.Length; i++){
        if (niveles[i] == nivelActual){
            index = i;
            break;
        }
    }
    actualizarTexto();
  }

  override protected void Update(){
    base.Update();
    actualizarTexto();
  }
  override public void rightAction(){ 
    index++;
    if (index >= niveles.Length){
      index = 0;
    }    
    updateManager();
  }
  override public void leftAction(){
    index--;
    if (index < 0){
      index = niveles.Length-1;
    }
    updateManager();
  }

  void updateManager(){
    nivelActual = niveles[index];
    ManagerScript.coso.battleScene = nivelActual;
  }

  void actualizarTexto(){
    // pero desde el manager
    string elNivel = "Oficina";
    if (nivelActual == "SpritesScene"){
        elNivel = "Oficina";
    } else if (nivelActual == "Outside"){
        elNivel = "Afuera";
    } else {
        elNivel = nivelActual;
    }
    buttonText.text = $"Nivel: {elNivel}";
  }
}
