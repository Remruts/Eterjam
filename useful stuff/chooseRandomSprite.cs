using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Elige un sprite random de una lista de sprites
*/
public class chooseRandomSprite : MonoBehaviour {

	SpriteRenderer spr;
	public Sprite[] spriteList;

	void Start () {
		spr = GetComponent<SpriteRenderer>();
		spr.sprite = spriteList[Random.Range(0, spriteList.Length-1)];
	}
}
