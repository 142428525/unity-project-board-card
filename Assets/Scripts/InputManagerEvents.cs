using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public partial class InputManager
{
	// here are static members & inner classes related to events

	public class InputEventArgs<T> : EventArgs
	{
		public T Value { get; }

		public InputEventArgs(T v)
		{
			Value = v;
		}
	}

	public class CursorEventArgs : EventArgs
	{
		public Vector2 ScreenPos { get; }
		public Vector2 WorldPosMain { get; }

		public CursorEventArgs(Vector2 raw)
		{
			ScreenPos = raw;
			WorldPosMain = GetWorldPos(Utils.CameraView.Type.Board);
		}

		public Vector2 GetWorldPos(Utils.CameraView.Type type) => Utils.CameraView.ScreenToWorldPos(type, ScreenPos);

		public bool IsOnScreen()
		{
			return 0 <= ScreenPos.x && ScreenPos.x <= Screen.width && 0 <= ScreenPos.y && ScreenPos.y <= Screen.height;
		}
	}

	public static event EventHandler<InputEventArgs<Vector2Int>> OnEmplace;
	public static event EventHandler<CursorEventArgs> WhenCursorMove;
	public static event EventHandler<CursorEventArgs> WhenCursorOnScreen;
	public static event EventHandler<InputEventArgs<float>> WhenScroll;
	public static event EventHandler OnConsoleOpen;

	public static void InvokeEmplace(object sender, Vector2Int v) => OnEmplace?.Invoke(sender, new InputEventArgs<Vector2Int>(v));
}
