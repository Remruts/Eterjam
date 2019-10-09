using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ManagerScript : MonoBehaviour
{
    //global variables

    // player array
    public List<GameObject> furniturePrefab;

    public List<PlayerScript> currentPlayers;
    public int[] playerLives;
    public int startingLives = 5;

    public GameObject[] playerPrefabs;

    int bodyCount;

    public static ManagerScript coso;

    public GameObject winText;
    public GameObject pauseText;

    bool matchEnded = false;
    public bool paused = false;

    float currentTimeScale = 1f;

    public Color32 team0TextColor = new Color32(255, 176, 255, 255);
    public Color32 team0OutlineColor = new Color32(255, 117, 117, 255);

    public Color32 team1TextColor = new Color32(88, 255, 119, 255);
    public Color32 team1OutlineColor = new Color32(126, 161, 121, 255);

    public Texture2D[] transBacks;

    public float timeScale = 1.0f;

    public int currentWinningTeam = 0;

    public string resultsScene = "endBattleScene";

    CustomTimer timeScaleTimer;
   
    // Start is called before the first frame update
    void Awake(){
      // TODO: eliminar al manager de la escena
      if (ManagerScript.coso == null){
        DontDestroyOnLoad(gameObject);
        ManagerScript.coso = this;
        currentPlayers = new List<PlayerScript>();
      } else{
        Destroy(gameObject);
      }

      resetTimeScale();
      timeScaleTimer = new CustomTimer(0f, resetTimeScale);

      resetLives();
    }

    void resetTimeScale(){
      currentTimeScale = timeScale;
    }

    void resetLives(){
      playerLives = new int[2];
      playerLives[0] = startingLives;
      playerLives[1] = startingLives;
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
      if (Input.GetKey("escape")){
        Application.Quit();
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
      string winner = aWinningTeam == 0 ? "de Desarrollo" : "de Comercial";
      if (matchEnded){
          return;
      }

      currentWinningTeam = aWinningTeam;
      matchEnded = true;
      /*
      winText.SetActive(true);

      var elTexto = winText.GetComponent<TMP_Text>();
      if (aWinningTeam==0){
          elTexto.color = team0TextColor;
          elTexto.outlineColor = team0OutlineColor;
      }else{
          elTexto.color = team1TextColor;
          elTexto.outlineColor = team1OutlineColor;
      }
      
      elTexto.text = "Ganó el equipo " + winner;
      */

      setTimeScale(0.1f, 5f);
      Invoke("resetearJuego", 0.1f);
    }

    void resetearJuego(){
        //resetTimeScale();
        //resetLives();

        //matchEnded = false;
        winText.SetActive(false);

        currentPlayers = new List<PlayerScript>();
        transitionScript.transition.setTransition(resultsScene, "noise0");
        transitionScript.transition.startTransition(0.5f);
        transitionScript.transition.setTransitionTexture(transBacks[currentWinningTeam]);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
