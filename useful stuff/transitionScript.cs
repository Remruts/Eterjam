using UnityEngine;
using System.Collections;

/*
Script para transiciones lindas. Necesita del material con el shader y las texturas específicas.
*/
[ExecuteInEditMode]
public class transitionScript : MonoBehaviour {

	public Material EffectMaterial;
	public bool invert = false;
	public bool transitioning = false;
	public float transitionTime = 3f;

	public Texture[] transitionMasks;
	public static transitionScript transition;
	public string level = "";
	public string newLocationName = "???";
	public bool showText = true;

	float cutoff = 0f;
	Vector3 newCharPos;
	ManagerScript.Direction newFaceDir;
	public string newTransitionType = "left_to_right";

	void Awake(){
		if (transition == null){
			transition = this;
		}
	}

	void Start(){
		setTransitionType(newTransitionType);
		//uint index = (uint) Mathf.Floor(Random.Range (0, transitionMasks.Length));
		//EffectMaterial.SetTexture ("_MaskTex", transitionMasks [index]);
		cutoff = invert ? 1 : 0;
		EffectMaterial.SetFloat ("_Cutoff", cutoff);
	}

	void Update(){
		if (transitioning) {
			if (invert) {
				if (cutoff > 0) {
					cutoff -= Time.deltaTime / (transitionTime * Time.timeScale);
					if (cutoff <= 0) {
						cutoff = 0f;
						transitioning = false;
						invert = false;
						ManagerScript.man.resetInteraction();
						if (showText){
							ManagerScript.man.showLocationText();
						}
					}
				}
			} else {
				if (cutoff < 1) {
					cutoff += Time.deltaTime / (transitionTime * Time.timeScale);
					if (cutoff >= 1) {
						cutoff = 1f;

						if (level != ""){
							ManagerScript.man.GoToScene(level);
							ManagerScript.man.changeCharaPositions(newCharPos);
							ManagerScript.man.changeFacingDirection(newFaceDir);
							ManagerScript.man.changeLocationText(newLocationName);

							switch(newTransitionType){
							case "left_to_right":
								setTransitionType("right_to_left");
							break;
							case "right_to_left":
								setTransitionType("left_to_right");
							break;
							case "top_to_bot":
								setTransitionType("bot_to_top");
							break;
							case "bot_to_top":
								setTransitionType("top_to_bot");
							break;
							case "noise2":
								setTransitionType("noise3");
							break;
							case "noise3":
								setTransitionType("noise2");
							break;

							}
						}
						invert = true;
					}
				}
			}

			EffectMaterial.SetFloat ("_Cutoff", cutoff);
		}
	}

	public void setTransition(string transLevel, Vector3 charaPos, ManagerScript.Direction faceDir, string transitionType = "left_to_right", string locationName = "???", bool showLocationText = true){
		setTransitionType(transitionType);
		level = transLevel;
		newCharPos = charaPos;
		newFaceDir = faceDir;
		newLocationName = locationName;
		showText = showLocationText;
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst){
		Graphics.Blit (src, dst, EffectMaterial);
	}

	public void setTransitionType(string transType){
		Texture tex = System.Array.Find(transitionMasks, element => element.name == transType);
		if (tex != null){
			EffectMaterial.SetTexture ("_MaskTex",tex);
		}
		newTransitionType = transType;
	}

	public void setCutoff(float c){
		EffectMaterial.SetFloat ("_Cutoff", c);
	}

	public void startTransition(float time){
		transitioning = true;
		transitionTime = time;
		ManagerScript.man.stopInteraction();
	}
}
