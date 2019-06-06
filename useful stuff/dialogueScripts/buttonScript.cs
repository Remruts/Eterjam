using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
/*
Script tonto para que no se salga de foco un botón cuando cliqueás con el mouse afuera.
*/
public class buttonScript : MonoBehaviour
{
    public TMP_Text buttonText;
    GameObject lastselect;

    void Start(){
        lastselect = null;
    }

    void Update(){
        if (EventSystem.current.currentSelectedGameObject == null){
            EventSystem.current.SetSelectedGameObject(lastselect);
        } else {
            lastselect = EventSystem.current.currentSelectedGameObject;
            if (EventSystem.current.currentSelectedGameObject == gameObject){
                buttonText.color = Color.black;
            } else {
                buttonText.color = Color.white;
            }
        }

    }
}
