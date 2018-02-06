using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky_movements : Ghost_movements {

	// Use this for initialization
	void Start () {


        //init Cell, moving, targetPos et tilemap.
        // + PacTransform, Direction (Right), targetPos (case devant), state 
        //Ghost_movements.Start();
        base.Start();
        Init();

        ScatterPos = new Vector3Int(-14, 13, 0);


        Debug.Log("Start.Blinky_mov", gameObject);

    }
	
	// Update is called once per frame
	void Update () {

        //Mets à jour sa propre position dans Cell et celle du Pacman pour le poursuivre.
        updateCell();
        updatePacPos();

        if (estDansSpawn())
            sortirSpawn();
        else
            poursuivre(PacmanPos);

    }
}
