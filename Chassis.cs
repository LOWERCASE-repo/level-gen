#pragma warning disable 649
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LevelGen {
	
	class Chassis {
		
		internal List<HashSet<Bezite>> branchSets = new List<HashSet<Bezite>>();
		internal List<Bezite> bridges = new List<Bezite>();
		
		Blueprint graph;
		float curviness;
		
		internal Chassis(Blueprint graph, int count, float curviness) {
			this.graph = graph;
			this.curviness = curviness;
			for (int i = 0; i < graph.pivots.Count; i++) {
				branchSets.Add(new HashSet<Bezite>());
				if (i + 1 < graph.pivots.Count) {
					bridges.Add(Spawn(graph.pivots[i], graph.pivots[i + 1]));
				}
			}
			float delta = graph.totalArc / (float)count;
			float offset = Random.value * delta;
			for (int i = 0; i < count; i++) Eval(offset + delta * i);
			ShiftAll(-branchSets[0].ElementAt(0).Eval(1f));
		}
		
		internal void Worldify(Func<Vector2, Vector2> Worldify) {
			foreach (HashSet<Bezite> branchSet in branchSets) {
				foreach (Bezite branch in branchSet) {
					for (int i = 0; i < branch.pivots.Length; i++) {
						branch.pivots[i] = Worldify(branch.pivots[i]);
					}
				}
			}
			foreach (Bezite bridge in bridges) {
				for (int i = 0; i < bridge.pivots.Length; i++) {
					bridge.pivots[i] = Worldify(bridge.pivots[i]);
				}
			}
		}
		
		void ShiftAll(Vector2 shift) {
			foreach (HashSet<Bezite> branchSet in branchSets) {
				foreach (Bezite branch in branchSet) branch.Shift(shift);
			}
			foreach (Bezite bridge in bridges) bridge.Shift(shift);
		}
		
		void Eval(float angle) {
			foreach (HullNode node in graph.nodes) {
				if (angle <= node.Arc) {
					angle += node.linkAngle;
					Vector2 end = node.nextPivot + graph.radius.Rotate(angle);
					Bezite bezite = Spawn(node.nextPivot, end);
					branchSets[graph.pivots.IndexOf(node.nextPivot)].Add(bezite);
					return;
				} else angle -= node.Arc;
			}
		}
		
		Bezite Spawn(Vector2 start, Vector2 end) {
			Vector2 dir = 0.5f * (end - start);
			float angle = 90f * (0.5f - Random.value);
			angle += (angle < 0f) ? -45f : 45f;
			Vector2 pivot = start + (dir * curviness).Rotate(angle);
			return new Bezite(start, dir + pivot, end);
		}
	}
}
