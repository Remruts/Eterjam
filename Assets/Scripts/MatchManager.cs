//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchManager : MonoBehaviour
{
     typeOfMatch currentMatchType = typeOfMatch.vidas;
    float matchTime = 120f;
    Dictionary<int, PlayerScript> currentPlayers;
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
    CustomTimer respawnTimer;

    geneticLottery[] AIStrategies;

    List<int> deadPlayers;
   
    // Start is called before the first frame update
    void Awake(){
      MatchManager.match = this;
      currentPlayers = new Dictionary<int, PlayerScript>();

      startAI();
      resetTimeScale();
      timeScaleTimer = new CustomTimer(0f, resetTimeScale);
      respawnTimer = new CustomTimer(0f, respawnPlayer);

      deadPlayers = new List<int>();
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

    public void startAI(){      
      AIStrategies = new geneticLottery[2];
      AIStrategies[0] = new geneticLottery();
      AIStrategies[1] = new geneticLottery();

      for (int i = 0; i < AIStrategies.Length; i++){
        if (ManagerScript.coso.cpus[i]){
          string jsonSavePath = $"Assets/AIModels/ai{i}.json";
          AIStrategies[i].loadStrategies(jsonSavePath);
        }
      }
    }

    public void saveAI(){
      Debug.Log("Saving AI...");
      for (int i = 0; i < AIStrategies.Length; i++){
        if (ManagerScript.coso.cpus[i]){
          Debug.Log($"Saving player {i}'s AI");
          string jsonSavePath = $"Assets/AIModels/ai{i}.json";
          AIStrategies[i].saveStrategies(jsonSavePath);
        }
      }
      Debug.Log("Saved AI!");
    }

    public ref geneticLottery getStrategies(int team){
      return ref AIStrategies[team];
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
        if (deadPlayers.Count > 0){
          respawnTimer.tick(()=>0.6f);
        }
      }

      if (Input.GetButtonDown("P1Start") || Input.GetButtonDown("P2Start")){
        pauseGame();
      }
    }


    public void onPlayerDeath(PlayerScript aDeadPlayer, int killerPlayerTeam) {
      int deadPlayerTeam = aDeadPlayer.team;
      if (currentPlayers.Count > 0){
        if (aDeadPlayer.cpu){
          var theOtherCPU = aDeadPlayer.GetComponent<AIScript>();
          theOtherCPU.addScoreToAI(-4f);
          theOtherCPU.mutate();
          theOtherCPU.plan();
        }

        currentPlayers.Remove(deadPlayerTeam);
        if (currentPlayers.ContainsKey(killerPlayerTeam)){
          currentPlayers[killerPlayerTeam].victoryShout();

          if (currentPlayers[killerPlayerTeam].cpu)
          {
            var theCPU = currentPlayers[killerPlayerTeam].GetComponent<AIScript>();
            theCPU.addScoreToAI(20f);
            theCPU.giveMoreTime();
          }
        }
      }

      if (matchEnded){
        return;
      }

      switch (currentMatchType){
      case typeOfMatch.vidas:
        playerLives[deadPlayerTeam] -= 1;
        if (playerLives[deadPlayerTeam] > 0){
          addDeadPlayer(deadPlayerTeam);
        } else {
          roundOver(); 
        }
      break;
      case typeOfMatch.tiempo:
        playerLives[killerPlayerTeam] += 1;
        addDeadPlayer(deadPlayerTeam);
      break;
      }
    }

    void addDeadPlayer(int deadPlayerTeam){
      foreach(int p in deadPlayers){
        if (p == deadPlayerTeam){
          return;
        }
      }
      deadPlayers.Add(deadPlayerTeam);
      respawnTimer.start(0.6f);
    }

    void respawnPlayer(){
      if (deadPlayers.Count == 0){
        return;
      }
      int player = deadPlayers[0];
      deadPlayers.RemoveAt(0);

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
      currentPlayers[p.team] = p;
    }

    public List<GameObject> getPlayers(){
      List<GameObject> retList = new List<GameObject>();
      foreach(var player in currentPlayers.Values){
        if (player != null){
          retList.Add(player.gameObject);
        }
      }
      return retList;
    }

}
