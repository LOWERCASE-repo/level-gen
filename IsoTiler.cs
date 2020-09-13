#pragma warning disable 649
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace LevelGen {
	
	class IsoTiler {
		
		internal IsoTiler(HashSet<Vector2Int> sketch, Tilemap tilemap, TileBase tile) {
			foreach (Vector2Int pos in sketch) {
				tilemap.SetTile((Vector3Int)pos, tile);
			}
		}
	}
	
	
}
