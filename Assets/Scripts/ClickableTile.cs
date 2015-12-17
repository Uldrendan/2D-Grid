using UnityEngine;
using System.Collections;

public class ClickableTile : MonoBehaviour {

	public int tileX;
	public int tileY;
	[HideInInspector]
	public TileMap map;
	public TileType type;
	public Node nodeData;

	Color start_color;
	SpriteRenderer sprite_rend;

	void Start(){
		sprite_rend = this.GetComponent<SpriteRenderer> ();
		start_color = sprite_rend.color;
	}

	void OnMouseUp(){
		map.GeneratePathTo (tileX, tileY);
	}

	void OnMouseEnter(){
		start_color = sprite_rend.color;
		if (!type.isWalkable)
			sprite_rend.color = Color.red;
		else
			sprite_rend.color = Color.yellow;
	}
	
	void OnMouseExit(){
		sprite_rend.color = start_color;
	}
}
