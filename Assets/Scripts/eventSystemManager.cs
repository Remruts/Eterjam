using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class eventSystemManager : MonoBehaviour {
  public GameObject currentButton;
  private AxisEventData currentAxis;
  private CustomTimer timer;
  private float maxTimeBetweenInputs = 0.3f;
  private float timeBetweenInputs = 0.3f;

  private bool canSelect = true;

  private MoveDirection prevDir = MoveDirection.Down;

  void Start(){
    timer = new CustomTimer(timeBetweenInputs, ()=>{canSelect = true;});
  }

  void Update(){
    
    timeBetweenInputs = Mathf.Lerp(timeBetweenInputs, maxTimeBetweenInputs, Time.deltaTime * 2.0f);
    
    if(canSelect) {
      currentAxis = new AxisEventData (EventSystem.current);
      if (EventSystem.current.currentSelectedGameObject == null){
        EventSystem.current.SetSelectedGameObject(currentButton);
      } else {
        currentButton = EventSystem.current.currentSelectedGameObject;
      }
      for (int i=1; i < 3; i++){
        selectStuff(i);
      }
    } else {
      timer.tick(timer.getDuration);
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
}
