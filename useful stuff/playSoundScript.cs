using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSoundScript : MonoBehaviour {

	public AudioClip sound;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		audioSource.PlayOneShot(sound, settingsScript.settings.soundVolume);
	}
}
