#pragma warning disable 649
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using Navigation;

namespace LevelGen {
	
	class Architect : MonoBehaviour {
		
		[Header("Structure")]
		[SerializeField] int pivotCount = 5;
		[SerializeField] int roomCount = 10;
		[SerializeField] float pathLength = 20f;
		[SerializeField] float curviness = 1f;
		[SerializeField] float startAngle = 0f;
		[SerializeField] float angleBias = 0f;
		
		[Header("Tiles")]
		[SerializeField] float bridgeRadius = 3f;
		[SerializeField] float branchRadius = 7f;
		[SerializeField] float slope = -0.5f;
		[SerializeField] TileBase[] tiles = new TileBase[13];
		[SerializeField] TileBase isoTile;
		
		Tilemap tilemap;
		Spawner spawner;
		Grid grid;
		
		float startTime;
		
		void Awake() {
			tilemap = transform.GetChild(0).GetComponent<Tilemap>();
			spawner = transform.GetChild(1).GetComponent<Spawner>();
			grid = GetComponent<Grid>();
		}
		
		IEnumerator Start() {
			startTime = Time.realtimeSinceStartup;
			int seed = (int)(100000f - Random.value * 200000f);
			Random.InitState(seed);
			Debug.Log($"level seed: {seed}");
			Blueprint blueprint = new Blueprint(pivotCount, pathLength, startAngle, angleBias);
			Chassis chassis = new Chassis(blueprint, roomCount, curviness);
			Raster raster = new Raster(chassis, Hermite);
			// Tiler tiler = new Tiler(raster.positions, tilemap, tiles);
			IsoTiler isoTiler = new IsoTiler(raster.positions, tilemap, isoTile);
			
			chassis.Worldify(Worldify);
			spawner.Spawn(chassis, branchRadius, pathLength);
			yield return null;
			
			Weaver weaver = new Weaver(chassis);
			NavGraph.self.Init(weaver.nodes);
			
			Debug.Log($"gen time: {Time.realtimeSinceStartup - startTime}");
			Destroy(this);
		}
		
		float Hermite(float time) {
			float delta = branchRadius - bridgeRadius;
			return bridgeRadius + delta * time.Hermite(slope);
		}
		
		Vector2 Worldify(Vector2 pos) {
			return grid.CellToWorld(Vector3Int.RoundToInt(pos));
		}
	}
}
