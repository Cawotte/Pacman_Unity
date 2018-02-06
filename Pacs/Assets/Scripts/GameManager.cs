using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static int Score;
    public static int numPellet;
    public Text ScoreText;
    // Use this for initialization
    void Start () {
        Score = 0;
        numPellet = GameObject.Find("PapaPellets").transform.childCount;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //ScoreText.text = "Score = " + GameManager.Score + "\n Nombre de pellets restantes :" + numPellet;
        ScoreText.text = "DATA :"+
               "\n\tREMAINING PELLETS : " + numPellet +
               "\n\tPELLETS EATEN : " + Score +
               "\n\tSCORE : " + Score * 10;

    }

    void Foo()
    {
        print("Salut");
    }
}


