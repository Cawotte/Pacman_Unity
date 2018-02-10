using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    //Sons
    public Sound[] sounds;

    //Musique
    public Sound[] musics;

    int num_music = 0;
    public Sound music;

    //bool gamePaused;
    //Singleton pattern
    public static AudioManager instance;

	void Awake () {

        //Singleton
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }


        DontDestroyOnLoad(gameObject);

		foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        music.source = gameObject.AddComponent<AudioSource>();
        music.source.clip = musics[num_music].clip;
        music.source.volume = musics[num_music].volume;
        music.source.pitch = musics[num_music].pitch;
        music.source.Play();

	}
	 
    public Sound Find(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Son:" + name + " non trouvé!");
        }
        return s;
    }
    
    public void MuteAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.mute = true;
    }

    public void UnMuteAllSounds()
    {

        if (!GameManager.getInstance().gamePaused && GameObject.Find("Audio").GetComponent<Buttons_Behaviour>().On)
        {
            foreach (Sound s in sounds)
            {
                s.source.mute = false;
                s.source.Play();
            }
        }
            
    }
    public void PauseAll()
    {
        foreach (Sound s in sounds)
            s.source.Pause();
        music.source.Pause();
    }
    public void ResumeAll()
    {
        foreach (Sound s in sounds)
            s.source.UnPause();
        music.source.UnPause();
    }

    public void nextMusic()
    {
        if ( musics.Length-1 > num_music )
            num_music++;
        else
            num_music = 0;

        music.source.Stop(); //On stop la musique précédente
        //On la remplace par la nouvelle
        music.source.clip = musics[num_music].clip;
        music.source.volume = musics[num_music].volume;
        music.source.pitch = musics[num_music].pitch;
        music.source.Play();
    }

    public static AudioManager getInstance()
    {
        return instance;
    }
}
