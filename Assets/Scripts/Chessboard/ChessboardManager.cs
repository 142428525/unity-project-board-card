using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Chessboard
{
	public class ChessboardManager : Utils.MonoSingleton<ChessboardManager>
	{
		public Tilemap CHESSBOARD;
		public Tilemap NON_COLLI_CHECKS;
		public Tilemap COLLI_CHECKS;
		public TileBase CHESSBOARD_TILE;	// for-test only
		public TileBase CHECK_TILE;	// for-test only

		// Start is called before the first frame update
		void Start()
		{
			add_event_listener();

			CHESSBOARD.ClearAllTiles();
			NON_COLLI_CHECKS.ClearAllTiles();
			COLLI_CHECKS.ClearAllTiles();

			Debug.Log($"{CHESSBOARD.origin}, {CHESSBOARD.size}");   // here the 2 are (0, 0) and (0, 0)

			CHESSBOARD.size = new Vector3Int(18, 18);

			Debug.Log($"{CHESSBOARD.origin}, {CHESSBOARD.size}");

			CHESSBOARD.FloodFill(CHESSBOARD.origin, CHESSBOARD_TILE);

			void add_event_listener()
			{
				InputManager.OnEmplace += on_emplace;
			}
		}

		// Update is called once per frame
		void Update()
		{

		}

		public Vector2Int ToBoardPos(Vector3 world_pos)
		{
			return (Vector2Int)CHESSBOARD.WorldToCell(world_pos);
		}

		private void on_emplace(object sender, InputManager.InputEventArgs<Vector2Int> e)
		{
			var cell_pos = e.Value;
			var is_emplacable = CHESSBOARD.HasTile((Vector3Int)cell_pos) && !COLLI_CHECKS.HasTile((Vector3Int)cell_pos);
			
			Debug.Log($"[boardsys] {is_emplacable}");

			if (is_emplacable)
			{
				COLLI_CHECKS.SetTile((Vector3Int)cell_pos, CHECK_TILE);
				Debug.Log($"落子于{cell_pos}");
			}
		}
	}
}