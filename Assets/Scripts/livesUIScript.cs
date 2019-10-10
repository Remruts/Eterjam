using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class livesUIScript : MonoBehaviour
{
  Image livesImage;
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
      float width = MatchManager.match.playerLives[team] * 160;
      livesImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
      
    }
  }
}
