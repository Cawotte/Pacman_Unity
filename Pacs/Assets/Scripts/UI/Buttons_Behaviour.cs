using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons_Behaviour : MonoBehaviour {

    //Les deux sprites sur lesquels le bouton va alterner pour signaler qu'il est en position On/Off
    Sprite ImageOn;
    public Sprite ImageOff;



    public GameObject PausePanel;

    GameManager game_manager;

    //Button bt;
    [HideInInspector]
    public bool On;
	void Start () {

        ImageOn = GetComponent<Image>().sprite; //Le sprite On est le sprite initial

        On = true; //Le bouton est on.

        game_manager = GameManager.getInstance();
    }
	
    //Change l'état du bouton ( On / Off ) ainsi que son sprite.
    public void OnOff()
    {

        //Si le jeu est en pause, on ne fait rien
        //if (game_manager.gamePaused)
        //    return;

        if ( On ) {
            On = false;
            GetComponent<Image>().sprite = ImageOff;
        }
        else {
            On = true;
            GetComponent<Image>().sprite = ImageOn;
        }
    }

    //Eteint le son
    public void sfx_OnOff()
    {
        //Si le jeu est en pause, on ne fait rien
        if (game_manager.gamePaused)
            return;

        if (!On)
            AudioManager.getInstance().MuteAllSounds();
        else
            AudioManager.getInstance().UnMuteAllSounds();
    }

    public void pause_onOff()
    {
        pause(On);
    }

    public void pause(bool onOff)
    {
        AudioManager audio = AudioManager.getInstance();
        if (onOff) //Si le jeu est en pause, on le reprend.
        {
            game_manager.gamePaused = false;
            audio.UnMuteAllSounds();
            Time.timeScale = 1;
            PausePanel.SetActive(false);
        }
        else
        {
            game_manager.gamePaused = true;
            audio.MuteAllSounds();
            Time.timeScale = 0;
            PausePanel.SetActive(true);
        }

    }

    public void music_onOff()
    {
        AudioManager audio = AudioManager.getInstance();
        if (!On)
        {
            audio.music.source.Pause();
        }
        else
        {
            audio.music.source.UnPause();
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }



}


