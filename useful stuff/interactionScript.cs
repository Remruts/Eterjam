using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
Interfaz tonta para los objetos interactivos.
Otros scripts pueden llamar a este script haciendo algo como

		objetito.GetComponent<interactionScript>().interact();

Siendo por ejemplo "objetito" un objeto con el que se colisionó o está cerca del personaje.
*/
public class interactionScript : MonoBehaviour
{
    public virtual void interact(){

    }
}
