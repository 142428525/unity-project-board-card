using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
	public static class CameraView
	{
		public enum Type
		{
			Board,
			UI
		}

		public static Camera UICamera = (from cam in Camera.allCameras
										 where cam.CompareTag("UICamera")
										 select cam).First();

		public static Vector2 ScreenToWorldPos(Type type, Vector2 raw)
		{
			return type switch
			{
				Type.Board => Camera.main.ScreenToWorldPoint(raw),
				Type.UI => UICamera.ScreenToWorldPoint(raw),
				_ => throw new ArgumentOutOfRangeException(nameof(type), "Not a valid enum value.")
			};
		}
	}
}
