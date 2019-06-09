using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteScript : MonoBehaviour
{
	public Sprite[] spriteList;
    // Start is called before the first frame update
    void Start()
    {
			GetComponent<SpriteRenderer>().sprite = spriteList[Random.Range(0, spriteList.Length)];
    }

}
