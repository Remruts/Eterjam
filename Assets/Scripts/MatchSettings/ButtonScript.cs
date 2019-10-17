using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


[RequireComponent(typeof(Button))]
public class ButtonScript : MonoBehaviour{
  protected Button theButton;
  protected TMP_Text buttonText;
  protected bool canPress = true;
  private float maxTimeBetweenInputs = 0.3f;
  private float timeBetweenInputs = 0.3f;
  private MoveDirection prevDir = MoveDirection.Right;
  protected CustomTimer pressTimer;
  public bool selected = false;
  public TMP_FontAsset disabledFont;

  public Color selectedColor = Color.white;
  public Color notSelectedColor = Color.white;
  // Start is called before the first frame update
  protected virtual void Start(){
    theButton = GetComponent<Button>();
    buttonText = GetComponentInChildren<TMP_Text>();
    if (!theButton.IsInteractable()){
      buttonText.font = disabledFont;
    }
    pressTimer = new CustomTimer(timeBetweenInputs, ()=> {canPress = true;}, true);
  }

  protected virtual void Update(){
    if (!theButton.IsInteractable()){
      return;
    }

    if (!canPress){
      pressTimer.tick(pressTimer.getDuration);
    }
    timeBetweenInputs = Mathf.Lerp(timeBetweenInputs, maxTimeBetweenInputs, Time.deltaTime * 2.0f);
    
    this.selected = (gameObject == EventSystem.current.currentSelectedGameObject);

    //FIXME: COLOR DEL BOTÓN CUANDO ES SELECCIONADO
    //buttonText.color = (this.selected ? new Color(0.46f, 1f, 0.4f, 1f) : Color.white);
    buttonText.color = (this.selected ? selectedColor : notSelectedColor);

    if (this.selected && canPress){
      for (int i = 1; i < 3; i++){
        float axis = Input.GetAxis($"P{i}Horizontal");
        if (axis > 0.5f){
          this.rightAction();
          canPress = false;
          if (prevDir == MoveDirection.Right){
            timeBetweenInputs = Mathf.Max(0f, timeBetweenInputs - 0.05f);
          } else {
            timeBetweenInputs = maxTimeBetweenInputs;
          }
          prevDir = MoveDirection.Right;
          pressTimer.start(timeBetweenInputs);
        } else if (axis < -0.5f){
          this.leftAction();
          canPress = false;
          if (prevDir == MoveDirection.Left){
            timeBetweenInputs = Mathf.Max(0f, timeBetweenInputs - 0.05f);
          } else {
            timeBetweenInputs = maxTimeBetweenInputs;
          }
          prevDir = MoveDirection.Left;
          pressTimer.start(timeBetweenInputs);
        }

        bool buttonPressed = Input.GetButtonDown($"P{i}Jump");
        if (buttonPressed){
          this.pressAction();
          canPress = false;
          timeBetweenInputs = maxTimeBetweenInputs;
          pressTimer.resetTimerInstantly();
        }
      }
    }
  }

  public virtual void rightAction(){}
  public virtual void leftAction(){}
  public virtual void pressAction(){}
}
