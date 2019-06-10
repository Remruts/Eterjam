using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScipt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("P1Jump") || Input.GetButton("P2Jump")) {
            SceneManager.LoadScene("SpritesScene");
        }
				if (Input.GetKey("escape")){
					Application.Quit();
				}
    }
}
