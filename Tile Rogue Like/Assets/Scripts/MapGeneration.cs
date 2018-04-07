using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration {

	// for generating a map randomly
	// - unwalkable tiles cannot surround whole map
	// - maximum of 25% of area is unwalkable tiles
	// - unwalkable tiles have similar patterns (buildings, mountain ranges, large rocks, etc.) kind of clumped together


	int mapSizeX;
	int mapSizeY;

	GameObject player;

	enum Tile{Grass, Dirt, Stone};
	Tile grass, dirt, stone;

	public MapGeneration(int x, int y, GameObject _player){
		mapSizeX = x;
		mapSizeY = y;

		grass = Tile.Grass;
		dirt = Tile.Dirt;
		stone = Tile.Stone;

		player = _player;
	}

	public void generateMap(int[,] tiles){

		// maximum amount of unwalkable tiles is 25%
		int maxUnwalkable = Mathf.RoundToInt(tiles.Length * .25f);

		System.Random rnd = new System.Random ();

		int count = 0;

		// initialize all tiles to a walkable tile (grass)
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				tiles [x, y] = (int)grass;

				int randomNumber = rnd.Next (1, 100);

				if (randomNumber < 50) {
					tiles [x, y] = (int)dirt;
				}
				if (randomNumber > 90 && count < maxUnwalkable && (x != player.transform.position.x && y != player.transform.position.y)) {
					tiles [x, y] = (int)stone;
					count++;
				}
			}
		}
	}

}
