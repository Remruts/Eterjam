using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManager;

public class ManagerScript : MonoBehaviour
{
    //global variables

    // player array

    ArrayList currentPlayers;

    int bodyCount;
    
    // Start is called before the first frame update
    void Start()
    {
        // Spawns players, player arrays?
         
        
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

        //SceneManager.loadScene(SceneManager.GetActiveScene().name);
        // Do something

        // show congrats()

    }
    

    void addPlayer()
    {

    }


}

