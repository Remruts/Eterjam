using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

/*
Siento que es más fácil tener un objeto manager que tenga un script como este.
Al mismo objeto se le pueden agregar otros managers, como el itemManager.
Si no se van a usar cajas de diálogos con Yarn, se pueden borrar esas líneas.
*/
public class ManagerScript : MonoBehaviour {

	public static ManagerScript man;
	public bool debug = true;

	public GameObject playerChar;
	public GameObject[] followers;

	//[HideInInspector]
	//public List<GameObject> obstacles;

	public Yarn.Unity.DialogueRunner dialog;

	public bool transitioning = false;

	public bool isPaused = false;
	public string currentSpeaker = "Narrator";

	public string currentLocation = "Room 1";

	void Awake(){
		if (man == null){
			DontDestroyOnLoad(this.gameObject);
			man = this;
		} else {
			Destroy(gameObject);
			return;
		}
	}

	void Start(){
		//updateObstacles();
	}

	/*
	void addObstaclesWithTag(string tag){
		GameObject[] goArray = GameObject.FindGameObjectsWithTag(tag);
		for (int i = 0; i < goArray.Length; i++){
			Collider2D col = goArray[i].GetComponent<Collider2D>();
			if (col != null && col.enabled){
				obstacles.Add(goArray[i]);
			}
		}
	}

	void updateObstacles(){
		obstacles = new List<GameObject>();
		addObstaclesWithTag("solid");
		//addObstaclesWithTag("enemy");
		addObstaclesWithTag("player");
	}
	*/

	void Update () {
		//updateObstacles();

		if (Input.GetKeyDown("escape")){
			Application.Quit();
		}

		if (isInCutscene){
			return;
		}

		if (Input.GetButtonDown("Pause")){
			isPaused = !isPaused;
			if (isPaused){
				pauseIcon.SetActive(true);
				pauseGame();
			} else {
				pauseIcon.SetActive(false);
				resumeGame();
			}
		}
	}

	public void pauseGame(){
		Time.timeScale = 0.0f;
	}

	public void resumeGame(){
		Time.timeScale = 1.0f;
	}

	public void startDialogue(string node){
		if (dialog.isDialogueRunning == false){
			dialog.StartDialogue (node);
		}
	}

	public void loadDialogue(TextAsset text){
		if (dialog.isDialogueRunning == false){
			dialog.Clear();
			dialog.AddScript(text);
		}
	}

	public void ResetScene(){
		string scene = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(scene);
	}

	public void GoToScene(string scene){
		//TODO: LOAD ASYNC y quedar en transición hasta terminar de cargar.
		SceneManager.LoadScene(scene);
		locationText.GetComponent<Animator>().Play("hidden");
	}

	public void resetInteraction(){
		isInCutscene = false;
	}

	public void stopInteraction(){
		isInCutscene = true;
	}

	public void changeCharaPositions(Vector3 newPos){
		playerChar.transform.position = newPos;
		foreach (GameObject follower in followers){
			follower.transform.position = newPos + Vector3.up * 0.5f;
		}
		Camera.main.GetComponent<camScript>().jumpTo(newPos);
	}

	public void changeFacingDirection(Direction faceDir){
		float faceX = -1;
		float faceY = 0;

		switch(faceDir){
		case Direction.Up:
			faceX = 0;
			faceY = 1;
		break;
		case Direction.Down:
			faceX = 0;
			faceY = -1;
		break;
		case Direction.Left:
			faceX = -1;
			faceY = 0;
		break;
		case Direction.Right:
			faceX = 1;
			faceY = 0;
		break;
		}

		Animator pcAnim = playerChar.GetComponent<Animator>();
		pcAnim.SetFloat("FaceX", faceX);
		pcAnim.SetFloat("FaceY", faceY);

		foreach (GameObject follower in followers){
			Animator followerAnim = follower.GetComponent<Animator>();
			followerAnim.SetFloat("FaceX", faceX);
			followerAnim.SetFloat("FaceY", faceY);
		}
	}

	public void disableMC(){
		playerChar.SetActive(false);
	}

	public void disableFollower(int i){
		if (i < 0 || i > followers.Length-1){
			return;
		}
		followers[i].SetActive(false);
	}
}
