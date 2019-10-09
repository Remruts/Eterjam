using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/*
Script para transiciones lindas. Necesita del material con el shader y las texturas específicas.
*/
[ExecuteInEditMode]
public class transitionScript : MonoBehaviour {

	public Material EffectMaterial;
	public bool invert = false;
	public bool transitioning = false;
	public float transitionTime = 1f;

	public Texture[] transitionMasks;
	public static transitionScript transition;
	public string level = "";	

	float cutoff = 0f;
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
        if (Application.isEditor && !Application.isPlaying){
            cutoff = 0f;
        }
		EffectMaterial.SetFloat ("_Cutoff", cutoff);
	}

	void Update(){
        if (Application.isEditor && !Application.isPlaying) {
            return;
        }
		if (transitioning) {
			if (invert) {
				if (cutoff > 0) {
					cutoff -= Time.deltaTime / (transitionTime * Time.timeScale);
					if (cutoff <= 0) {
						cutoff = 0f;
						transitioning = false;
						invert = false;
					}
				}
			} else {
				if (cutoff < 1) {
					cutoff += Time.deltaTime / (transitionTime * Time.timeScale);
					if (cutoff >= 1) {
						cutoff = 1f;

						if (level != ""){
							goToScene(level);
							
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
						//invert = true;
					}
				}
			}

			EffectMaterial.SetFloat ("_Cutoff", cutoff);
		}
	}

	public void setTransition(string transLevel, string transitionType = "left_to_right"){
		setTransitionType(transitionType);
		level = transLevel;
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

    public void setTransitionTexture(Texture2D tex){
        if (tex != null){
            EffectMaterial.SetTexture("_TransitionTex", tex);
        }
    }

    public void goToScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

	public void setCutoff(float c){
		EffectMaterial.SetFloat ("_Cutoff", c);
	}

	public void startTransition(float time){        
		transitioning = true;
		transitionTime = time;
        invert = false;
	}
}
