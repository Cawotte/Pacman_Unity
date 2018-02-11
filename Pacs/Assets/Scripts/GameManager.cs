using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    /* Role de la classe GameManager :
     * 
     * Gérer le Timer qui commande l'état actuelle des fantomes, qui définit leur comportement, voir les classes Ghost_movements
     * et les classes propres à chaque fantomes pour plus de détails sur leur comportement.
     * 
     * Affiche les scores.
     * 
     * 
     * 
     * **/

    //Singleton pattern
    public static GameManager instance;


    public static int Score; //Contient le score.
    public static int numPellet; //Mémorise le nombre de pac-gommes restantes

    //UI
    public Text ScoreText;
    public Text MouseText;

    //Etat dans lequel doit être tout les fantomes. Il est en static pour que le script "Pacman_collisions" puisse facilement y accéder pour modifier l'état
    //lorsquu'n super point est mangé.
    [HideInInspector]
    public int state;
    public bool frightened;

    //Des constantes avec le temps de chaque mode.
    public const float DUREE_CHASE = 20.0f;
    public const float DUREE_SCATTER = 7.0f;
    public const float DUREE_AFRAID = 7.0f;
    public const float DUREE_MORT = 3.0f;

    [HideInInspector]
    public float timeLeft; //Notre timer.

    //On va lister le script de chaque fantomes dans cette liste pour pouvoir ensuite appeller les méthodes permettant d'interagir avec eux.
    Component[] GhostScripts;

    [HideInInspector]
    public bool gamePaused = false;


    public GameObject GameOverPanel;
    public Text gameOverText;
    public GameObject VictoryPanel;
    public Text victoryText;


    void Awake ()
    {

        //Singleton
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        //Init des scores et nombre de points à récolter.
        numPellet = GameObject.Find("PapaPellets").transform.childCount;
        Score = 0;

        //Etat de base des fantomes
        timeLeft = DUREE_CHASE;
        state = 1;
        frightened = false;

        //On récupère le script des fantome
        GhostScripts = GameObject.Find("Fantomes").GetComponentsInChildren<Ghost_movements>();

        //gamePaused = false;


    }

	
	// Update is called once per frame
	void Update () {

        if (Pacman_movements.nbVies <= 0) //Si pacman n'a plus de vie : Game Over
            gameOver();
        if (numPellet == 0) //Si pacman a mangé tout les points
            victory();


        //Chronomètre des modes des fantomes :
            //On diminue timeLeft en fonction du temps actuelle, sa valeur varie entre les Update().
            //si TimeLeft est égale à 20.0, alors Timeleft sera égale à 0 ou moins au bout de 20s.
        timeLeft -= Time.deltaTime; 

        /* l'etat des fantomes :
         * 0 = Mort
         * 1 = Chase
         * 2 = Scatter
         * 3 = Frightened
         * */

        //Lorsque le timer se termine, on change le mode des fantomes.
        if (timeLeft <= 0.0f)
        {
            
            foreach (Ghost_movements fantome in GhostScripts)
            {
                //Si on passe de Scatter à Chase ou vice-versa, les fantomes font demi-tour.
                if (state == 1 || state == 2)
                    fantome.faireDemiTour();
                if (state == 3) //Si les fantomes étaient effrayés, on les remets dans leur état normal.
                    fantome.nestPlusEffraye();
            }

            //On change d'état et on réinitialise le compteur.
            switch (state)
            {
                case 1:  //On repasse en Scatter a la fin du mode Chase
                    state++;
                    timeLeft = DUREE_SCATTER;
                    break;
                case 2: //On repasse en Chase a la fin du mode Scatter
                    state--;
                    timeLeft = DUREE_CHASE;
                    break;
                case 3:
                    state = 1; //On repasse en Chase a la fin du mode Frightened
                    timeLeft = DUREE_CHASE;
                    frightened = false;
                    break;
            }

            
            //On affecte les valeurs de l'état des fantomes en conséquences.
            foreach (Ghost_movements fantome in GhostScripts)
                fantome.setState(state);

        }

        //Si Pacman viens de manger une super boulette, on effraie tout les fantomes.


        //Affichage mode
        if ( state == 1 )
            MouseText.text = "Mode Chase !";
        else if ( state == 2)
            MouseText.text = "Mode Scatter !";
        else if (state == 3)
            MouseText.text = "Mode Frightened !";
        

        string vies = "";
        for (int i = 0; i < Pacman_movements.nbVies; i++)
            vies += " X ";
        ScoreText.text = "Score : " + Score +
            "\nNombre de Pac-Gommes\nrestantes :" + numPellet +
            "\nVies Restantes : " + vies ;
    }


    // Effraie tout les fantomes, appelé par Pacman_collisions, lorsque Pacman mange un super pac-gomme.
   public void Frighten()
   {
        timeLeft = DUREE_AFRAID;
        state = 3;
        foreach (Ghost_movements fantome in GhostScripts)
        {
            fantome.setState(state);
            fantome.deviensEffraye();
        }
    }

    //Affiche l'écran de GameOver
    public void gameOver()
    {
        gamePaused = true;
        AudioManager.getInstance().MuteAllSounds();
        GameOverPanel.SetActive(true);
        //GameObject.Find("GameOverButton").GetComponent<Buttons_Behaviour>().pause(false);
        gameOverText.text =
            "GAME OVER !\n" +
            "Vous avez perdu \ntoutes vos vies !\n" +
            "Vous avez  obtenu  un score de " + Score + ".\n" +
            "\nCliquez ICI pour quitter !";
        Time.timeScale = 0;
    }

    //Affiche l'écran de victoire
    public void victory()
    {
        gamePaused = true;
        AudioManager.getInstance().MuteAllSounds();
        VictoryPanel.SetActive(true);
        //GameObject.Find("VictoryButton").GetComponent<Buttons_Behaviour>().pause(false);
        victoryText.text =
            "Bravo ! \n" +
            "Vous avez mangee\n" +
            "tout les Pac-gommes !\n\n" +
            "Vous avez obtenu un score  total de  " + Score + ".\n\n" +
            "Cliquez ICI pour quitter !";
        Time.timeScale = 0;

    }
    public static GameManager getInstance()
    {
        return instance;
    }

}


