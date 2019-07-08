using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScipt : MonoBehaviour
{
    void Start(){
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("P1Jump") || Input.GetButtonDown("P2Jump") || Input.GetButtonDown("P1Start") || Input.GetButtonDown("P2Start")) {
            transitionScript.transition.setTransition("SpritesScene", "noise0");
            transitionScript.transition.startTransition(0.5f);
            //SceneManager.LoadScene("SpritesScene");
        }
        if (Input.GetKey("escape")){
            Application.Quit();
        }
    }
}
