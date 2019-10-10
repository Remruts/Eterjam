using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ManagerScript : MonoBehaviour
{
    public int startingLives = 5;
    public static ManagerScript coso;

    public Texture2D[] transBacks;

    public int currentWinningTeam = 0;

    int[] results;

    public string resultsScene = "endBattleScene";
    public string battleScene = "SpritesScene";
    public string titleScene = "TitleScene";

    CustomTimer timeScaleTimer;
   
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
      transitionScript.transition.setTransition(scene, "noise0");
      transitionScript.transition.startTransition(0.5f);
      transitionScript.transition.setTransitionTexture(transBacks[currentWinningTeam]);
    }

    void resetTimeScale(){
      Time.timeScale = 1f;
    }

}
