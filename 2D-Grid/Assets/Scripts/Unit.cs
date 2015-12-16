using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public int tileX;
	public int tileY;

	public TileMap map;

	public List<Node> currentPath = null;

	public float moveSpeed = 2;

	float timer = 0;
	float moveTime = 0.3f;


	void Update(){
		if (currentPath != null) {
			int currNode = 0;
			while(currNode < currentPath.Count-1){
				Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
				Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode+1].x, currentPath[currNode+1].y);

				Debug.DrawLine(start,end);

				currNode++;
			}
			timer+=Time.deltaTime;
			if(timer >= moveTime){
				MoveNextTile();
				timer=0;
			}
		}
	}

	public void MoveNextTile(){
		float remainingMovement = moveSpeed;
		while (remainingMovement > 0) {
			if (currentPath == null) {
				return;
			}

			remainingMovement -= map.CostToEnterTile(currentPath [0].x, currentPath [0].y, currentPath [1].x, currentPath [1].y);

			tileX = currentPath[1].x;
			tileY = currentPath[1].y;
			transform.position = map.TileCoordToWorldCoord (tileX,tileY);

			//remove old current/first from list
			currentPath.RemoveAt (0);

			if (currentPath.Count == 1) { //we have arrived
				currentPath = null;
			}
		}
	}

}
