using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class markerScript : MonoBehaviour
{
  public Sprite[] markers;
  public Color[] playerColors;
  SpriteRenderer spr;
  TMP_Text childText;
  // Start is called before the first frame update
  void Start(){
    spr = GetComponent<SpriteRenderer>();
    childText = GetComponentInChildren<TMP_Text>();
    updateMarker();
  }

  public void updateMarker(){
    var player = GetComponentInParent<PlayerScript>();
    
    string theText = $"J{player.team + 1}";
    Color theColor = playerColors[player.team];
    if (player.cpu){
      theText = $"CPU{player.team + 1}";
      spr.sprite = markers[markers.Length-1];
      theColor = playerColors[playerColors.Length-1];
    } else {
      spr.sprite = markers[player.team];
    }

    childText.text = theText;
    childText.color = theColor;
  }

  // Update is called once per frame
  void Update(){
    updateMarker();
  }
}
