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
  public string attractModeScene = "SpritesScene";

  [Range(0f, 1f)]
  public float musicVolume = 1.0f;
  [Range(0f, 1f)]
  public float audioVolume = 1.0f;

  CustomTimer attractModeTimer;
  public float timeToAttractMode = 5f;
  public float timeInAttractMode = 30f;

  public bool[] cpus;

  public bool attractMode = false;
  
  // Start is called before the first frame update
  void Awake(){
    // TODO: eliminar al manager de la escena
    if (ManagerScript.coso == null){
      DontDestroyOnLoad(gameObject);
      ManagerScript.coso = this;
      attractModeTimer = new CustomTimer(
        5f, 
        ()=>{
          string currentScene = SceneManager.GetActiveScene().name;
          if (currentScene == titleScene){
            goTo(attractModeScene); 
            attractMode = true;
            cpus[0] = true;
            cpus[1] = true;
            attractModeTimer.setDuration(timeInAttractMode);
          } else if (currentScene == attractModeScene && attractMode){
            goTo(titleScene);
          }
        }
      );
    } else{
      string currentScene = SceneManager.GetActiveScene().name;
      if (currentScene == coso.titleScene){
        coso.attractMode = false;
        coso.cpus[0] = false;
        coso.cpus[1] = false;
        coso.attractModeTimer.start(timeToAttractMode);
      }
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
    string currentScene = SceneManager.GetActiveScene().name;
    
    if (currentScene == resultsScene || (attractMode && currentScene == attractModeScene)){
      if(Input.GetButtonDown("P1Jump") || Input.GetButtonDown("P2Jump") || Input.GetButtonDown("P1Start") || Input.GetButtonDown("P2Start")) {
        goTo(titleScene);
      }
    }
    
    if (currentScene == titleScene){
      attractModeTimer.tick(()=> timeInAttractMode);
    } else if (currentScene == attractModeScene){
      attractModeTimer.tick(()=> timeToAttractMode);
    }
    
    if (Input.GetKey("escape")){
      Application.Quit();
    }
  }

  public void endMatch(){
    Invoke(nameof(resetTimeScale), 0.1f);
    if (!attractMode){
      goTo(resultsScene);
    } else {
      goTo(titleScene);
    }
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
