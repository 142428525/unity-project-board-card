using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessboardManager : MonoBehaviour
{
	public Tilemap CHESSBOARD;
	public Tilemap CHECKS;
	public TileBase CHESSBOARD_TILE;
	public TileBase CHECK;

	public static ChessboardManager Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		CHESSBOARD.ClearAllTiles();
		CHECKS.ClearAllTiles();

		Debug.Log($"{CHESSBOARD.origin}, {CHESSBOARD.size}");

		CHESSBOARD.size = new Vector3Int(18, 18);

		Debug.Log($"{CHESSBOARD.origin}, {CHESSBOARD.size}");

		CHESSBOARD.FloodFill(CHESSBOARD.origin, CHESSBOARD_TILE);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
