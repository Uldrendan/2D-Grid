using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileMap : MonoBehaviour {
	public GameObject selectedUnit;

	public GameObject[,] allTiles;
	public GameObject grassTile;
	public GameObject swampTile;
	public GameObject mountainTile;
	//not sure if better to use array of tiles for generation
	//public GameObject[] availableTiles;

	Node[,] graph;
	
	public int mapSizeX = 10;
	public int mapSizeY = 10;
	
	void Start(){
		//set up selected unit
		selectedUnit.GetComponent<Unit> ().tileX = (int)selectedUnit.transform.position.x;
		selectedUnit.GetComponent<Unit> ().tileY = (int)selectedUnit.transform.position.y;
		selectedUnit.GetComponent<Unit> ().map = this;
		
		GenerateMapData ();
		GeneratePathFindingGraph ();
		GenerateMapVisuals ();
	}
	
	void GenerateMapData(){
		allTiles = new GameObject[mapSizeX,mapSizeY];
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++){
				allTiles[x,y] = grassTile;
			}
		}
		
		for (int x = 3; x <=5; x++) {
			for(int y=0; y < 4; y++){
				allTiles[x,y] = swampTile;
			}
		}
		allTiles [4, 4] = mountainTile;
		allTiles [5, 4] = mountainTile;
		allTiles [6, 4] = mountainTile;
		allTiles [7, 4] = mountainTile;
		allTiles [8, 4] = mountainTile;	
		allTiles [4, 5] = mountainTile;	
		allTiles [4, 6] = mountainTile;	
		allTiles [8, 5] = mountainTile;
		allTiles [8, 6] = mountainTile;	
	}

	
	public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY){
		TileType tt = allTiles [targetX, targetY].GetComponent<ClickableTile> ().type;
		if (UnitCanEnterTile (targetX, targetY) == false) {
			return Mathf.Infinity;
		}
		float cost = tt.movementCost;
		return cost;
	}
	
	void GeneratePathFindingGraph(){
		//initialize array
		graph = new Node[mapSizeX, mapSizeY];
		
		//initialize nodes
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				//graph[x,y] = new Node();
				allTiles[x,y].GetComponent<ClickableTile>().nodeData = new Node();
				graph[x,y] = allTiles[x,y].GetComponent<ClickableTile>().nodeData;
				graph[x,y].x = x;
				graph[x,y].y = y;
			}
		}
		
		//set neighbours
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				
				if(x>0)
					graph[x,y].neighbours.Add (graph[x-1,y]);
				if(x < mapSizeX-1)
					graph[x,y].neighbours.Add (graph[x+1,y]);
				if(y>0)
					graph[x,y].neighbours.Add (graph[x,y-1]);
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add (graph[x,y+1]);
			}
		}
	}
	
	void GenerateMapVisuals(){
		GameObject board = new GameObject();
		board.name = "Board";
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++){
				GameObject go = (GameObject)Instantiate(allTiles[x,y], new Vector3(x,y,0), Quaternion.identity);
				ClickableTile ct = go.GetComponent<ClickableTile>();
				ct.tileX = x;
				ct.tileY = y;
				ct.map = this;
				
				go.transform.parent = board.transform;
			}
		}	
	}
	
	public Vector3 TileCoordToWorldCoord(int x, int y){
		return new Vector3 (x, y, 0);
	}
	
	public bool UnitCanEnterTile(int x, int y){
		//could test unit's movetype against terrain flags
		return allTiles [x, y].GetComponent<ClickableTile> ().type.isWalkable;
	}
	
	public void GeneratePathTo(int x, int y){
		//clear out old path
		selectedUnit.GetComponent<Unit> ().currentPath = null;
		
		if (UnitCanEnterTile (x, y) == false) {
			//clicked on impassable path
			return;
		}
		
		Dictionary<Node, float> dist = new Dictionary<Node, float> ();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node> ();
		
		List<Node> unvisited = new List<Node> ();
		
		Node source = graph [selectedUnit.GetComponent<Unit> ().tileX, selectedUnit.GetComponent<Unit> ().tileY];
		
		Node target = graph [x, y];
		
		dist [source] = 0;
		prev [source] = null;
		
		//initialize everything to have infinity distance, no better knowledge
		foreach (Node v in graph) {
			if(v != source){
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}
			unvisited.Add (v);
		}
		
		while (unvisited.Count > 0) {
			//u is going to be the unvisited node with the smallest distance
			Node u = null;
			
			foreach(Node possibleU in unvisited){
				if(u == null || dist[possibleU] < dist[u]){
					u = possibleU;
				}
			}
			
			if(u==target){
				break; //exit while loop
			}
			
			unvisited.Remove (u);
			
			foreach(Node v in u.neighbours){
				//                float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
				if(alt< dist[v]){
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}
		//either we found the shortest route or or there is no route
		if (prev [target] == null) {
			return;
		}
		
		List<Node> currentPath = new List<Node> ();
		Node curr = target;
		while (curr!=null) {
			currentPath.Add (curr);
			curr = prev[curr];
		}
		
		//right now describes path from target to source
		currentPath.Reverse ();
		
		selectedUnit.GetComponent<Unit> ().currentPath = currentPath;
	}
}