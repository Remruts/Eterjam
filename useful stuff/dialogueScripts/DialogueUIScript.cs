/*

The MIT License (MIT)

Copyright (c) 2015 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

// Displays dialogue lines to the player, and sends
// user choices back to the dialogue system.

// Note that this is just one way of presenting the
// dialogue to the user. The only hard requirement
// is that you provide the RunLine, RunOptions, RunCommand
// and DialogueComplete coroutines; what they do is up to you.


//NOTE: Esta es una versión modificada del DialogueUIScript.

public class DialogueUIScript : Yarn.Unity.DialogueUIBehaviour
{

	// The object that contains the dialogue and the options.
	// This object will be enabled when conversation starts, and
	// disabled when it ends.
	public GameObject dialogueContainer;
	public GameObject[] dialogueBoxes;

	// The UI element that displays lines
	public TMP_Text lineText;
	public TMP_Text shadowText;

	// A UI element that appears after lines have finished appearing
	public GameObject continuePrompt;

	//UI element for the name of the speaker
	public GameObject speakerLabel;
	public TMP_Text speakerLabelText;

	// UI for items, etc.
	public GameObject itemContainer;
	public GameObject itemMenu;

		// A delegate (ie a function-stored-in-a-variable) that
	// we call to tell the dialogue system about what option
	// the user selected
	private Yarn.OptionChooser SetSelectedOption;

	[Tooltip("How quickly to show the text, in seconds per character")]
	public float textSpeed = 0.025f;
	float regularSpeed;

	// The buttons that let the user choose an option
	List<Button> optionButtons;
	public GameObject optionsContainer;

	string currentSpeaker = "Narrator";
	string prevLine;

	VariableStorageScript variableStorage;

	void Awake ()
	{
		variableStorage = GetComponent<VariableStorageScript>();

		// Start by hiding the container, line and option buttons
		if (dialogueContainer != null)
			dialogueContainer.SetActive(false);

		if (lineText != null){
			lineText.gameObject.SetActive (false);
		}
		if (shadowText != null){
			shadowText.gameObject.SetActive (false);
		}

		if (speakerLabel != null){
			speakerLabel.SetActive(false);
		}
		if (speakerLabelText != null){
			speakerLabelText.gameObject.SetActive(false);
		}
		if (optionsContainer != null){
			optionsContainer.SetActive(false);
		}

		if (itemContainer != null){
			itemContainer.SetActive(false);
		}

		// Hide the continue prompt if it exists
		if (continuePrompt != null)
			continuePrompt.SetActive (false);

		regularSpeed = textSpeed;
	}


	// Show a line of dialogue, gradually
	public override IEnumerator RunLine (Yarn.Line line)
	{
		// Show the text
		lineText.gameObject.SetActive (true);
		shadowText.gameObject.SetActive (true);

		lineText.text = CheckVars(line.text);
		lineText.ForceMeshUpdate(true);
		shadowText.text = lineText.text;
		shadowText.ForceMeshUpdate(true);

		int charaCount = lineText.textInfo.characterCount;

		if (textSpeed > 0.0f) {

			// Make every line invisible
			for (int i = 0; i < charaCount; i++){
				changeCharAlpha(lineText, i, 0);
				changeCharAlpha(shadowText, i, 0);
			}

			var linkinfo = lineText.textInfo.linkInfo;
			// Display the line one character at a time
			for (int i=0; i < charaCount; i++) {

				textSpeed = regularSpeed;

				for (int k=0; k < lineText.textInfo.linkCount; k++){
					if (linkinfo[k].GetLinkID() == "slowText"){
						if (i >= linkinfo[k].linkTextfirstCharacterIndex && i < linkinfo[k].linkTextfirstCharacterIndex + linkinfo[k].linkTextLength){
							textSpeed = 0.2f;
							Debug.Log("Changing text speed to: " +  textSpeed.ToString("F4"));
						}
					} else if (linkinfo[k].GetLinkID() == "instantText"){
						if (i == linkinfo[k].linkTextfirstCharacterIndex){
							for (int j=0; j < linkinfo[k].linkTextLength; j++){
								changeCharAlpha(lineText, i+j, 255);
								changeCharAlpha(shadowText, i+j, 255);
							}
							i = linkinfo[k].linkTextfirstCharacterIndex + linkinfo[k].linkTextLength-1;
						}
					} else if (linkinfo[k].GetLinkID() == "pause"){
						if (i == linkinfo[k].linkTextfirstCharacterIndex){
							yield return new WaitForSeconds (0.5f);
						}
					}
				}

				changeCharAlpha(lineText, i, 255);
				changeCharAlpha(shadowText, i, 255);
				lineText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
				shadowText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

				if (Input.GetButtonDown("Interact")){
					for (int j = i; j <= charaCount; j++){
						changeCharAlpha(lineText, j, 255);
						changeCharAlpha(shadowText, j, 255);
					}
					yield return null;
					break;
				}
				//Debug.Log(textSpeed.ToString("F4"));
				yield return new WaitForSeconds (textSpeed);
			}
		} else {
			// Display the line immediately if textSpeed == 0
			if (line.text != ""){
				lineText.text = line.text;
			}
		}

		// Show the 'press any key' prompt when done, if we have one
		if (continuePrompt != null)
			continuePrompt.SetActive (true);

		// Wait for any user input
		while (Input.GetButtonDown("Interact") == false) {
			yield return null;
		}

		prevLine = lineText.text;
		if (itemContainer != null && itemContainer.activeSelf){
			//itemContainer.SetActive(false);
			itemContainer.GetComponent<itemContainerScript>().Restart();
		}
		//lineText.text = "";
		//shadowText.text = "";
		// Hide the text and prompt
		//lineText.gameObject.SetActive (false);

		if (continuePrompt != null)
			continuePrompt.SetActive (false);
		yield return null;
	}

	string CheckVars (string input){
        string output = string.Empty;
        bool checkingVar = false;
        string currentVar = string.Empty;

        int index = 0;
        while (index < input.Length) {
            if (input [index] == '[') {
                checkingVar = true;
                currentVar = string.Empty;
            } else if (input [index] == ']') {
                checkingVar = false;
                output += ParseVariable(currentVar);
                currentVar = string.Empty;
            } else if (checkingVar) {
                currentVar += input [index];
            } else {
                output += input[index];
            }
            index += 1;
        }

        return output;
    }

    string ParseVariable (string varName){
        //Check YarnSpinner's variable storage first
        if (variableStorage.GetValue (varName) != Yarn.Value.NULL) {
            return variableStorage.GetValue (varName).AsString;
        }

        //Handle other variables here
        if(varName == "$time") {
            return Time.time.ToString();
        }

        //If no variables are found, return the variable name
        return varName;
    }

	void changeCharAlpha(TMP_Text tmp_text, int j, int alpha){
		int materialIndex = tmp_text.textInfo.characterInfo[j].materialReferenceIndex;
		Color32[] newVertexColors = tmp_text.textInfo.meshInfo[materialIndex].colors32;
		int vertexIndex = tmp_text.textInfo.characterInfo[j].vertexIndex;

		//c0 = new Color32((byte)255, (byte)255, (byte)255, (byte)255);

		newVertexColors[vertexIndex + 0].a = (byte) alpha;
		newVertexColors[vertexIndex + 1].a = (byte) alpha;
		newVertexColors[vertexIndex + 2].a = (byte) alpha;
		newVertexColors[vertexIndex + 3].a = (byte) alpha;
		tmp_text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
	}

	void changeCharColor(TMP_Text tmp_text, int j, Color32 col){
		int materialIndex = tmp_text.textInfo.characterInfo[j].materialReferenceIndex;
		Color32[] newVertexColors = tmp_text.textInfo.meshInfo[materialIndex].colors32;
		int vertexIndex = tmp_text.textInfo.characterInfo[j].vertexIndex;

		//c0 = new Color32((byte)255, (byte)255, (byte)255, (byte)255);
		col.a = newVertexColors[vertexIndex + 0].a;

		newVertexColors[vertexIndex + 0] = col;
		newVertexColors[vertexIndex + 1] = col;
		newVertexColors[vertexIndex + 2] = col;
		newVertexColors[vertexIndex + 3] = col;
		tmp_text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
	}

	// Show a list of options, and wait for the player to make a selection.
	public override IEnumerator RunOptions (Yarn.Options optionsCollection,
	                                        Yarn.OptionChooser optionChooser)
	{
		//Display the previous line
		lineText.text = prevLine;
		shadowText.text = prevLine;

		int optionsNumber = optionsCollection.options.Count;

		optionsContainerScript optscr = null;
		if (optionsNumber > 0){
			optionsContainer.SetActive(true);
			optscr = optionsContainer.GetComponent<optionsContainerScript>();
			optionButtons = optscr.addButtonsWithOptions(optionsNumber);
		}

		// Display each option in a button, and make it visible
		int i = 0;
		foreach (var optionString in optionsCollection.options) {
			optionButtons[i].gameObject.SetActive (true);
			//optionButtons[i].onClick.AddListener(() => SetOption(i));

			EventTrigger trigger = optionButtons[i].GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.Submit;

			int currentNum = i;
			entry.callback.AddListener(
				(data) => SetOption(currentNum)
			);
			trigger.triggers.Add(entry);

			foreach (TMP_Text txt in optionButtons[i].GetComponentsInChildren<TMP_Text> ()){
				txt.text = optionString;
			}

			i++;
		}

		// Record that we're using it
		SetSelectedOption = optionChooser;

		// Wait until the chooser has been used and then removed (see SetOption below)
		while (SetSelectedOption != null) {
			yield return null;
		}

		if (itemMenu != null && itemMenu.activeSelf){
			itemMenu.SetActive(false);
			ManagerScript.man.givingItems = false;
		}
		if (optscr != null){
			optscr.destroyAllButtons();
		}
		optionButtons = null;
		optionsContainer.SetActive(false);
	}

	public void SetDialogueBox(string type){
		foreach (GameObject db in dialogueBoxes){
			db.SetActive(false);
		}
		switch (type){
		case "Narrator":
			dialogueBoxes[1].SetActive(true);
			break;
		case "Regular":
			dialogueBoxes[0].SetActive(true);
			break;
		}
	}

	// Called by buttons to make a selection.
	public void SetOption (int selectedOption)
	{
		// Call the delegate to tell the dialogue system that we've
		// selected an option.
		SetSelectedOption (selectedOption);

		// Now remove the delegate so that the loop in RunOptions will exit
		SetSelectedOption = null;
	}

	// Run an internal command.
	public override IEnumerator RunCommand (Yarn.Command command)
	{
		// "Perform" the command
		Debug.Log ("Command: " + command.text);

		yield break;
	}

	public override IEnumerator DialogueStarted ()
	{
		ManagerScript.man.stopInteraction();
		Debug.Log ("Dialogue starting!");

		// Enable the dialogue controls.
		if (dialogueContainer != null)
			dialogueContainer.SetActive(true);

		yield break;
	}

	// Yay we're done. Called when the dialogue system has finished running.
	public override IEnumerator DialogueComplete ()
	{
		Debug.Log ("Complete!");
		UpdateSpeakerLabel("");
		lineText.text = "";
		shadowText.text = "";

		// Hide the dialogue interface.
		if (dialogueContainer != null){
			dialogueContainer.SetActive(false);
		}

		ManagerScript.man.resetInteraction();

		yield break;
	}

	// Esto es usado en mi juego para adaptar el tamaño del label del hablante a su nombre. Pero se puede borrar...
	void UpdateSpeakerLabel(string speaker){
		speakerLabelText.text = speaker;

		RectTransform textRect = speakerLabelText.gameObject.GetComponent<RectTransform>();
		Vector2 sizeD = textRect.sizeDelta;
		sizeD.x = speaker.Length * 16;
		textRect.sizeDelta = sizeD;
		RectTransform labelRect = speakerLabel.GetComponent<RectTransform>();
		sizeD.x += 40;
		sizeD.x = Mathf.Max(sizeD.x, 112);
		labelRect.sizeDelta = sizeD;
	}

	[Yarn.Unity.YarnCommand("changespeaker")]
	public void changeSpeaker(String speaker){
		if (speakerLabel == null || speakerLabelText == null){
			Debug.Log("No speaker label found.");
			return;
		}

		switch (speaker){
		case "Narrator":
			speakerLabel.SetActive(false);
			speakerLabelText.gameObject.SetActive(false);
			SetDialogueBox("Narrator");
			return;
		case "Cottontail":
			speaker = "Lady Bluebell Cottontail";
			break;
		}
		currentSpeaker = speaker;
		ManagerScript.man.currentSpeaker = speaker;

		speakerLabel.SetActive(true);
		speakerLabelText.gameObject.SetActive(true);
		UpdateSpeakerLabel(currentSpeaker);
	}

	[Yarn.Unity.YarnCommand("getitem")]
	public void getItem(String item){
		if (itemContainer != null){
			itemContainer.SetActive(true);
			itemContainer.GetComponent<itemContainerScript>().Animate();
			itemManagerScript.itemMan.AddItem(item);
		}
	}

	[Yarn.Unity.YarnCommand("giveitem")]
	public void giveItem(String item){
		itemManagerScript.itemMan.RemoveItem(item);
	}

	[Yarn.Unity.YarnCommand("expression")]
	public void changeExpression(String expression){
		//Maybe in the future...
	}

	[Yarn.Unity.YarnCommand("changedialoguebox")]
	public void changeDialogueBox(String boxtype){
		SetDialogueBox(boxtype);
	}

	[Yarn.Unity.YarnCommand("showitemmenu")]
	public void showItemMenu(){
		if (itemMenu != null){
			itemMenu.SetActive(true);
			ManagerScript.man.givingItems = true;
		}
	}

}
