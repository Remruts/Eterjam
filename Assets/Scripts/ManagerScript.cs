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

    int bodyCount;

    public static ManagerScript coso;

    public GameObject winText;

    bool matchEnded = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Spawns players, player arrays?
        coso = this;
        currentPlayers = new List<PlayerScript>();
        /*
for (int i = 0; i < 10; i++)
{
    wait(100);
    GameObject mueble = Instantiate(furniturePrefab[Random.Range(0,furniturePrefab.Count)], new Vector3(0f, -5f + i*0.5f, 0f), Quaternion.Euler(0,0,Random.Range(0,180))) as GameObject;
}
        */
    }

    // Update is called once per frame
    void Update()
    {
			if (Input.GetKey("escape")){
				Application.Quit();
			}
    }


    public void onPlayerDeath(PlayerScript aDeadPlayer)
    {

        currentPlayers.Remove(aDeadPlayer);
        int deadPlayerTeam = aDeadPlayer.team;
        foreach (PlayerScript player in this.currentPlayers)
        {
            if (player.team == deadPlayerTeam)
            {
                return;
            }
        }
        this.roundOver((deadPlayerTeam + 1) % 2);
    }

    public void pauseGame()
    {
        // pause the game TODO
    }


    void roundOver(int aWinningTeam)
    {
        string winner = aWinningTeam == 0 ? "de Desarrollo" : "de Comercial";
        if (matchEnded)
        {
            return;
        }
        matchEnded = true;
        winText.SetActive(true);
        var elTexto = winText.GetComponent<TMP_Text>();
        if (aWinningTeam==0)
        {
            elTexto.color = new Color32(224, 116, 222, 255);
        }else{
            elTexto.color = new Color32(73, 178, 99, 255);
        }

        elTexto.text = "Ganó el equipo " + winner;


        Invoke("resetearJuego", 5f);

    }

    void resetearJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void addPlayer(PlayerScript p)
    {
        currentPlayers.Add(p);
    }

    IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
    }


}
