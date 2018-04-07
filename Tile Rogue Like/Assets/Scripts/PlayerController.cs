using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	/*private int mapSizeX, mapSizeY;
	private GameObject player;
	private Transform playerPos;

	private int amountOfTurns = 0;
	private bool canMove = true;

	void Start(){
		mapSizeX = GameObject.Find("Map").GetComponent<TileMap>().mapSizeX;
		mapSizeY = GameObject.Find("Map").GetComponent<TileMap>().mapSizeY;
		player = this.gameObject;
		playerPos = player.transform;
	}

	void Update(){
		if (canMove)
			getMovements ();
	}

	void getMovements(){

		bool movement = false;

		/*if (Input.GetKeyDown (KeyCode.W)) {
			// checks if player is in bounds of the map and if they are going to hit a stone which does not allow them to move
			if (!(playerPos.position.y + 1 >= mapSizeY) && tileTypes [tiles [(int)playerPos.position.x, (int)playerPos.position.y + 1]].isWalkable) {
				// if the player moves, this will become true
				movement = true;
				movePlayer (0, 1);
			} 
			// check if player is walking off the map
			else if (playerPos.position.y + 1 >= mapSizeY) {
				moveOffTheMap (0, 1);
			}
		}*/
	/*}


	void movePlayer(int x, int y){

	}

	void moveOffTheMap(int x, int y){

	}*/

}