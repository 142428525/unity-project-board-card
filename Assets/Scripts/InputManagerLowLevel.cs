using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputManager
{
	public static class LowLevel
	{
		public static bool ExistMouse { get { return Mouse.current != null; } }
		public static bool ExistKeyboard { get { return Keyboard.current != null; } }

		public static Vector2 ReadMousePosition()
		{
			return ExistMouse ? Mouse.current.position.ReadValue() : throw new InvalidOperationException("Mouse doesn't exist.");
		}
	}
}