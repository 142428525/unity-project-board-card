using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Chessboard;

public class ChessboardDataManager : MonoBehaviour
{
	private int[,] chessboard_map;
	private int[,] checks_map;

	public static ChessboardDataManager Instance { get; private set; }

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

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Construct(Vector2Int size)
	{
		chessboard_map = new int[size.y, size.x];
		checks_map = new int[size.y, size.x];

		// NOTE: array index != coordinate position
		// idx[i, j] <-> pos(j, i)
		// pos(x, y) <-> idx[y, x]
	}

	public void Resize(ResizeInfo info)
	{

	}
}
