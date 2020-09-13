using UnityEngine;

namespace LevelGen {
	
	// bezier + hermite, trades linearity for end pivot emphasis
	
	class Bezite {
		
		internal Vector2[] pivots;
		
		internal Bezite(params Vector2[] pivots) {
			this.pivots = pivots;
		}
		
		internal void Shift(Vector2 shift) {
			for (int i = 0; i < pivots.Length; i++) {
				pivots[i] += shift;
			}
		}
		
		internal Vector2 Eval(float time) {
			int endIndex = pivots.Length - 1;
			if (time == 0f) return pivots[0];
			if (time == 1f) return pivots[endIndex];
			return EvalRec(time, 0, endIndex);
		}
		
		Vector2 EvalRec(float time, int startIndex, int endIndex) {
			if (startIndex == endIndex) return pivots[startIndex];
			Vector2 start = EvalRec(time, startIndex, endIndex - 1);
			Vector2 end = EvalRec(time, startIndex + 1, endIndex);
			// return Vector2.Lerp(start, end, time);
			return start + (end - start) * time.Hermite();
		}
	}
}
