using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class eventSystemManager : MonoBehaviour {
  public GameObject currentButton;
  public RectTransform cursor;
  private AxisEventData currentAxis;
  private CustomTimer timer;
  private float maxTimeBetweenInputs = 0.3f;
  private float timeBetweenInputs = 0.3f;

  private bool canSelect = true;

  private MoveDirection prevDir = MoveDirection.Down;
  private GameObject startButton;

  void Awake(){
    startButton = currentButton;
  }

  void Start(){
    timer = new CustomTimer(timeBetweenInputs, ()=>{canSelect = true;}, true);  
  }

  void Update(){
    
    timeBetweenInputs = Mathf.Lerp(timeBetweenInputs, maxTimeBetweenInputs, Time.deltaTime * 2.0f);
    
    if(canSelect) {
      currentAxis = new AxisEventData (EventSystem.current);
      if (EventSystem.current.currentSelectedGameObject == null){
        EventSystem.current.SetSelectedGameObject(currentButton);
      }
      for (int i=1; i < 3; i++){
        selectStuff(i);
      }
      currentButton = EventSystem.current.currentSelectedGameObject;
    } else {
      timer.tick(timer.getDuration);
    }

    if (cursor != null){
      cursor.SetParent(currentButton.transform.GetChild(0));
      cursor.anchoredPosition = new Vector3(0f, 0f, 10f);
      cursor.localScale = Vector3.one;
    }
  }

  void selectStuff(int i){
    if (Input.GetAxis($"P{i}Vertical") < -0.5) {
      // move up
      updateEventSystem(MoveDirection.Up);
    } else if (Input.GetAxis($"P{i}Vertical") > 0.5) {
      // move down
      updateEventSystem(MoveDirection.Down);
    } else if (Input.GetAxis($"P{i}Horizontal") > 0.5) {
      // move right
      updateEventSystem(MoveDirection.Right);
    } else if (Input.GetAxis($"P{i}Horizontal") < -0.5) {
      // move left
      updateEventSystem(MoveDirection.Left);
    }
  }

  void updateEventSystem(MoveDirection dir){
    currentAxis.moveDir = dir;
    ExecuteEvents.Execute(currentButton, currentAxis, ExecuteEvents.moveHandler);
    canSelect = false;
    if (dir == prevDir){
      timeBetweenInputs = Mathf.Max(0f, timeBetweenInputs - 0.05f);
    } else {
      timeBetweenInputs = maxTimeBetweenInputs;
    }
    timer.start(timeBetweenInputs);
    prevDir = dir;
  }

  public void reset(){
    setSelectedButton(startButton);
  }
  public void setSelectedButton(GameObject theButton){
    EventSystem.current.SetSelectedGameObject(theButton);
    currentButton = theButton;
    currentButton.GetComponent<ButtonScript>().selected = true;
  }
}
