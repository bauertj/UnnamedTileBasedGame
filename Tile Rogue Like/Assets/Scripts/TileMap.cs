using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TileMap : MonoBehaviour {

	// array to keep track of different types of tiles
	public TileType[] tileTypes;

	int[,] tiles;
	Node[,] graph;

	// current shortest path
	List<Node> currentPath = null;

	// width size of map
	public int mapSizeX;
	// height of map
	public int mapSizeY;

	// game object keeping track of the player
	public GameObject player;

	// game objects to keep track of game over and win screen
	public GameObject gameOver;
	public GameObject winScreen;
	public GameObject pauseMenu;
	public GameObject playerPanel;
	public GameObject controlPanel;
	public GameObject inventoryPanel;
	//Inventory inventory;

	// the enemy that will spawn
	public GameObject[] enemyArray;
	List<GameObject> enemy = null;

	// how many enemies will spawn
	public int maxEnemies;
	int numberOfEnemies;

	// keeps track of score
	public Text experienceCounter;
	int experience = 0;

	// always set to true unless the game is paused, game is over, etc.
	bool canMove = true;
	bool paused = false;

	// enumerator for different tile types
	enum Tile{Grass, Dirt, Stone};

	// a list of gameobjects that can be picked up from the ground
	public List<GameObject> pickupList;

	// Text objects for text areas that show hit or miss descriptions
	Text hitOrMiss, enemyHit;

	// map generation object
	MapGeneration mapGenerator;

	// boolean to determine if the player is in a menu where they are choosing
	// i.e. which direction to shoot 
	bool choosing = false;

	List<MapCoordinates> mapCoordinates = null;
	int playerTotalPositionX = 0;
	int playerTotalPositionY = 0;

	// keeps track of how many turns have passed
	public int amountOfTurns = 0;


	void Start () {
		init ();
		GenerateMapVisuals ();
	}

	void Update(){
		if (!choosing)
			getMovements ();
		else
			canShoot ();

	}

	/*
	 * this is where all variables are initialized
	 * */
	void init(){
		// a list of enemies
		enemy = new List<GameObject> ();

		mapCoordinates = new List<MapCoordinates> ();

		hitOrMiss = GameObject.Find ("HitOrMiss").GetComponent<Text> ();
		enemyHit = GameObject.Find ("EnemyHit").GetComponent<Text> ();

		playerPanel.transform.position = new Vector3(Screen.width / 2, Screen.height/2, 0);

		experienceCounter.text = "Experience: " + experience;

		numberOfEnemies = maxEnemies;

		// set all extra panels to off
		gameOver.SetActive (false);
		winScreen.SetActive (false);
		pauseMenu.SetActive (false);

		// the size of the map instantiated
		tiles = new int[mapSizeX, mapSizeY];

		// creates instance of a map generator
		mapGenerator = new MapGeneration (mapSizeX, mapSizeY, player);

		//inventory = GameObject.Find ("Inventory").GetComponent<Inventory> ();
	}


	/* 
	 * separate class Node which represents each tile on the map
	 * each node has a list of neighbors which are also nodes
	 * */
	public class Node{
		public List<Node> neighbors;
		public int x;
		public int y;

		public Node(){
			neighbors = new List<Node>();
		}

		public float DistanceTo(Node n){
			return Vector2.Distance (
				new Vector2 (x, y),
				new Vector2 (n.x, n.y));
		}
	}


	/*
	 * populates 2d array graph
	 * then creates a path that will go to a target
	 * */
	void GeneratePathfindingGraph(){
		graph = new Node[mapSizeX, mapSizeY];
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				graph [x, y] = new Node ();
				graph [x, y].x = x;
				graph [x, y].y = y;
			}
		}
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				// checks to see if the tile is walkable
				// if so, will add the coordinates to the graph allowing the enemy to move
				if (tileTypes [tiles [x, y]].isWalkable) {
					if (x > 0)
						graph [x, y].neighbors.Add (graph [x - 1, y]);
					if (x < mapSizeX - 1)
						graph [x, y].neighbors.Add (graph [x + 1, y]);
					if (y > 0)
						graph [x, y].neighbors.Add (graph [x, y - 1]);
					if (y < mapSizeY - 1)
						graph [x, y].neighbors.Add (graph [x, y + 1]);
				}
			}
		}
	}
		

	/*
	 * with the tile array created, this will instantiate all the different tile types on the screen
	 * */
	void GenerateMapVisuals(){
		mapGenerator.generateMap (tiles);
		GameObject mapParent = (GameObject) Instantiate (new GameObject ("MapParent"));
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				TileType tt = tileTypes [tiles [x, y]];
				GameObject go = (GameObject) Instantiate (tt.tilePrefab, new Vector3 (x, y, 0), Quaternion.identity);
				go.name = tt.name + " " + x + " " + y;
				go.transform.parent = mapParent.transform;
				TilePositions tp = go.GetComponent<TilePositions> ();
				tp.tileX = x;
				tp.tileY = y;
			}
		}
		// add the newly generated map to the list of the total map
		MapCoordinates newMap = new MapCoordinates(mapParent, playerTotalPositionX, playerTotalPositionY) ;
		mapCoordinates.Add (newMap);
		System.Random rand = new System.Random ();

		// spawn all enemies here
		// instantiate the enemy and save as game object
		for (int i = 0; i < maxEnemies; i++) {	
			for (int j = 0; j < enemyArray.Length; j++) {
				int randomX = rand.Next (0, mapSizeX);
				int randomY = rand.Next (0, mapSizeY);
				// enemies will spawn on a random tile in the map
				if (tileTypes [tiles [randomX, randomY]].isWalkable && (randomX != player.transform.position.x && randomY != player.transform.position.y)) {
					GameObject enemySpawn = (GameObject)Instantiate (enemyArray [j], new Vector3 (randomX, randomY, 0), Quaternion.identity); 
					enemy.Add (enemySpawn);
					enemySpawn.transform.parent = mapParent.transform;
				} else {
					j--;
				}
			}
		}
		numberOfEnemies = enemy.Count;
		GeneratePathfindingGraph ();
		for (int i = 0; i < pickupList.Count (); i++) {
			int randomX = rand.Next (0, mapSizeX);
			int randomY = rand.Next (0, mapSizeY);
			if (tileTypes [tiles [randomX, randomY]].isWalkable && (randomX != player.transform.position.x && randomY != player.transform.position.y)) {
				GameObject item = (GameObject) Instantiate (pickupList.ElementAt (i), new Vector3 (randomX, randomY, -0.5f), Quaternion.identity);
				item.transform.parent = mapParent.transform;
			} else {
				i--;
			}
		}
	}

	/*
	 * this will find the coordinates of a tile on the field given a name
	 * */
	int[] findCoordinates(string name){
		int[] coords = new int[2];
		string value = "";
		for (int i = 0; i < name.Length; i++) {
			if (System.Char.IsDigit (name.ElementAt (i))) {
				value += name.ElementAt (i);
			} else if (name.ElementAt (i).Equals (' ')) {
				System.Int32.TryParse (value, out coords [0]);
				value = "";
			}
		}
		System.Int32.TryParse (value, out coords [1]);
		return coords;
	}

	/*
	 * this will update the tiles array so the player can navigate the map properly
	 * */
	void updateMap(){
		for (int i = 0; i < mapCoordinates.Count (); i++) {
			if (mapCoordinates.ElementAt (i).x == playerTotalPositionX && mapCoordinates.ElementAt (i).y == playerTotalPositionY) {
				foreach(Transform child in mapCoordinates.ElementAt(i).map.transform){
					if (child.name.Contains ("grass")) {
						int[] tileCoords = findCoordinates (child.name);
						tiles [tileCoords [0], tileCoords [1]] = 0;
					} else if (child.name.Contains ("dirt")) {
						int[] tileCoords = findCoordinates (child.name);
						tiles [tileCoords [0], tileCoords [1]] = 1;
					} else if (child.name.Contains ("stone")) {
						int[] tileCoords = findCoordinates (child.name);
						tiles [tileCoords [0], tileCoords [1]] = 2;
					}
				}
			}
		}
	}
		
	/*
	 * translates the player on the screen
	 * given an x and y direction
	 * */
	void movePlayer(int x, int y){
		// checks to see if the player is still alive before allowing to move
		if (player.GetComponent<Player> ().alive () && canMove) {
			Transform playerPos = player.transform;
			bool hit = false;
			// counts which enemy the player is attacking
			int enemyCounter = 0;
			foreach (GameObject t in enemy) {
				if (playerPos.position.x + x == t.transform.position.x && playerPos.position.y + y == t.transform.position.y) {
					int damageDone = player.GetComponent<Player> ().doDamage ();
					if (damageDone == 0) {
						hitOrMiss.text = "Player: Miss";
					} else {
						hitOrMiss.text = "Player: Hit for " + damageDone + " damage.";
					}
					t.GetComponent<Enemy> ().doDamage (damageDone);
					// if the enemy's health gets to 0, destroy the gameobject and remove from the list
					if (t.GetComponent<Enemy> ().currentHealth <= 0) {
						enemyDeath (enemyCounter);
						break;
					}
					hit = true;
				}
				enemyCounter++;
			}
			// if the player does not hit the enemy, continue moving
			if (!hit) {
				playerPos.Translate (x, y, 0);

				//playerPos.transform.position = Vector3.MoveTowards(playerPos.transform.position, new Vector3(x, y, 0), .3f * Time.deltaTime);
			}
		}
		amountOfTurns++;
		if (amountOfTurns % 3 == 0) {
			player.GetComponent<Player> ().regainHealth (10);
		}
	}

	/*
	 * handles the death of an enemy 
	 * */
	void enemyDeath(int enemyPosition){
		enemy.RemoveAt (enemyPosition);
		numberOfEnemies--;
		experience++;
		experienceCounter.text = "Experience: " + experience;
	}

	/*
	 * will find enemies on the field and add to a list
	 * this will help keep track of what enemies are alive
	 * */
	void findEnemies(Transform parent){
		enemy.Clear ();
		for (int i = 0; i < parent.childCount; i++) {
			if (parent.GetChild (i).tag.Equals ("Enemy")) {
				enemy.Add (parent.GetChild(i).gameObject);
			}
		}
		numberOfEnemies = enemy.Count ();
	}

	/*
	 * given an x and y direction, this handles when the player is going to move off the map
	 * */
	void moveOffTheMap(int x, int y){
		if (canMove) {
			int xToMove = 0;
			int yToMove = 0;
			Transform playerPos = player.transform;
			GameObject currentMap = null;
			playerTotalPositionX += x;
			playerTotalPositionY += y;

			bool canMove = true;
			if (x != 0) {
				yToMove = (int)playerPos.position.y;
				if (x == -1)
					xToMove = mapSizeX - 1;
			}
			if (y != 0) {
				xToMove = (int)playerPos.position.x;
				if (y == -1)
					yToMove = mapSizeY - 1;
			}


			// loop through the maps that have already been created
			for (int i = 0; i < mapCoordinates.Count (); i++) {
				// set the current map to false
				if ((mapCoordinates.ElementAt (i).x == (playerTotalPositionX - x)) && (mapCoordinates.ElementAt (i).y == (playerTotalPositionY - y))) {
					currentMap = mapCoordinates.ElementAt (i).map;
					currentMap.SetActive (false);
					enemy.Clear ();
				}
			}
			// if the map has not already been visited
			if (!(mapCoordinates.Any (findMap => (findMap.x == playerTotalPositionX) && (findMap.y == playerTotalPositionY)))) {
				// generate a new map
				GenerateMapVisuals ();
				// loop through all created maps
				for (int i = 0; i < mapCoordinates.Count (); i++) {
					if ((mapCoordinates.ElementAt (i).x == playerTotalPositionX) && (mapCoordinates.ElementAt (i).y == playerTotalPositionY)) {
						// if the player is able to walk on next tile, go to next map
						// otherwise, the player will not be allowed to move
						if (tileTypes [tiles [xToMove, yToMove]].isWalkable) {
							player.GetComponent<Player> ().findObjects ();
							playerPos.position = new Vector3 (xToMove, yToMove, 0);
							
						} else {
							playerTotalPositionX -= x;
							playerTotalPositionY -= y;
							currentMap.SetActive (true);
							updateMap ();
							hitOrMiss.text = "The path is blocked here.";
						}
					}
				}
			}
		// if the map has already been visited
		else {
				// loop through the maps that have already been created
				for (int i = 0; i < mapCoordinates.Count (); i++) {
					if ((mapCoordinates.ElementAt (i).x == playerTotalPositionX) && (mapCoordinates.ElementAt (i).y == playerTotalPositionY)) {
						// updates the map to get tile array current
						updateMap ();
						findEnemies (mapCoordinates.ElementAt (i).map.transform);
						for (int j = 0; j < enemy.Count (); j++) {
							if (enemy.ElementAt (j).transform.position.x == xToMove && enemy.ElementAt (j).transform.position.y == yToMove) {
								canMove = false;
							}
						}

						if (tileTypes [tiles [xToMove, yToMove]].isWalkable && canMove) {
						
							mapCoordinates.ElementAt (i).map.SetActive (true);
							playerPos.position = new Vector3 (xToMove, yToMove, 0);
							player.GetComponent<Player> ().findObjects ();
							findEnemies (mapCoordinates.ElementAt (i).map.transform);

							GeneratePathfindingGraph ();
						} else {
							playerTotalPositionX -= x;
							playerTotalPositionY -= y;
							currentMap.SetActive (true);
							updateMap ();
							findEnemies (currentMap.transform);
							hitOrMiss.text = "The path is blocked here.";
							break;
						}
					}
				}
			}
		}
	}

	void print(){
		for (int i = 0; i < mapCoordinates.Count (); i++) {
			Debug.Log ("(" + mapCoordinates.ElementAt (i).x + ", " + mapCoordinates.ElementAt (i).y + ")");
		}
	}

	/*
	 * this will take care of all controls the player is given
	 * */
	void getMovements(){
		bool movement = false;
		Transform playerPos = player.transform;
		if (Input.GetKeyDown (KeyCode.F2)) {
			print();
		}
		if (Input.GetKeyDown (KeyCode.W)) {
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
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			if (!(playerPos.position.y <= 0) && tileTypes [tiles [(int)playerPos.position.x, (int)playerPos.position.y - 1]].isWalkable) {
				movePlayer (0, -1);
				movement = true;
			}
			// check if player is walking south off map
			else if (playerPos.position.y - 1 < 0) {
				moveOffTheMap (0, -1);
			}
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			if (!(playerPos.position.x >= (mapSizeX - 1) ) && tileTypes[tiles[(int)playerPos.position.x + 1, (int)playerPos.position.y]].isWalkable) {
				movePlayer (1, 0);
				movement = true;
			}
			// check if player is walking off the map
			else if (playerPos.position.x + 1 >= mapSizeX) {
				moveOffTheMap (1, 0);
			}
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			if (!(playerPos.position.x <= 0) && tileTypes[tiles[(int)playerPos.position.x - 1, (int)playerPos.position.y]].isWalkable) {
				movePlayer (-1, 0);
				movement = true;
			}
			// check if player is walking off the map
			else if (playerPos.position.x - 1 < 0) {
				moveOffTheMap (-1, 0);
			}
		}
		// input for skipping a turn
		if (Input.GetKeyDown (KeyCode.T)) {
			amountOfTurns++;
			if (amountOfTurns % 3 == 0) {
				player.GetComponent<Player> ().regainHealth (10);
			}
			movement = true;
		}
		// shoot an arrow
		if (Input.GetKeyDown (KeyCode.E)) {
			if(player.GetComponent<Player>().getArrows() > 0)
				choosing = true;
		}
		// character menu
		if (Input.GetKeyDown (KeyCode.C)) {
			if (playerPanel.activeSelf)
				playerPanel.SetActive (false);
			else
				playerPanel.SetActive (true);
		}
		// inventory menu
		if (Input.GetKeyDown (KeyCode.I)) {
			if (inventoryPanel.activeSelf)
				inventoryPanel.SetActive (false);
			else if(!paused)
				inventoryPanel.SetActive(true);
		}
		// pause menu
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (paused) {
				pauseMenu.SetActive (false);
				canMove = true;
				paused = false;
			} else {
				pauseMenu.SetActive (true);
				canMove = false;
				paused = true;
			}


			Button cont = pauseMenu.transform.Find ("Continue").GetComponent<Button> ();
			cont.onClick.AddListener (continueGame);

		}
		if (Input.GetKeyDown (KeyCode.F1)) {
			if (!controlPanel.activeSelf)
				controlPanel.SetActive (true);
		}
		// signifies end of turn
		if (movement) {
			aiTurn ();
		}
	}

	/*
	 * listener for continue button
	 * */
	void continueGame(){
		pauseMenu.SetActive (false);
		canMove = true;
		paused = false;
	}

	/*
	 * awaits direction from the player when wanting to shoot an arrow
	 * */
	void canShoot(){
		canMove = false;
		hitOrMiss.text = "Which direction will you shoot?";
		if (Input.GetKeyDown (KeyCode.Escape)) {
			hitOrMiss.text = "";
			canMove = true;
			choosing = false;
		}
		if (Input.GetKeyDown (KeyCode.W)) 
			rangeWeapon (0, 1);
		if (Input.GetKeyDown (KeyCode.S))
			rangeWeapon (0, -1);
		if (Input.GetKeyDown (KeyCode.A))
			rangeWeapon (-1, 0);
		if (Input.GetKeyDown (KeyCode.D))
			rangeWeapon (1, 0);
	}

	/*
	 * handles shooting an arrow, checking for an enemy
	 * */
	void rangeWeapon(int x, int y){
		int positionInList = 0;
		GameObject enemyToShoot = null;
		// if the player is shooting in the x direction...
		if (x != 0) {
			// if the player is shooting to the right...
			if (x == 1) {
				// loop through all enemies on map and see if there are any that are too the right and add to the list
				for (int i = 0; i < numberOfEnemies; i++) {
					if (enemy.ElementAt (i).transform.position.x > player.transform.position.x && enemy.ElementAt(i).transform.position.y == player.transform.position.y) {
						if (enemyToShoot == null) {
							enemyToShoot = enemy.ElementAt (i);
							positionInList = i;
						}
						if (enemyToShoot != null && enemy.ElementAt (i).transform.position.x < enemyToShoot.transform.position.x) {
							enemyToShoot = enemy.ElementAt (i);
							positionInList = i;
						}
					}
				}
				// looks at tiles to the right of the player
				if (enemyToShoot != null) {
					for (int i = (int)player.transform.position.x + 1; i < enemyToShoot.transform.position.x; i++) {
						if (!tileTypes [tiles [i, (int)player.transform.position.y]].isWalkable) {
							choosing = false;
							canMove = true;
							player.GetComponent<Player> ().setArrows (-1);
							return;
						}
					}
					enemyToShoot.GetComponent<Enemy> ().doDamage (player.GetComponent<Player> ().arrowDamage);
					if (enemyToShoot.GetComponent<Enemy> ().currentHealth <= 0) {
						enemyDeath (positionInList);
					}
					hitOrMiss.text = "Nice shot! You did " + player.GetComponent<Player> ().arrowDamage + " damage";
				} else {
					choosing = false;
					canMove = true;
					aiTurn ();
					hitOrMiss.text = "You hit nothing.";
				}

				player.GetComponent<Player> ().setArrows (-1);
			}
			if (x == -1) {
				for (int i = 0; i < numberOfEnemies; i++) {
					if (enemy.ElementAt (i).transform.position.x < player.transform.position.x && enemy.ElementAt(i).transform.position.y == player.transform.position.y) {
						if (enemyToShoot == null) {
							enemyToShoot = enemy.ElementAt (i);
							positionInList = i;
						}
						if (enemyToShoot != null && enemy.ElementAt (i).transform.position.x > enemyToShoot.transform.position.x) {
							enemyToShoot = enemy.ElementAt (i);
							positionInList = i;
						}
					}
				}
				// looks at tiles to the left of the player
				if (enemyToShoot != null) {
					for (int i = (int)enemyToShoot.transform.position.x; i < (int)player.transform.position.x - 1; i++) {
						if (!tileTypes [tiles [i, (int)player.transform.position.y]].isWalkable) {
							choosing = false;
							canMove = true;
							player.GetComponent<Player> ().setArrows (-1);
							return;
						}
					}

					enemyToShoot.GetComponent<Enemy> ().doDamage (player.GetComponent<Player> ().arrowDamage);
					if (enemyToShoot.GetComponent<Enemy> ().currentHealth <= 0) {
						enemyDeath (positionInList);
					}
					hitOrMiss.text = "Nice shot! You did " + player.GetComponent<Player> ().arrowDamage + " damage";
				} else {
					choosing = false;
					canMove = true;
					aiTurn ();
					hitOrMiss.text = "You hit nothing.";
				}
				player.GetComponent<Player> ().setArrows (-1);
			}
		}
		if (y != 0) {
			// player shooting up
			if (y == 1) {
				// loop through all enemies on map and see if there are any that are above
				for (int i = 0; i < numberOfEnemies; i++) {
					if (enemy.ElementAt (i).transform.position.y > player.transform.position.y && enemy.ElementAt(i).transform.position.x == player.transform.position.x) {
						if (enemyToShoot == null) {
							enemyToShoot = enemy.ElementAt (i);
							positionInList = i;
						}
						if (enemyToShoot != null && enemy.ElementAt (i).transform.position.y < enemyToShoot.transform.position.y) {
							enemyToShoot = enemy.ElementAt (i);
							positionInList = i;
						}
					}
				}
				// looks at tiles above the player
				if (enemyToShoot != null) {
					for (int i = (int)player.transform.position.y + 1; i < (int)enemyToShoot.transform.position.y; i++) {
						if (!tileTypes [tiles [(int)player.transform.position.x, i]].isWalkable) {
							choosing = false;
							canMove = true;
							player.GetComponent<Player> ().setArrows (-1);
							return;
						}
					}
					enemyToShoot.GetComponent<Enemy> ().doDamage (player.GetComponent<Player> ().arrowDamage);
					if (enemyToShoot.GetComponent<Enemy> ().currentHealth <= 0) {
						enemyDeath (positionInList);
					}
					hitOrMiss.text = "Nice shot! You did " + player.GetComponent<Player> ().arrowDamage + " damage";
				} else {
					choosing = false;
					canMove = true;
					aiTurn ();
					hitOrMiss.text = "You hit nothing.";
				}
				player.GetComponent<Player> ().setArrows (-1);
			}
			if (y == -1) {
				for (int i = 0; i < numberOfEnemies; i++) {
					if (enemy.ElementAt (i).transform.position.y < player.transform.position.y && enemy.ElementAt(i).transform.position.x == player.transform.position.x) {
						if (enemyToShoot == null) {
							enemyToShoot = enemy.ElementAt (i);
							positionInList = i;
						}

						if (enemyToShoot != null && enemy.ElementAt (i).transform.position.y > enemyToShoot.transform.position.y) {
							enemyToShoot = enemy.ElementAt (i);
							positionInList = i;
						}
					}
				}
				// looks at tiles to the left of the player
				if (enemyToShoot != null) {
					for (int i = (int)enemyToShoot.transform.position.y; i < (int)player.transform.position.y - 1; i++) {
						if (!tileTypes [tiles [(int)player.transform.position.x, i]].isWalkable) {
							choosing = false;
							canMove = true;
							player.GetComponent<Player> ().setArrows (-1);
							return;
						}
					}

					enemyToShoot.GetComponent<Enemy> ().doDamage (player.GetComponent<Player> ().arrowDamage);
					if (enemyToShoot.GetComponent<Enemy> ().currentHealth <= 0) {
						enemyDeath (positionInList);
					}
					hitOrMiss.text = "Nice shot! You did " + player.GetComponent<Player> ().arrowDamage + " damage";
				} else {
					choosing = false;
					canMove = true;
					aiTurn ();
					hitOrMiss.text = "You hit nothing.";
				}
				player.GetComponent<Player> ().setArrows (-1);
			}
		}
		choosing = false;
		canMove = true;
		aiTurn ();
	}

	/*
	 * will check the shortest path to the player and move the enemy accordingly
	 * */
	public void aiTurn(){
		if(canMove){
			// will loop and find the best path for every enemy that is in the game
			for (int i = 0; i < numberOfEnemies; i++) {
				Transform enemyPos = enemy[i].transform;
				Transform playerPos = player.transform;
				// Dijkstras algorithm
				Dictionary<Node, float> dist = new Dictionary<Node, float> ();
				Dictionary<Node, Node> prev = new Dictionary<Node, Node> ();
				// list of nodes we have not checked
				List<Node> unvisited = new List<Node> ();
				Node source = graph [(int)enemyPos.position.x, (int)enemyPos.position.y];
				Node target = graph [(int)playerPos.position.x, (int)playerPos.position.y];
				dist [source] = 0;
				prev [source] = null;
				// initialize to infinite distance
				foreach (Node v in graph) {
					if (v != source) {
						dist [v] = Mathf.Infinity;
						prev [v] = null;
					}
					unvisited.Add (v);
				}
				while (unvisited.Count > 0) {
					// u is unvisited node with smallest distance
					Node u = null;
					foreach (Node possibleU in unvisited) {
						if (u == null || dist [possibleU] < dist [u]) {
							u = possibleU;
						}
					}
					if (u == target) {
						break;
					}
					unvisited.Remove (u);
					foreach (Node v in u.neighbors) {
						float alt = dist [u] + u.DistanceTo (v);
						if (alt < dist [v]) {
							dist [v] = alt;
							prev [v] = u;
						}
					}
				}
				// We now have either the shortest route to the target
				// Or there is no route to the target
				if (prev [target] != null) {		
					currentPath = new List<Node> ();
					Node curNode = target;
					while (curNode != null) {
						currentPath.Add (curNode);
						curNode = prev [curNode];
					}
					// currentPath is now a route from our target to our source. 
					// need to invert it
					currentPath.Reverse ();
					// removes first element of list which is the current position of the enemy
					// then grabs the next element which is the location where it should move next
					// finally translates enemy accordingly
					currentPath.Remove (currentPath.First ());
					Node first = currentPath.First ();
					// vector3 of the translation the enemy moves towards the player
					Vector3 moveTo = new Vector3 (first.x - enemyPos.position.x, first.y - enemyPos.position.y, 0);
					// projected coordinates of where enemy will move to
					Vector3 coordinatesToMove = new Vector3 (enemyPos.position.x + moveTo.x, enemyPos.position.y + moveTo.y, 0);
					// true if going to collide with another enemy
					bool col = false;
					// if they run into the player, do not collide
					if (!(coordinatesToMove.x == playerPos.position.x && coordinatesToMove.y == playerPos.position.y)) {
						foreach (GameObject en in enemy) {
							// up
							if (coordinatesToMove.x == en.transform.position.x && coordinatesToMove.y == en.transform.position.y) {
								col = true;
							}
							// down
							if (coordinatesToMove.x == en.transform.position.x && coordinatesToMove.y == en.transform.position.y) {
								col = true;
							}
							// left
							if (coordinatesToMove.x == en.transform.position.x && coordinatesToMove.y == en.transform.position.y) {
								col = true;
							}
							// right
							if (coordinatesToMove.x == en.transform.position.x && coordinatesToMove.y == en.transform.position.y) {
								col = true;
							}
						}
						if (!col) {
							enemy [i].GetComponent<Enemy> ().moveTowardsPlayer (moveTo);
						}
					} else {
						// player is hit, take damage
						int damageDone = player.GetComponent<Player> ().takeDamage (enemy [i].GetComponent<Enemy> ().damage);

						if (damageDone == 0) {
							enemyHit.text = "Enemy: Miss";
						} else {
							enemyHit.text = "Enemy: Hit for " + damageDone + " damage.";
						}
						// if player is dead, turn gameover sign on
						if (!player.GetComponent<Player> ().alive ()) {
							gameOver.SetActive (true);
						}
					}
				} // end if
			} // end for
		}
	} // end method
}