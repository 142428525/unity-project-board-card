using System;
using System.Collections;
using System.Collections.Generic;

namespace Chessboard
{
	public struct ResizeInfo
	{
		[Flags]
		public enum ResizedBound
		{
			None = 0b0000,
			Top = 0b0001,
			Bottom = 0b0010,
			Left = 0b0100,
			Right = 0b1000
		}

		public enum CellAnchor
		{
			Center,
			N, E, S, W,
			NE, NW, SE, SW
		}

		public ResizedBound Bounds;
		public CellAnchor Anchor;
	}
}
