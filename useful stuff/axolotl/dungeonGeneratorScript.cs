using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class dungeonGeneratorScript : MonoBehaviour {

	public int width, height;
	Tilemap wall_tmap;
	Tilemap floor_tmap;
	public TileBase[] wall_tiles;
	public TileBase[] floor_tiles;

	public GameObject zombiePrefab;
	public GameObject stairsPrefab;

	public int holeNumber = 5;
	public int zombieNumber = 30;
	public static dungeonGeneratorScript dungeon;

	[HideInInspector]
	public int[,] wallMap;
	[HideInInspector]
	public int[,] floorMap;

	Vector2Int[] clearPoints;

	int[,] GenerateArray(bool empty){
	    int[,] map = new int[width, height];
	    for (int x = 0; x < width; x++){
	        for (int y = 0; y < height; y++){
				map[x, y] = empty ? 0 : 1;
	        }
	    }
	    return map;
	}


	public void RenderMap(int[,] map, Tilemap tilemap, TileBase[] tiles){
	    //Clear the map (ensures we dont overlap)
	    tilemap.ClearAllTiles();
	    //Loop through the width of the map
	    for (int x = 0; x < width ; x++) {
	        //Loop through the height of the map
	        for (int y = 0; y < height; y++){
	            // 1 = tile, 0 = no tile
	            if (map[x, y] > 0) {
	                tilemap.SetTile(new Vector3Int(width/2-x -1, height/2-y -1, 0), tiles[map[x, y]-1]);
	            }
	        }
	    }
	}

	void RandomWalkCave(int[,] map, int floorX, int floorY, int steps){

	    //Determine our required floorAmount
	    int reqFloorAmount = steps;
	    //Used for our while loop, when this reaches our reqFloorAmount we will stop tunneling
	    int floorCount = 0;

	    //Set our start position to not be a tile (0 = no tile, 1 = tile)
	    map[floorX, floorY] = 0;
	    //Increase our floor count
	    floorCount++;

		while (floorCount < reqFloorAmount) {
	        //Determine our next direction
	        int randDir = Random.Range(0, 4);

	        switch (randDir){
	            //Up
	            case 0:
	                //Ensure that the edges are still tiles
	                if ((floorY + 1) < map.GetUpperBound(1) - 1){
	                    //Move the y up one
	                    floorY++;
	                }
	                break;
	            //Down
	            case 1:
	                //Ensure that the edges are still tiles
	                if ((floorY - 1) > 1) {
	                    //Move the y down one
	                    floorY--;
	                }
	                break;
	            //Right
	            case 2:
	                //Ensure that the edges are still tiles
	                if ((floorX + 1) < map.GetUpperBound(0) - 1){
	                    //Move the x to the right
	                    floorX++;
	                }
	                break;
	            //Left
	            case 3:
	                //Ensure that the edges are still tiles
	                if ((floorX - 1) > 1){
	                    //Move the x to the left
	                    floorX--;
	                }
	                break;
	        }

			paintRectangle(map, floorX, floorY, 1, 1, 0);
			floorCount++;
	    }
	}

	void MakeHoles(int[,] map, int number){
		clearPoints = new Vector2Int[number];

		for (int i = 0; i < number; i++){
		    int startX = Random.Range(2, width - 2);
		    int startY = Random.Range(2, height - 2);

			clearPoints[i] = new Vector2Int(startX, startY);

			RandomWalkCave(map, startX, startY, (int)((width*height / number) * 0.9f));
		}

		// Now connect the holes
		foreach (Vector2Int p in clearPoints){
			foreach (Vector2Int p2 in clearPoints){
				if (p != p2){
					// connect them all
					DrunkLine(map, p, p2);
				}
			}
		}
	}

	void DrunkLine(int[,] map, Vector2Int p1, Vector2Int p2){
		int remainingSteps = (int) Vector2Int.Distance(p1, p2) + 10;
		if (p1 == p2){
			return;
		}

		Vector2Int currentPos = p1;
		map[currentPos.x, currentPos.y] = 0;

		int leftProba = 4;
		int rightProba = 1;
		int upProba = 1;
		int downProba = 4;

		while(remainingSteps > 0){

			leftProba = 1;
			rightProba = 1;
			upProba = 1;
			downProba = 1;

			Vector2 difVector = p2 - currentPos;
			difVector = difVector.normalized * 10;

			if (difVector.x > 0){
				rightProba += (int)difVector.x;
			} else {
				leftProba -= (int)difVector.x;
			}
			if (difVector.y > 0){
				upProba += (int)difVector.y;
			} else {
				downProba -= (int)difVector.y;
			}

			int probaSums = leftProba+rightProba+upProba+downProba;
			int[] probaArray = new int[probaSums];
			for (int i=0; i < probaSums; i++){
				if (leftProba > 0){
					leftProba -= 1;
					probaArray[i] = 3;
					continue;
				}
				if (rightProba > 0){
					rightProba -= 1;
					probaArray[i] = 2;
					continue;
				}
				if (upProba > 0){
					upProba -= 1;
					probaArray[i] = 0;
					continue;
				}
				if (downProba > 0){
					downProba -= 1;
					probaArray[i] = 1;
					continue;
				}
			}

			int randDir = probaArray[Random.Range(0, probaSums-1)];

	        switch (randDir){
	            case 0:
	                if ((currentPos.y + 1) < width - 1){
	                    currentPos.y++;
	                }
	                break;
	            case 1:
	                if ((currentPos.y - 1) > 1) {
	                    currentPos.y--;
	                }
	                break;
	            case 2:
	                if ((currentPos.x + 1) < height - 1){
	                    currentPos.x++;
	                }
	                break;
	            case 3:
	                //Ensure that the edges are still tiles
	                if ((currentPos.x - 1) > 1){
	                    currentPos.x--;
	                }
	                break;
	        }
			paintRectangle(map, currentPos.x, currentPos.y, 1, 1, 0);
			remainingSteps -= 1;
		}

		// straight line if random fail
		int stp = 0;
		while (currentPos.x != p2.x || currentPos.y != p2.y){
			stp++;
			if (stp > 1000){
				Debug.Log("p1: " + p1 + " p2: " + p2);
				Debug.Log("loop infinito");
				Debug.Log("currentPos: " + currentPos + " p2: " + p2);
				//UnityEditor.EditorApplication.isPaused = true;
				return;
			}
			Vector2Int difVector = p2 - currentPos;

			if (difVector.x > 0){
				if ((currentPos.x + 1) < width){
					currentPos.x++;
				}
			} else if (difVector.x < 0){
				if ((currentPos.x - 1) > 0){
					currentPos.x--;
				}
			} else if (difVector.y > 0){
				if ((currentPos.y + 1) < height){
					currentPos.y++;
				}
			} else {
				if ((currentPos.y - 1) > 0) {
					currentPos.y--;
				}
			}
			paintRectangle(map, currentPos.x, currentPos.y, 1, 1, 0);
		}
	}

	void paintRectangle(int[,] map, int posX, int posY, int sizeX, int sizeY, int tilenum){
		for (int x=posX; x <= posX+sizeX; x++){
			for (int y=posY; y <= posY+sizeY; y++){
				if (x < 1 || x > width-2 || y < 1 || y > height-2){
					continue;
				}
				map[x, y] = tilenum;
			}
		}
	}

	void changeTilesWithNothingBelow(int[,] map){
		for(int x = 1; x < width-1; x++){
			for(int y = 0; y < height-1; y++){
				if (map[x, y] == 1 && map[x, y+1] == 0){

					map[x, y] = 2;

					if (y !=0){
						if (map[x, y-1] == 0){
							map[x, y-1] = 1;
						}
					}
				}
			}
		}
	}

	public void generateDungeon(){

		wall_tmap = GameObject.Find("tilemap_walls").GetComponent<Tilemap>();
		floor_tmap = GameObject.Find("tilemap_floor").GetComponent<Tilemap>();

		wallMap = GenerateArray(false);
		floorMap = GenerateArray(false);
		Debug.Log("Generating dungeon...");
		Debug.Log(Random.seed);
		MakeHoles(wallMap, holeNumber);
		placeChar();
		placeStairs();
		placeEnemies();
		changeTilesWithNothingBelow(wallMap);

		RenderMap(wallMap, wall_tmap, wall_tiles);
		RenderMap(floorMap, floor_tmap, floor_tiles);
	}

	void placeChar(){
		paintRectangle(wallMap, clearPoints[0].x-3, clearPoints[0].y-4, 6, 6, 0);
		managerScript.man.playerChar.transform.position = new Vector3(width/2 - clearPoints[0].x-0.5f, height/2-clearPoints[0].y, 0);
	}

	void placeStairs(){
		paintRectangle(wallMap, clearPoints[1].x-3, clearPoints[1].y-4, 6, 6, 0);
		Instantiate(stairsPrefab, new Vector3(width/2 - clearPoints[1].x-0.5f, height/2-clearPoints[1].y, 0), Quaternion.identity);
	}

	void placeEnemies(){
		for (int i=0; i < zombieNumber; i++){
			Vector2Int enemyPoint = clearPoints[Random.Range(0, holeNumber-1)] + Vector2Int.right * Random.Range(-5, 5) + Vector2Int.up * Random.Range(-7, 7);

			int cutoff = 0;
			while(!checkEmpty(wallMap, enemyPoint)){
				enemyPoint = clearPoints[Random.Range(0, holeNumber-1)] + Vector2Int.right * Random.Range(-5, 5) + Vector2Int.up * Random.Range(-7, 7);
				cutoff++;
				if (cutoff > 20){
					break;
				}
			}
			if (!checkEmpty(wallMap, enemyPoint)){
				continue;
			}

			Instantiate(zombiePrefab, new Vector3(width/2 - enemyPoint.x-0.5f, height/2-enemyPoint.y, 0f), Quaternion.identity);
		}
	}

	bool checkEmpty(int[,] map, Vector2Int point){
		for (int x=point.x - 1; x <= point.x+1; x++) {
			for (int y=point.y - 1; y <= point.y+1; y++) {
				if (x < 1 || x > width-2 || y < 1 || y > height-2 || map[x, y] != 0){
					return false;
				}
				if ((managerScript.man.playerChar.transform.position - new Vector3(width/2 - point.x-0.5f, height/2-point.y, 0)).magnitude < 6f){
					return false;
				}
			}
		}
		return true;
	}

	// Use this for initialization
	void Awake () {
		dungeon = this;
	}

	// Update is called once per frame
	void Update () {
		/*
		if (managerScript.man.debug){
			if (Input.GetKey("space")){
				managerScript.man.startDungeon();
			}
		}
		*/
	}
}
