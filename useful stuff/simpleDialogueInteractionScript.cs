using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Un ejemplo de herencia del interactionScript. Agregás esto a cualquier objeto con el que quieras que salga un texto cuando interactuás con él.
*/
public class simpleDialogueInteractionScript : interactionScript
{
    public TextAsset dialogueText;
    public string node = "Start";

    void Start(){
    }

    override public void interact(){
        if (ManagerScript.man.isInMenu){
            return;
        }
        ManagerScript.man.loadDialogue(dialogueText);
        ManagerScript.man.startDialogue(node);
    }

    public void changeNode(string newNode){
        node = newNode;
    }
}
