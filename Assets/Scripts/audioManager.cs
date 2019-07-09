using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager audioman;
    // Por ahora sólo sigue andando la música
    void Awake(){
        if (audioman == null){
            DontDestroyOnLoad(this.gameObject);
            audioman = this;
        }
        else{
            Destroy(gameObject);
            return;
        }
    }
}
