using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : MonoBehaviour {

	public LayerMask avoidableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	public static AStar pathfinding;

	// Use this for initialization
	void Awake () {
		pathfinding = this;

		nodeDiameter = nodeRadius;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
	}

	void Update(){
		//updateGrid();
	}

	public void createGrid(){
		//Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2f - Vector3.up * gridWorldSize.y/2f;

		grid = new Node[gridSizeX, gridSizeY];
		for (int x = 0; x < gridSizeX; x++){
			for (int y = 0; y < gridSizeY; y++){
				Vector3 worldPoint = new Vector3(gridWorldSize.x, gridWorldSize.y, 0f)/2f - Vector3.right * (x*nodeDiameter + nodeRadius/2f) - Vector3.up * (y*nodeDiameter + nodeRadius/2f);

				bool walkable = dungeonGeneratorScript.dungeon.wallMap[x, y] == 0;
				grid[x, y] = new Node(walkable, worldPoint, x, y);
			}
		}
	}

	public void updateGrid(){
		for (int x = 0; x < gridSizeX; x++){
			for (int y = 0; y < gridSizeY; y++){
				bool walkable = dungeonGeneratorScript.dungeon.wallMap[x, y] == 0;
				grid[x, y].walkable = walkable;
			}
		}
	}

	float heuristic(Node a, Node b){
		return Mathf.Abs(a.i - b.i) + Mathf.Abs(a.j - b.j);
	}

	public Node point2Grid(Vector3 point){

		int x = (int)((gridWorldSize.x/2f-point.x)/nodeDiameter);
		int y = (int)((gridWorldSize.y/2f-point.y)/nodeDiameter);

		if (x >= gridSizeX){
			x = gridSizeX -1;
		}
		if (x < 0){
			x = 0;
		}
		if (y >= gridSizeY){
			y = gridSizeY -1;
		}
		if (y < 0){
			y = 0;
		}

		return grid[x, y];
	}

	List<Node> neighbours(Node current){
		List<Node> result = new List<Node>();

		for (int i = -1; i < 2; ++i){
			for (int j = -1; j < 2; ++j){
				if (i==0 && j==0){
					continue;
				}
				int x = current.i + i;
				int y = current.j + j;
				if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY){
					if (grid[x, y].walkable){
						result.Add(grid[x, y]);
					}
				}

			}
		}

		return result;
	}

	public List<Vector3> FindPath(Vector3 start, Vector3 goal){
		Node goalNode = point2Grid(goal);
		Node startNode = point2Grid(start);

		PriorityQueue<Node> frontier = new PriorityQueue<Node>();
		frontier.Add(startNode, 0);
		Dictionary<Node, float> cost_so_far = new Dictionary<Node, float>();
		Dictionary<Node, Node> came_from = new Dictionary<Node, Node>();

		List<Vector3> result = new List<Vector3>();

		came_from[startNode] = startNode;
  		cost_so_far[startNode] = 0;

		if (!goalNode.walkable){
			return result;
		}

		while (!frontier.Empty()){
			//TODO: implementar heap
			Node current = frontier.Get();

			if (current == goalNode){
				break;
			}

			foreach (Node next in neighbours(current)){
				float new_cost = cost_so_far[current] + 1;
				if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next]){
					cost_so_far[next] = new_cost;
					float priority = new_cost + heuristic(next, goalNode);
					frontier.Add(next, priority);
					came_from[next] = current;
				}
			}

		}

		if (came_from.ContainsKey(goalNode)){
			Node currentNode = goalNode;
			while(currentNode != startNode){
				result.Add(currentNode.worldPosition);
				currentNode = came_from[currentNode];
			}
		}
		result.Reverse();
		return result;
	}

	void OnDrawGizmos(){
		Gizmos.DrawWireCube(new Vector3(0, 0, 1f), new Vector3(gridWorldSize.x, gridWorldSize.y, 1f));

		if (managerScript.man == null){
			return;
		}
		if (managerScript.man.debug){
			if (grid != null){
				foreach (Node n in grid){
					if (n.walkable){
						Gizmos.color = Color.white;
						Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter-0.1f));
					} else {
						Gizmos.color = Color.red;
						Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter-0.1f));
					}
				}

				//Test
				/*
				Gizmos.color = Color.blue;
				Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Gizmos.DrawCube(point2Grid(point).worldPosition, Vector3.one * (nodeDiameter-0.1f));

				List<Vector3> path = FindPath(transform.position, point);
				if (path.Count > 0){
					foreach(Vector3 p in path){
						Gizmos.DrawCube(p, Vector3.one * (nodeDiameter-0.1f));
					}
				}
				*/
			}

		}
	}


}
