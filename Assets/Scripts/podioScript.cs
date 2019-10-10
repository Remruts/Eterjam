using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class podioScript : MonoBehaviour
{   
  public GameObject gato_win;
  public GameObject gato_lose;
  public GameObject lagarto_win;
  public GameObject lagarto_lose;
  // Start is called before the first frame update
  void Start(){
    bool ganaElGato = (ManagerScript.coso.currentWinningTeam == 0);
    gato_win.SetActive(ganaElGato);
    lagarto_win.SetActive(!ganaElGato);
    gato_lose.SetActive(!ganaElGato);
    lagarto_lose.SetActive(ganaElGato);    
  }
}
