using System;
using System.Collections;
using System.Collections.Generic;
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
		public Vector2 WorldPos { get; }

		public CursorEventArgs(Vector2 raw)
		{
			ScreenPos = raw;
			WorldPos = Camera.main.ScreenToWorldPoint(raw);
		}

		public bool isOnScreen()
		{
			Bounds screen_border = new();
			screen_border.SetMinMax(Vector2.zero, new Vector2(Screen.width, Screen.height));
			return screen_border.Contains(ScreenPos);
		}
	}

	public static event EventHandler<InputEventArgs<Vector2Int>> OnEmplace;
	public static event EventHandler<CursorEventArgs> WhenCursorMove;
	public static event EventHandler<CursorEventArgs> WhenCursorOnScreen;
	public static event EventHandler<InputEventArgs<float>> WhenScroll;
	public static event EventHandler OnConsoleOpen;

	public static void InvokeEmplace(object sender, Vector2Int v) => OnEmplace?.Invoke(sender, new InputEventArgs<Vector2Int>(v));
}
