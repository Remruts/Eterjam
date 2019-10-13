//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchManager : MonoBehaviour
{
     typeOfMatch currentMatchType = typeOfMatch.vidas;
    float matchTime = 120f;
    List<PlayerScript> currentPlayers;
    [Range(0, 99)]
    public int[] playerLives;

    public GameObject[] playerPrefabs;

    public static MatchManager match;

    public GameObject pauseText;
    public GameObject timerText;

    bool matchEnded = false;
    public bool paused = false;

    float currentTimeScale = 1f;

    public Texture2D[] transBacks;

    public float timeScale = 1.0f;

    public string resultsScene = "endBattleScene";

    CustomTimer timeScaleTimer;
    CustomTimer gameTimer;
   
    // Start is called before the first frame update
    void Awake(){
      MatchManager.match = this;
      currentPlayers = new List<PlayerScript>();
      
      resetTimeScale();
      timeScaleTimer = new CustomTimer(0f, resetTimeScale);
    }

    void Start(){
      currentMatchType = ManagerScript.coso.matchType;
      matchTime = ManagerScript.coso.startingTime;

      resetLives();

      if (currentMatchType == typeOfMatch.tiempo){
        timerText.SetActive(true);
        gameTimer = new CustomTimer(matchTime, roundOver);
      } else {
        timerText.SetActive(false);
      }
    }

    void resetTimeScale(){
      currentTimeScale = timeScale;
    }

    void resetLives(){
      playerLives = new int[2];

      int maxLives = 5;
      switch (currentMatchType){
      case typeOfMatch.vidas:
        maxLives = ManagerScript.coso.startingLives;
      break;
      case typeOfMatch.tiempo:
        maxLives = 0;
      break;
      }

      playerLives[0] = maxLives;
      playerLives[1] = maxLives;
    }

    // Update is called once per frame
    void Update(){
      if (matchEnded){
        return;
      }
      
      if (!paused){
        Time.timeScale = currentTimeScale;
        timeScaleTimer.tick(()=>0f, false, !Mathf.Approximately(currentTimeScale, timeScale));

        if (currentMatchType == typeOfMatch.tiempo){
          gameTimer.tick(()=> 0f);

          float currentGameTime = gameTimer.getCurrentTime();
          var timespan = 
            System.TimeSpan.FromSeconds(currentGameTime);

          string theText = timespan.ToString(@"mm\:ss");
          
          if (currentGameTime < 6){
            var timeAnim = timerText.GetComponent<Animator>();
            if (!timeAnim.GetBool("LastSeconds")){
              float counterSpeed = 0.8f;
              gameTimer.setTickScale(counterSpeed);
              timeAnim.SetFloat("timeScale", counterSpeed);
              timeAnim.SetBool("LastSeconds", true);
            }
            theText = timespan.ToString("%s");
          }

          timerText.GetComponent<TMP_Text>().text = 
            theText;
          if (currentGameTime < 1){
            timerText.SetActive(false);
            gameTimer.setTickScale(1f);
          }
        }
      }

      if (Input.GetButtonDown("P1Start") || Input.GetButtonDown("P2Start")){
        pauseGame();
      }
    }


    public void onPlayerDeath(PlayerScript aDeadPlayer) {
      int deadPlayerTeam = aDeadPlayer.team;
      currentPlayers.Remove(aDeadPlayer);
      currentPlayers[0].victoryShout();

      if (matchEnded){
        return;
      }

      switch (currentMatchType){
      case typeOfMatch.vidas:
        playerLives[deadPlayerTeam] -= 1;
        if (playerLives[deadPlayerTeam] > 0){
          StartCoroutine(respawnPlayer(deadPlayerTeam, 0.6f));
        } else {
          roundOver(); 
        }
      break;
      case typeOfMatch.tiempo:
        playerLives[(deadPlayerTeam + 1) % 2] += 1;
        StartCoroutine(respawnPlayer(deadPlayerTeam, 0.6f));
      break;
      }
    }

    IEnumerator respawnPlayer(int player, float waitTime){
      yield return new WaitForSeconds(waitTime);
      // Posicion random
      Vector3 newPosition = new Vector3(Random.Range(-9f, 9f), Random.Range(-4f, 4f), 0f);
      //Hacer aparecer muñeco
      GameObject playerInstance = Instantiate(playerPrefabs[player], newPosition, Quaternion.identity);
      //Invencibilidad por 2 segundos
      playerInstance.GetComponent<PlayerScript>().MakeInvincible(2f);
    }

    public void pauseGame(){
      // pause the game TODO
      paused = !paused;
      if (paused){
        pauseText.SetActive(true);
        Time.timeScale = 0f;
        pauseText.GetComponent<Animator>().Play("show");            
      } else {
        //pauseText.SetActive(false);
        Time.timeScale = currentTimeScale;
        pauseText.GetComponent<Animator>().Play("hide");
      }
    }


    void roundOver(){
      int aWinningTeam = (playerLives[0] > playerLives[1] ? 0 : 1);
      if (matchEnded){
          return;
      }

      if (ManagerScript.coso != null){
        ManagerScript.coso.currentWinningTeam = aWinningTeam;
      }
      matchEnded = true;

      setTimeScale(0.1f, 5f);
      Invoke(nameof(endMatch), 0.1f);
    }

    void endMatch(){
      ManagerScript.coso.saveResults(playerLives[0], playerLives[1]);
      ManagerScript.coso.endMatch();
    }

    public void setTimeScale(float newTimeScale, float duration){
      currentTimeScale = newTimeScale;
      timeScaleTimer.start(duration * newTimeScale);
      Time.timeScale = currentTimeScale;
    }

    public void addPlayer(PlayerScript p){
      currentPlayers.Add(p);
    }

    public List<GameObject> getPlayers(){
      List<GameObject> retList = new List<GameObject>();
      foreach(var player in currentPlayers){
        if (player != null){
          retList.Add(player.gameObject);
        }
      }
      return retList;
    }

}
