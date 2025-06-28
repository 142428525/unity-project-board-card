using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		var mouse = Mouse.current;
		if (mouse.leftButton.wasPressedThisFrame)
		{
			var point = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
			Collider2D[] ds = Physics2D.OverlapPointAll(point);
			foreach (var d in ds)
			{
				var cell_pos = ChessboardManager.Instance.CHESSBOARD.WorldToCell(point);
				var is_tile_empty = ds.Length == 1 && ds.Single().gameObject == ChessboardManager.Instance.CHESSBOARD.gameObject;
				Debug.Log($"{point}, {cell_pos}");
				Debug.Log($"{is_tile_empty}: {ds.Length == 1}, {ds.Single().gameObject == ChessboardManager.Instance.CHESSBOARD.gameObject}");

				if (is_tile_empty)
				{
					ChessboardManager.Instance.CHECKS.SetTile(cell_pos, ChessboardManager.Instance.CHECK);
					Debug.Log($"落子于{cell_pos}");
				}
			}
		}
	}
}
