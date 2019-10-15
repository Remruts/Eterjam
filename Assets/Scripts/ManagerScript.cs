using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum typeOfMatch{
  vidas = 0,
  tiempo = 1
}

public class ManagerScript : MonoBehaviour
{
  public int startingLives = 3;
  public float startingTime = 120f;
  public float maxStartingTime = 900f;
  public float minStartingTime = 30f;

  public int minStartingLives = 1;
  public int maxStartingLives = 99;
  
  public typeOfMatch matchType = typeOfMatch.vidas;
  public static ManagerScript coso;

  public Texture2D[] transBacks;

  public int currentWinningTeam = 0;

  int[] results;

  public string resultsScene = "endBattleScene";
  public string matchSettingsScene = "matchSettingsScene";
  public string battleScene = "SpritesScene";
  public string titleScene = "TitleScene";

  [Range(0f, 1f)]
  public float musicVolume = 1.0f;
  [Range(0f, 1f)]
  public float audioVolume = 1.0f;

  CustomTimer timeScaleTimer;

  public bool[] cpus;
  
  // Start is called before the first frame update
  void Awake(){
    // TODO: eliminar al manager de la escena
    if (ManagerScript.coso == null){
      DontDestroyOnLoad(gameObject);
      ManagerScript.coso = this;
    } else{
      Destroy(gameObject);
    }
  }

  public void saveResults(int team0, int team1){
    results = new int[2];
    results[0] = team0;
    results[1] = team1;
  }

  // Update is called once per frame
  void Update(){
    if (SceneManager.GetActiveScene().name == resultsScene){
      if(Input.GetButtonDown("P1Jump") || Input.GetButtonDown("P2Jump") || Input.GetButtonDown("P1Start") || Input.GetButtonDown("P2Start")) {
        goTo(titleScene);
      }
    }
    if (Input.GetKey("escape")){
      Application.Quit();
    }
  }

  public void endMatch(){
    Invoke(nameof(resetTimeScale), 0.1f);
    goTo(resultsScene);
  }

  public void goTo(string scene){
    audioManager.audioman.PlayMusicOfScene(scene, 0.5f);
    transitionScript.transition.setTransition(scene, "noise0");
    transitionScript.transition.startTransition(0.5f);
    transitionScript.transition.setTransitionTexture(transBacks[currentWinningTeam]);
  }

  void resetTimeScale(){
    Time.timeScale = 1f;
  }

  public void addStartingTime(float t){
    startingTime += t;
    if (startingTime > maxStartingTime){
      startingTime = minStartingTime;
    } else if (startingTime < minStartingTime){
      startingTime = maxStartingTime;
    }
  }

  public void addStartingLives(int l){
    startingLives += l;
    if (startingLives > maxStartingLives){
      startingLives = minStartingLives;
    } else if (startingLives < minStartingLives){
      startingLives = maxStartingLives;
    }
  }

}
