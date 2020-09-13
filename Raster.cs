#pragma warning disable 649
using UnityEngine;
using System;
using System.Collections.Generic;

namespace LevelGen {
	
	class Raster {
		
		internal HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
		HashSet<Vector2Int> oldPositions = new HashSet<Vector2Int>();
		Dictionary<int, HashSet<Vector2Int>> brushes = new Dictionary<int, HashSet<Vector2Int>>();
		Vector2Int[] dirs = new Vector2Int[4] {
			Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
		};
		Func<float, float> Hermite;
		Bezite bezite;
		
		internal Raster(Chassis chassis, Func<float, float> Hermite) {
			this.Hermite = Hermite;
			foreach (Bezite bridge in chassis.bridges) {
				bezite = bridge;
				new Linker<Vector3Int>(InitBridgeNode, Eval);
			}
			foreach (HashSet<Bezite> branchSet in chassis.branchSets) {
				foreach (Bezite branch in branchSet) {
					bezite = branch;
					new Linker<Vector3Int>(InitPathNode, Eval);
				}
			}
			while (!oldPositions.SetEquals(positions)) {
				oldPositions = new HashSet<Vector2Int>(positions);
				positions.Clear();
				foreach (Vector2Int tile in oldPositions) Smooth(tile);
			}
		}
		
		Vector3Int InitPathNode(float time) {
			Vector3Int node = Vector3Int.RoundToInt(bezite.Eval(time));
			node.z = (int)Mathf.Round(Hermite(time));
			Pave((Vector2Int)node, node.z);
			return node;
		}
		
		Vector3Int InitBridgeNode(float time) {
			Vector3Int node = Vector3Int.RoundToInt(bezite.Eval(time));
			node.z = (int)Mathf.Round(Hermite(0f));
			Pave((Vector2Int)node, node.z);
			return node;
		}
		
		bool Eval(LinkedListNode<Vector3Int> node) {
			Vector2Int nodePos = (Vector2Int)node.Value;
			Vector2Int nextPos = (Vector2Int)node.Next.Value;
			int nodeRadius = node.Value.z;
			int nextRadius = node.Next.Value.z;
			int sqrDist = (nextPos - nodePos).sqrMagnitude;
			int max = (nodeRadius > nextRadius) ? nodeRadius : nextRadius;
			return sqrDist > max * max;
		}
		
		void Smooth(Vector2Int tile) {
			int cellCount = 0;
			foreach (Vector2Int dir in dirs) {
				if (oldPositions.Contains(tile + dir)) cellCount++;
			}
			int minCellCount = oldPositions.Contains(tile) ? 1 : 2;
			if (cellCount > minCellCount) {
				if (positions.Add(tile)) {
					foreach (Vector2Int dir in dirs) Smooth(tile + dir);
				}
			}
		}
		
		void Pave(Vector2Int point, int radius) {
			HashSet<Vector2Int> brush;
			if (brushes.ContainsKey(radius)) brush = brushes[radius];
			else brush = CreateBrush(radius);
			foreach (Vector2Int pos in brush) positions.Add(point + pos);
		}
		
		HashSet<Vector2Int> CreateBrush(int radius) {
			HashSet<Vector2Int> brush = new HashSet<Vector2Int>();
			int sqrRadius = radius * radius;
			for (int x = 0; x < radius; x++) {
				int height = (int)Mathf.Round(Mathf.Sqrt(sqrRadius - x * x));
				for (int y = 0; y < height; y++) {
					brush.Add(new Vector2Int(x, y));
					brush.Add(new Vector2Int(-y, x));
					brush.Add(new Vector2Int(-x, -y));
					brush.Add(new Vector2Int(y, -x));
				}
			}
			brushes.Add(radius, brush);
			return brush;
		}
	}
}
