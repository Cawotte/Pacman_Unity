using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons_Behaviour : MonoBehaviour {

    //Les deux sprites sur lesquels le bouton va alterner pour signaler qu'il est en position On/Off
    Sprite ImageOn;
    public Sprite ImageOff;

    Button bt;
    bool onOff;
	void Start () {
        ImageOn = GetComponent<Image>().sprite; //Le sprite On est le sprite initial
        bt = GetComponent<Button>(); //On récupère le composant bouton
        onOff = true; //Le bouton est on.
    }
	
    //Change l'état du bouton ( On / Off ) ainsi que son sprite.
    public void OnOff()
    {
        if ( onOff ) {
            onOff = false;
            GetComponent<Image>().sprite = ImageOff;
        }
        else {
            onOff = true;
            GetComponent<Image>().sprite = ImageOn;
        }
    }

    public void audio_onOff()
    {
        AudioSource sfx = GameObject.Find("Pacman").GetComponent<AudioSource>();
        if (onOff) {
            sfx.enabled = true;
            sfx.Play();
        }
        else {
            sfx.enabled = false;
        }
    }

    public void pause_onOff()
    {
        if (onOff)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }



}


