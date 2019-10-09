using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomizePitchScript : MonoBehaviour
{
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    // Start is called before the first frame update
    void Start(){
      GetComponent<AudioSource>().pitch = Random.Range(minPitch, maxPitch);
    }
}
