using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EatablePellets : MonoBehaviour {
    

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {
		

	}
    

    void OnTriggerEnter2D (Collider2D coll) {

        if (gameObject.tag == "Player" && coll.gameObject.tag == "Pellet" )
        {
            GameManager.Score++;
            GameManager.numPellet--;
            Destroy(coll.gameObject);
        }

        if (coll.gameObject.tag == "Player" && gameObject.tag == "Monster")
        {
            GameObject.Find("Pacman").GetComponent<Transform>().position = new Vector3(1, -9, 0);
        }
    }
}
