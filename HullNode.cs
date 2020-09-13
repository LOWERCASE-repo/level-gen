#pragma warning disable 649
using UnityEngine;
using System.Collections.Generic;

namespace LevelGen {
	
	class HullNode {
		
		internal Vector2 pos, pivot, nextPivot;
		internal HullNode next;
		internal float Arc {
			get => (next.angle - linkAngle).Angle();
		}
		internal float linkAngle;
		float angle;
		
		internal HullNode(Vector2 pos, Vector2 pivot, Vector2 nextPivot, float angle, float linkAngle) {
			this.pos = pos;
			this.pivot = pivot;
			this.nextPivot = nextPivot;
			this.angle = angle.Angle();
			this.linkAngle = linkAngle.Angle();
		}
		
		internal bool Within(Vector2 pos, float sqrRadius) {
			if (pos == pivot || pos == nextPivot) return false;
			else return ((pos - this.pos).sqrMagnitude < sqrRadius);
		}
		
		internal bool WithinAny(IEnumerable<Vector2> pivots, float sqrRadius) {
			foreach (Vector2 pivot in pivots) {
				if (Within(pivot, sqrRadius)) {
					return true;
				}
			}
			return false;
		}
		
		internal float ClockDist(HullNode node) {
			return (angle - node.linkAngle).Angle();
		}
		
		internal float CounterClockDist(HullNode node) {
			return (node.angle - linkAngle).Angle();
		}
	}
}
