using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
  {
  public static audioManager audioman;
  public float fadeTime = 1.0f;

  AudioSource music;
  
  public Music[] trackList;
  public Dictionary<string, Music> trackOfScene;

  AudioClip nextTrack;
  musicPlayState state = musicPlayState.fadeIn;

  float musicVolume = 0f;
  float maxVolume = 1.0f;
  
  // Por ahora sólo sigue andando la música
  void Awake(){
    if (audioman == null){
      DontDestroyOnLoad(this.gameObject);
      audioman = this;
      trackOfScene = new Dictionary<string, Music>();
			foreach (Music m in trackList){
				trackOfScene.Add(m.scene, m);
			}
    } else {
      Destroy(gameObject);
      return;
    }

    music = GetComponent<AudioSource>();
  }

  void Update(){
    maxVolume = ManagerScript.coso.musicVolume;
    
    switch(state){
    case musicPlayState.fadeOut:
      fadeOut();
    break;
    case musicPlayState.fadeIn:
      fadeIn();
    break;
    }

    music.volume = musicVolume;
  }

  public void PlayMusicOfScene(string scene, float timeToFade = 1.0f){
    nextTrack = trackOfScene[scene].track;
    state = musicPlayState.fadeOut;
    fadeTime = timeToFade;
  }

  void fadeIn(){
    if (musicVolume < maxVolume){
      musicVolume += Time.deltaTime / fadeTime;
    } else {
      musicVolume = maxVolume;
      state = musicPlayState.playing;
    }
  }

  void fadeOut(){
    if (musicVolume > 0f){
      musicVolume -= Time.deltaTime / fadeTime;
    } else {
      musicVolume = 0f;
      state = musicPlayState.fadeIn;
      if (nextTrack != null){
        music.clip = nextTrack;
        music.Play();
      }
    }
  }
}

public enum musicPlayState{
  playing = 0,
  fadeIn = 1,
  fadeOut = 2
}

[System.Serializable]
public class Music {
  public string scene;
  public AudioClip track;

  public Music(string theScene, AudioClip theTrack){
    scene = theScene;
    track = theTrack;
  }
};
