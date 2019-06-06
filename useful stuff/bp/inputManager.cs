using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class inputManager : MonoBehaviour {

	public static inputManager inputman;

	public string[] attackButton;
	public string[] blockButton;
	public string[] specialButton;
	public string[] dodgeButton;
	public string[] startButton;
	public string[] CWButton;
	public string[] CCWButton;
	public string[] CWSlowButton;
	public string[] CCWSlowButton;

	public string[] upButton;
	public string[] downButton;
	public string[] leftButton;
	public string[] rightButton;

	public bool[] keyboard;

	// Use this for initialization
	void Awake () {
		if (inputman == null){
			DontDestroyOnLoad(gameObject);
			inputman = this;
		} else {
			if (inputman != this){
				Destroy(gameObject);
				return;
			}
		}

		attackButton = new string[4];
		blockButton = new string[4];
		specialButton = new string[4];
		dodgeButton = new string[4];
		startButton = new string[4];
		CWButton = new string[4];
		CCWButton = new string[4];
		CWSlowButton = new string[4];
		CCWSlowButton = new string[4];

		upButton = new string[4];;
		downButton = new string[4];;
		leftButton = new string[4];;
		rightButton = new string[4];;

		keyboard = new bool[4];

		for (int i = 0; i < 4; ++i){
			resetButtons(i);
		}
	}

	public void resetButtons(int i){
		attackButton[i] = "joystick " + (i + 1) + " button 2";
		blockButton[i] = "joystick " + (i + 1) + " button 0";
		specialButton[i] = "joystick " + (i + 1) + " button 3";
		dodgeButton[i] = "joystick " + (i + 1) + " button 1";
		startButton[i] = "joystick " + (i + 1) + " button 7";
		CWButton[i] = "joystick " + (i + 1) + " button 5";
		CCWButton[i] = "joystick " + (i + 1) + " button 4";
		CWSlowButton[i] = "joystick " + (i + 1) + " button 9";
		CCWSlowButton[i] = "joystick " + (i + 1) + " button 8";

		upButton[i] = "up";
		downButton[i] = "down";
		leftButton[i] = "left";
		rightButton[i] = "right";

		keyboard[i] = false;
	}

	public bool AttackUp(int id){
		return Input.GetKeyUp (attackButton[id]);
	}

	public bool Attack(int id){
		return Input.GetKeyDown (attackButton[id]);
	}

	public void setAttack(int id, string button){
		swap(id, button, attackButton[id]);
		attackButton[id] = button;
	}

	public bool Block(int id){
		return Input.GetKeyDown (blockButton[id]);
	}

	public void setBlock(int id, string button){
		swap(id, button, blockButton[id]);
		blockButton[id] = button;
	}

	public bool Dodge(int id){
		return Input.GetKeyDown (dodgeButton[id]);
	}

	public void setDodge(int id, string button){
		swap(id, button, dodgeButton[id]);
		dodgeButton[id] = button;
	}

	public bool Special(int id){
		return Input.GetKeyDown (specialButton[id]);
	}

	public void setSpecial(int id, string button){
		swap(id, button, specialButton[id]);
		specialButton[id] = button;
	}

	public bool StartButton(int id){
		return Input.GetKeyDown (startButton[id]);
	}

	public void setStart(int id, string button){
		swap(id, button, startButton[id]);
		startButton[id] = button;
	}

	public bool CW(int id){
		return Input.GetKey (CWButton[id]);
	}

	public void setCW(int id, string button){
		swap(id, button, CWButton[id]);
		CWButton[id] = button;
	}

	public bool CCW(int id){
		return Input.GetKey (CCWButton[id]);
	}

	public void setCCW(int id, string button){
		swap(id, button, CCWButton[id]);
		CCWButton[id] = button;
	}

	public bool CWSlow(int id){
		return Input.GetKey (CWSlowButton[id]);
	}

	public void setCWSlow(int id, string button){
		swap(id, button, CWSlowButton[id]);
		CWSlowButton[id] = button;
	}

	public bool CCWSlow(int id){
		return Input.GetKey (CCWSlowButton[id]);
	}

	public void setCCWSlow(int id, string button){
		swap(id, button, CCWSlowButton[id]);
		CCWSlowButton[id] = button;
	}

	public bool Up(int id){
		return Input.GetKey(upButton[id]);
	}

	public void setUp(int id, string button){
		swap(id, button, upButton[id]);
		upButton[id] = button;
	}

	public bool Down(int id){
		return Input.GetKey(downButton[id]);
	}

	public void setDown(int id, string button){
		swap(id, button, downButton[id]);
		downButton[id] = button;
	}

	public bool Left(int id){
		return Input.GetKey(leftButton[id]);
	}

	public void setLeft(int id, string button){
		swap(id, button, leftButton[id]);
		leftButton[id] = button;
	}

	public bool Right(int id){
		return Input.GetKey(rightButton[id]);
	}

	public void setRight(int id, string button){
		swap(id, button, rightButton[id]);
		rightButton[id] = button;
	}

	public void toggleKeyboard(int id){
		keyboard[id] = !keyboard[id];
	}

	void swap(int id, string button, string swapString){
		if (attackButton[id] == button){
			attackButton[id] = swapString;
		}
		if (blockButton[id] == button){
			blockButton[id] = swapString;
		}
		if (specialButton[id] == button){
			specialButton[id] = swapString;
		}
		if (dodgeButton[id] == button){
			dodgeButton[id] = swapString;
		}
		if (startButton[id] == button){
			startButton[id] = swapString;
		}
		if (CWButton[id] == button){
			CWButton[id] = swapString;
		}
		if (CCWButton[id] == button){
			CCWButton[id] = swapString;
		}
		if (CWSlowButton[id] == button){
			CWSlowButton[id] = swapString;
		}
		if (CCWSlowButton[id] == button){
			CCWSlowButton[id] = swapString;
		}
		if (upButton[id] == button){
			upButton[id] = swapString;
		}
		if (downButton[id] == button){
			downButton[id] = swapString;
		}
		if (leftButton[id] == button){
			leftButton[id] = swapString;
		}
		if (rightButton[id] == button){
			rightButton[id] = swapString;
		}
	}

	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/input.dat");

		inputData data = new inputData();
		data.attackButton = attackButton;
		data.blockButton = blockButton;
		data.specialButton = specialButton;
		data.dodgeButton = dodgeButton;
		data.startButton = startButton;
		data.CWButton = CWButton;
		data.CCWButton = CCWButton;
		data.CWSlowButton = CWSlowButton;
		data.CCWSlowButton = CCWSlowButton;

		data.upButton = upButton;
		data.downButton = downButton;
		data.leftButton = leftButton;
		data.rightButton = rightButton;

		data.keyboard = keyboard;

		bf.Serialize(file, data);
		file.Close();
	}

	public void Load(){
		if (File.Exists(Application.persistentDataPath + "/input.dat")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/input.dat", FileMode.Open);
			inputData data = (inputData) bf.Deserialize(file);
			file.Close();

			attackButton = data.attackButton;
			blockButton = data.blockButton;
			specialButton = data.specialButton;
			dodgeButton = data.dodgeButton;
			startButton = data.startButton;
			CWButton = data.CWButton;
			CCWButton = data.CCWButton;
			CWSlowButton = data.CWSlowButton;
			CCWSlowButton = data.CCWSlowButton;

			upButton = data.upButton;
			downButton = data.downButton;
			leftButton = data.leftButton;
			rightButton = data.rightButton;

			keyboard = data.keyboard;

			if (attackButton == null){
				attackButton = new string[4];
				for (int i=0; i<4; i++){
					attackButton[i] = "joystick " + (i + 1) + " button 2";
				}
			}
			if (blockButton == null){
				blockButton = new string[4];
				for (int i=0; i<4; i++){
					blockButton[i] = "joystick " + (i + 1) + " button 0";
				}
			}
			if (specialButton == null){
				specialButton = new string[4];
				for (int i=0; i<4; i++){
					specialButton[i] = "joystick " + (i + 1) + " button 3";
				}
			}
			if (dodgeButton == null){
				dodgeButton = new string[4];
				for (int i=0; i<4; i++){
					dodgeButton[i] = "joystick " + (i + 1) + " button 1";
				}
			}
			if (startButton == null){
				startButton = new string[4];
				for (int i=0; i<4; i++){
					startButton[i] = "joystick " + (i + 1) + " button 7";
				}
			}
			if (CWButton == null){
				CWButton = new string[4];
				for (int i=0; i<4; i++){
					CWButton[i] = "joystick " + (i + 1) + " button 5";
				}
			}
			if (CCWButton == null){
				CCWButton = new string[4];
				for (int i=0; i<4; i++){
					CCWButton[i] = "joystick " + (i + 1) + " button 4";
				}
			}
			if (CWSlowButton == null){
				CWSlowButton = new string[4];
				for (int i=0; i<4; i++){
					CWSlowButton[i] = "joystick " + (i + 1) + " button 9";
				}
			}
			if (CCWSlowButton == null){
				CCWSlowButton = new string[4];
				for (int i=0; i<4; i++){
					CCWSlowButton[i] = "joystick " + (i + 1) + " button 8";
				}
			}

			if (upButton == null){
				upButton = new string[4];
				for (int i=0; i<4; i++){
					upButton[i] = "up";
				}
			}
			if (downButton == null){
				downButton = new string[4];
				for (int i=0; i<4; i++){
					downButton[i] = "down";
				}
			}
			if (leftButton == null){
				leftButton = new string[4];
				for (int i=0; i<4; i++){
					leftButton[i] = "left";
				}
			}
			if (rightButton == null){
				rightButton = new string[4];
				for (int i=0; i<4; i++){
					rightButton[i] = "right";
				}
			}

			if (keyboard == null){
				keyboard = new bool[4];
				for (int i=0; i<4; i++){
					keyboard[i] = false;
				}
			}
		}
	}
}

[Serializable]
class inputData{
	public string[] attackButton;
	public string[] blockButton;
	public string[] specialButton;
	public string[] dodgeButton;
	public string[] startButton;
	public string[] CWButton;
	public string[] CCWButton;
	public string[] CWSlowButton;
	public string[] CCWSlowButton;

	public string[] upButton;
	public string[] downButton;
	public string[] leftButton;
	public string[] rightButton;

	public bool[] keyboard;
}
