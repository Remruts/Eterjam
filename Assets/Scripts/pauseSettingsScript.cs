using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class pauseSettingsScript : MonoBehaviour
{
  public GameObject questionContainer;
  public GameObject noButton;
  public GameObject yesButton;
  public GameObject optionsContainer;
  // Start is called before the first frame update
  public void resetOptions(){
    questionContainer.SetActive(false);
    optionsContainer.SetActive(true);
    GetComponent<eventSystemManager>().reset();
  }

  public void showQuestion(string question, string scene){
    questionContainer.SetActive(true);
    questionContainer.GetComponent<TMP_Text>().text = question;
    yesButton.GetComponent<goToButtonPause>().scene = scene;
    GetComponent<eventSystemManager>().setSelectedButton(noButton);
    optionsContainer.SetActive(false);
  }
}
