#pragma warning disable 649
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace LevelGen {
	
	// TODO SetTransformMatrix
	/*
	private void SetTile(Vector3Int pos, Quaternion rot, Tilemap tilemap, TileBase tile)
	{
	tilemap.SetTile(pos, tile);
	tilemap.SetTransformMatrix(pos, Matrix4x4.TRS(Vector3.zero, rot, Vector3.one));
	*/
	
	class Tiler {
		
		// first two are dir, third is value
		int[,] wallPoll = new int[4, 3] {
			{ 0, 1, 4 }, { 1, 0, 2 },
			{ 0, -1, 8 }, { -1, 0, 1 }
		};
		int[,] cornerPoll = new int[4, 3] {
			{ -1, 1, 1 }, { 1, 1, 2 },
			{ -1, -1, 4 }, { 1, -1, 8 }
		};
		
		Dictionary<int, TileBase> wallMap;
		Dictionary<int, TileBase> cornerMap;
		
		HashSet<Vector2Int> sketch;
		Tilemap tilemap;
		TileBase[] tiles;
		
		internal Tiler(HashSet<Vector2Int> mapSketch, Tilemap tilemap, TileBase[] tiles) {
			this.sketch = mapSketch;
			this.tilemap = tilemap;
			this.tiles = tiles;
			
			wallMap = new Dictionary<int, TileBase> {
				{5, tiles[0]}, {4, tiles[1]}, {6, tiles[2]},
				{1, tiles[3]}, {0, tiles[4]}, {2, tiles[5]},
				{9, tiles[6]}, {8, tiles[7]}, {10, tiles[8]}
			};
			
			cornerMap = new Dictionary<int, TileBase> {
				{1, tiles[9]}, {2, tiles[10]},
				{4, tiles[11]}, {8, tiles[12]}
			};
			
			foreach (Vector2Int pos in sketch) {
				int tileType = Poll(pos, wallPoll);
				tilemap.SetTile((Vector3Int)pos, wallMap[tileType]);
				if (tileType != 0) continue;
				tileType = Poll(pos, cornerPoll);
				if (!cornerMap.ContainsKey(tileType)) continue;
				tilemap.SetTile((Vector3Int)pos, cornerMap[tileType]);
			}
		}
		
		int Poll(Vector2Int pos, int[,] poll) {
			int tileType = 0;
			for (int i = 0; i < 4; i++) {
				Vector2Int pollPos = pos + new Vector2Int(poll[i, 0], poll[i, 1]);
				if (!sketch.Contains(pollPos)) tileType += poll[i, 2];
			}
			return tileType;
		}
	}
}
