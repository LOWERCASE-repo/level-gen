#pragma warning disable 649
using UnityEngine;
using System.Collections.Generic;
using Navigation;
using Behaviours;

namespace LevelGen {
	
	class Spawner : MonoBehaviour {
		
		[SerializeField] GameObject portal;
		[SerializeField] Alarm alarm;
		[SerializeField] Explorer enemy;
		
		List<Vector2> rooms = new List<Vector2>();
		
		internal void Spawn(Chassis chassis, float roomWidth, float pathLength) {
			foreach (HashSet<Bezite> branchSet in chassis.branchSets) {
				foreach (Bezite branch in branchSet) rooms.Add(branch.Eval(1f));
			}
			rooms.Remove(default(Vector2));
			Instantiate(portal, rooms[rooms.Count - 1], default(Quaternion));
			
			Alarm alarm = Instantiate(this.alarm, chassis.bridges[0].Eval(1f), default(Quaternion));
			alarm.gameObject.transform.Scale(pathLength);
			
			foreach (Vector2 pos in RandomCircle(5, roomWidth / 2f)) {
				Explorer enemy = Instantiate(this.enemy, pos, default(Quaternion));
				alarm.sleepers.Add(enemy);
			}
		}
		
		Vector2 RandomPos() {
			int index = Random.Range(0, rooms.Count);
			Vector2 room = rooms[index];
			rooms.Remove(room);
			return room;
		}
		
		HashSet<Vector2> RandomCircle(int count, float radius) {
			Vector2 center = RandomPos();
			Vector2 pos = Vector2.up * radius;
			HashSet<Vector2> circlePositions = new HashSet<Vector2>();
			float delta = 360f / (float)count;
			pos = pos.Rotate(Random.value * delta);
			for (int i = 0; i < count; i++) {
				circlePositions.Add(center + pos.Rotate(i * delta));
			}
			return circlePositions;
		}
	}
}
