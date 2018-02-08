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

    public static int Score; //Contient le score.
    public static int numPellet; //Mémorise le nombre de pac-gommes restantes

    //UI
    public Text ScoreText;
    public Text MouseText;

    //Etat dans lequel doit être tout les fantomes. Il est en static pour que le script "Pacman_collisions" puisse facilement y accéder pour modifier l'état
    //lorsquu'n super point est mangé.
    public int state;
    public bool frightened;

    //Des constantes avec le temps de chaque mode.
    public const float DUREE_CHASE = 20.0f;
    public const float DUREE_SCATTER = 7.0f;
    public const float DUREE_AFRAID = 7.0f;
    public const float DUREE_MORT = 3.0f;
    float timeLeft; //Notre timer.

    //On va lister le script de chaque fantomes dans cette liste pour pouvoir ensuite appeller les méthodes permettant d'interagir avec eux.
    Component[] GhostScripts;



    void Start ()
    {
        //Init des scores et nombre de points à récolter.
        numPellet = GameObject.Find("PapaPellets").transform.childCount;
        Score = 0;

        //Etat de base des fantomes
        timeLeft = DUREE_CHASE;
        state = 1;
        frightened = false;

        //On récupère le script des fantome
        GhostScripts = GameObject.Find("Fantomes").GetComponentsInChildren<Ghost_movements>();


    }

	
	// Update is called once per frame
	void Update () {

        //Chronomètre des modes des fantomes :
            //On diminue timeLeft en fonction du temps actuelle, sa valeur varie entre les Update().
            //si TimeLeft est égale à 20.0, alors Timeleft sera égale à 0 ou moins au bout de 20s.
        timeLeft -= Time.deltaTime; 


        //Lorsque le timer se termine, on change le mode des fantomes.
        if (timeLeft <= 0.0f)
        {
            
            foreach (Ghost_movements fantome in GhostScripts)
            {
                if (state == 1 || state == 2)
                    fantome.faireDemiTour();
                if (state == 3)
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

        //ScoreText.text = "Score = " + GameManager.Score + "\n Nombre de pellets restantes :" + numPellet;
        /*
        ScoreText.text = "DATA :"+
               "\n\tREMAINING PELLETS : " + numPellet +
               "\n\tPELLETS EATEN : " + Score +
               "\n\tSCORE : " + Score * 10;
        */

        ScoreText.text = "Score : " + Score +
            "\nNombre de Pac-Gommes\nrestantes :" + numPellet +
            "\nVies Restantes : " + "X X X";
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
    
}


