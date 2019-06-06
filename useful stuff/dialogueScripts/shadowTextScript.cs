using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Esto sólo copia el texto de otro texto (hasta en el editor). Está bueno para hacer sombras con el texto cuando el texto es tipo raster.
*/
[ExecuteInEditMode]
public class shadowTextScript : MonoBehaviour {

	public TMP_Text textToCopy;
	TMP_Text shadowText;
	RectTransform copyRect;

	void Awake(){
		shadowText = GetComponent<TMP_Text>();
		copyRect = textToCopy.gameObject.GetComponent<RectTransform>();
	}

	void Update(){
		if (shadowText != null && textToCopy != null){
			shadowText.text = textToCopy.text;
			RectTransform rect = shadowText.gameObject.GetComponent<RectTransform>();
			rect.sizeDelta = copyRect.sizeDelta;
		}
	}

}
