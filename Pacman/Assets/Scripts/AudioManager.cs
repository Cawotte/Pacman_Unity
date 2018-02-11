using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    /* L'AudioManager, il contient tout les sons qui sont joués, il les stock dans 
     * un Array d'objets de type Sound (une autre classe que j'ai défini).
     * Cela permet de pouvoir plus facilement jouer/arrêter tout les sons.
     * 
     * Les classes utilisants des sons récupèrent l'objet Sound approprié dans le tableau, et interagissent directement avec, car tout ces attributs sont en public.
     * 
     * On remplit l'Array de son dans l'inspecteur d'Unity, qui permet de définir et assigner des valeurs à des variables directement depuis l'interface d'Unity, plus intuitif que l'initialisation logiciel.
     * 
     * Il y a un second array de Sounds pour les musiques, ainsi on peut facilement arrêter les bruitages qui sont dans Sound tout en gardant la musique, utile pour la pause par exemple.
     * 
     * */

        /* Détails amusants :
         * - Les bruitages des mouvements des fantomes et de Pacman ont été fait par moi-même, au micro et à la bouche ! ;)
         * - Les musiques utilisés sont libre de droits.
         * */
    //Sons
    public Sound[] sounds;

    //Musique
    public Sound[] musics;
    int num_music;
    public Sound music;

    
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

        //Sound.source est un composant AudioSource, c'est le composant qui va jouer la musique. Pour chaque Sons dans sounds[], on 
        //initialise ici son AudioSource avec les valeurs dans de l'objet Sound s.
		foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        num_music = UnityEngine.Random.Range(0, musics.Length);

        //On initialise la musique de la même manière que pour le son, sauf qu'il y en a qu'une ici.
        music.source = gameObject.AddComponent<AudioSource>();
        music.source.clip = musics[num_music].clip;
        music.source.volume = musics[num_music].volume;
        music.source.pitch = musics[num_music].pitch;
        music.source.Play();

	}
	 
    //Renvoie l'objet Sound ayant le nom donné en paramètres. Utilisé par les autres scripts pour récupérer les sons qu'ils veulent jouer.
    public Sound Find(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Son:" + name + " non trouvé!");
        }
        return s;
    }
    
    //Coupe le son de tout les bruitages.
    public void MuteAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.mute = true;
    }

    //Rétablit le son de tout les bruitages.
    public void UnMuteAllSounds()
    {
        //On vérifie que le jeu n'est pas en pause et que le bouton qui gère l'activation des bruitages est bien allumé.
        //On ne veut pas que les bruitages continuent d'être joué quand le jeu est en pause.
        //On ne veut pas que les bruitages reprennent quand on reprend la partie si on les avait coupé.
        if (!GameManager.getInstance().gamePaused && GameObject.Find("Audio").GetComponent<Buttons_Behaviour>().On)
        {
            foreach (Sound s in sounds)
            {
                s.source.mute = false;
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

    //Change la musique pour la prochaine dans l'array music.
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

    //On récupère l'instance de AudioManger, pour qu'il soit utilisable plus facilement par les autres classes.
    public static AudioManager getInstance()
    {
        return instance;
    }
}
