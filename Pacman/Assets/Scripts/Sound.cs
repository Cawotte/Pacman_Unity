
using UnityEngine.Audio;
using UnityEngine;

//Permet de modifier les attributs de Sound dans l'inspecteur même lorsuqu'il est dans l'array de l'audioManager
[System.Serializable]

/*
 * Classe utilisé pour caractérisiter un son, n'importe quel pist audio qui va être joué.
 * L'audioManager contient un Array de 'Sound' qui vont chacun contenir un son.
 * 
 * */
public class Sound  {

    //Nom du son
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    //composant qui jouera le son
    public AudioSource source;


}
