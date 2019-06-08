using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScript : MonoBehaviour
{
    //global variables

    // player array

    public List<PlayerScript> currentPlayers;

    int bodyCount;

    public static ManagerScript coso;
    
    // Start is called before the first frame update
    void Start()
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
        this.roundOver(deadPlayerTeam + 1 % 2);
    }

    public void pauseGame()
    {
        // pause the game TODO
    }
    

   void roundOver(int aWinningTeam)
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Do something

        // show congrats()
        Debug.Log("Ganó el equipo " + aWinningTeam.ToString());

    }


    public void addPlayer(PlayerScript p)
    {
        currentPlayers.Add(p);
    }


}

