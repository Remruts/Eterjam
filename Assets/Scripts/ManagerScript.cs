using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ManagerScript : MonoBehaviour
{
    //global variables

    // player array

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void onPlayerDeath(PlayerScript aDeadPlayer)
    {
        
        currentPlayers.Remove(aDeadPlayer);
        int deadPlayerTeam = aDeadPlayer.team;
        foreach (PlayerScript player in this.currentPlayers)
        {
            if(player.team == deadPlayerTeam)
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
        if (matchEnded)
        {
            return;
        }
        matchEnded = true;
        winText.SetActive(true);
        winText.GetComponent<TMP_Text>().text = "Ganó el equipo " + aWinningTeam.ToString();


        Invoke("resetearJuego", 5f);

        // show congrats()
        Debug.Log("Ganó el equipo " + aWinningTeam.ToString());

    }

    void resetearJuego()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void addPlayer(PlayerScript p)
    {
        currentPlayers.Add(p);
    }


}

