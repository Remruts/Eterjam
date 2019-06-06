using UnityEngine;
﻿using System.Collections;
using System.Collections.Generic;

/*
Script para que los sprites estén ordenados (orden de dibujo) por posición en Y, lo que les da un efecto de profundidad a los objetos.
*/
public class dynamicOrderScript : MonoBehaviour {

	SpriteRenderer spr;
	public float pixelUnits = 16f;

	void Start () {
		spr = GetComponent<SpriteRenderer>();
		spr.sortingOrder = Mathf.RoundToInt(transform.position.y * pixelUnits) * -1;
	}

	void Update () {
		if (!gameObject.isStatic){
			spr.sortingOrder = Mathf.RoundToInt(transform.position.y * pixelUnits) * -1;
		}
	}
}
