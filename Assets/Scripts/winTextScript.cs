using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class winTextScript : MonoBehaviour
{
  public Color32 team0TextColor = new Color32(255, 176, 255, 255);
  public Color32 team0OutlineColor = new Color32(255, 117, 117, 255);

  public Color32 team1TextColor = new Color32(88, 255, 119, 255);
  public Color32 team1OutlineColor = new Color32(126, 161, 121, 255);
  // Start is called before the first frame update
  void Start(){
    int winningTeam = 0;
    if (ManagerScript.coso != null){
      winningTeam = ManagerScript.coso.currentWinningTeam;
    }
    string winner = winningTeam == 0 ? "de Desarrollo" : "de Comercial";

    var elTexto = GetComponent<TMP_Text>();
    if (winningTeam == 0){
      elTexto.color = team0TextColor;
      elTexto.outlineColor = team0OutlineColor;
    } else {
      elTexto.color = team1TextColor;
      elTexto.outlineColor = team1OutlineColor;
    }

    elTexto.text = "Ganó el equipo " + winner;
  }

  // Update is called once per frame
  void Update(){
      
  }
}
