using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public bool walkable;
	public Vector3 worldPosition;
	public int i, j;

	public Node(bool walk, Vector3 worldPos, int _i, int _j){
		walkable = walk;
		worldPosition = worldPos;
		i = _i;
		j = _j;
	}
}
