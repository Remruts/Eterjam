using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
Para efectos lindos en tus textos.
La forma de usarlo es así: Agregalo al objeto que tenga un componente de texto. Luego, en el texto usá alguno de los links siguientes:
	sine, jitter, rainbow, rainbow_sine.
Ej:
	<link=rainbow>Hola, este es un texto arcoiris.</link> También, podés poner links en cualquier otro lado y hasta combinarlo con rich text, como: <size=150%><link=jitter>MIEDO</link></size>
*/
public class textEffectsScript : MonoBehaviour {

	TMP_Text text;
	bool hasTextChanged = false;
	TMP_MeshInfo[] cachedMeshInfo;

	// Use this for initialization
	void Start () {
		text = GetComponent<TMP_Text>();
		cachedMeshInfo = text.textInfo.CopyMeshInfoVertexData();
	}

	void OnEnable()
	{
		// Subscribe to event fired when text object has been regenerated.
		TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
	}

	void OnDisable()
	{
		TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
	}

	void ON_TEXT_CHANGED(Object obj)
	{
		if (obj == text){
			hasTextChanged = true;
		}
	}

	void Update(){
		string linkID;

		if (hasTextChanged){
			cachedMeshInfo = text.textInfo.CopyMeshInfoVertexData();
			hasTextChanged = false;
		}

		//Vector3[] uiVertices = text.textInfo.meshInfo[0].vertices;
		for (int j=0; j < text.textInfo.linkCount; j++){
			var linkinfo = text.textInfo.linkInfo[j];
			linkID = linkinfo.GetLinkID();
			for (int i=0; i < linkinfo.linkTextLength; i++){
				TMP_CharacterInfo cInfo = text.textInfo.characterInfo[linkinfo.linkTextfirstCharacterIndex + i];

				if (!cInfo.isVisible){
					continue;
				}

				int materialIndex = cInfo.materialReferenceIndex;

				Vector3[] uiVertices = cachedMeshInfo[materialIndex].vertices;
				Vector3[] destinationVertices = text.textInfo.meshInfo[materialIndex].vertices;

				Vector3 offset = Vector3.zero;
				if (linkID == "jitter"){
					 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * Time.deltaTime * 60.0f;
				}
				if (linkID == "sine"){
					offset = new Vector3(0f, Mathf.Sin(Time.time * 10f + i), 0f);
				}
				if (linkID=="rainbow"){
					Color col = Color.HSVToRGB((Mathf.Sin(-Time.time+i/(float)linkinfo.linkTextLength) + 1f)/2f, 0.8f, 1f);
					changeCharColor(linkinfo.linkTextfirstCharacterIndex + i, (int)(col.r*255), (int)(col.g*255), (int)(col.b*255));
				}
				if (linkID=="rainbow_sine"){
					offset = new Vector3(0f, Mathf.Sin(Time.time * 10f + i), 0f);
					Color col = Color.HSVToRGB((Mathf.Sin(-Time.time+i/(float)linkinfo.linkTextLength) + 1f)/2f, 0.8f, 1f);
					changeCharColor(linkinfo.linkTextfirstCharacterIndex + i, (int)(col.r*255), (int)(col.g*255), (int)(col.b*255));
				}


				int vertexIndex = cInfo.vertexIndex;
				destinationVertices[vertexIndex + 0] = uiVertices[vertexIndex + 0] + offset;
				destinationVertices[vertexIndex + 1] = uiVertices[vertexIndex + 1] + offset;
				destinationVertices[vertexIndex + 2] = uiVertices[vertexIndex + 2] + offset;
				destinationVertices[vertexIndex + 3] = uiVertices[vertexIndex + 3] + offset;
			}
		}
		for (int i = 0; i < text.textInfo.meshInfo.Length; i++) {
			text.textInfo.meshInfo[i].mesh.vertices = text.textInfo.meshInfo[i].vertices;
			text.UpdateGeometry(text.textInfo.meshInfo[i].mesh, i);
		}

	}

	void changeCharColor(int j, int r, int g, int b){
		int materialIndex = text.textInfo.characterInfo[j].materialReferenceIndex;
		Color32[] newVertexColors = text.textInfo.meshInfo[materialIndex].colors32;
		int vertexIndex = text.textInfo.characterInfo[j].vertexIndex;

		//c0 = new Color32((byte)255, (byte)255, (byte)255, (byte)255);

		newVertexColors[vertexIndex + 0].r = (byte) r;
		newVertexColors[vertexIndex + 0].g = (byte) g;
		newVertexColors[vertexIndex + 0].b = (byte) b;

		newVertexColors[vertexIndex + 1].r = (byte) r;
		newVertexColors[vertexIndex + 1].g = (byte) g;
		newVertexColors[vertexIndex + 1].b = (byte) b;

		newVertexColors[vertexIndex + 2].r = (byte) r;
		newVertexColors[vertexIndex + 2].g = (byte) g;
		newVertexColors[vertexIndex + 2].b = (byte) b;

		newVertexColors[vertexIndex + 3].r = (byte) r;
		newVertexColors[vertexIndex + 3].g = (byte) g;
		newVertexColors[vertexIndex + 3].b = (byte) b;

		text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
	}
}
