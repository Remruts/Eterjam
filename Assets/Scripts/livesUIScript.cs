using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class livesUIScript : MonoBehaviour
{
  Image livesImage;
  public GameObject livesText;

  public int team = 0;
  // Start is called before the first frame update
  void Start(){
    livesImage = GetComponent<Image>();    
  }

  // Update is called once per frame
  void Update(){
    //TODO: Hacerlo en el manager, no en cada update
    resizeLives();
  }

  public void resizeLives(){
    if (MatchManager.match != null){
      int livesNum = MatchManager.match.playerLives[team];
      float width = livesNum * 160;
      if (livesNum > 5){
        width = 160;
        if (!livesText.activeSelf){
          livesText.SetActive(true);
        }
        string livesString = (livesNum < 10 ? " " : "" ) + $"x{livesNum}";
        livesText.GetComponent<TMP_Text>().text = livesString;
      } else {
        if (livesText.activeSelf){
          livesText.SetActive(false);
        }
      }
      livesImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
  }
}
