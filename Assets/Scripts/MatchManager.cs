using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MatchManager : MonoBehaviour
{
    //global variables

    // player array
    
    public List<PlayerScript> currentPlayers;
    public int[] playerLives;

    public GameObject[] playerPrefabs;

    public static MatchManager match;

    public GameObject pauseText;

    bool matchEnded = false;
    public bool paused = false;

    float currentTimeScale = 1f;

    public Texture2D[] transBacks;

    public float timeScale = 1.0f;

    public string resultsScene = "endBattleScene";

    CustomTimer timeScaleTimer;
   
    // Start is called before the first frame update
    void Awake(){
      MatchManager.match = this;
      currentPlayers = new List<PlayerScript>();
      
      resetTimeScale();
      timeScaleTimer = new CustomTimer(0f, resetTimeScale);
    }

    void Start(){
      resetLives();
    }

    void resetTimeScale(){
      currentTimeScale = timeScale;
    }

    void resetLives(){
      playerLives = new int[2];
      if (ManagerScript.coso != null){
        playerLives[0] = ManagerScript.coso.startingLives;
        playerLives[1] = ManagerScript.coso.startingLives;
      } else {
        Debug.LogWarning("No existe el manager!");
        playerLives[0] = 5;
        playerLives[1] = 5;
      }
      
    }

    // Update is called once per frame
    void Update()
    {
      // Para debug. Cambiar
      if (!paused){
        Time.timeScale = currentTimeScale;
        timeScaleTimer.tick(()=>0f, false, !Mathf.Approximately(currentTimeScale, timeScale));
      }
      
      if (matchEnded){
        return;
      }

      if (Input.GetButtonDown("P1Start") || Input.GetButtonDown("P2Start")){
        pauseGame();
      }
    }


    public void onPlayerDeath(PlayerScript aDeadPlayer) {
      int deadPlayerTeam = aDeadPlayer.team;
      currentPlayers.Remove(aDeadPlayer);
      currentPlayers[0].victoryShout();
      playerLives[deadPlayerTeam] -= 1;
      if (playerLives[deadPlayerTeam] > 0){
        StartCoroutine(respawnPlayer(deadPlayerTeam, 0.6f));
      } else {
        roundOver((deadPlayerTeam + 1) % 2); 
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

    public void pauseGame()
    {
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


    void roundOver(int aWinningTeam){
      if (matchEnded){
          return;
      }

      if (ManagerScript.coso != null){
        ManagerScript.coso.currentWinningTeam = aWinningTeam;
      }
      matchEnded = true;

      setTimeScale(0.1f, 5f);
      Invoke("terminarJuego", 0.1f);
    }

    void terminarJuego(){
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

}
