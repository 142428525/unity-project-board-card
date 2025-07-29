using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public partial class InputManager : Utils.MonoSingleton<InputManager>
{
	// NOTE: 假设用户条件艰苦。应该让用户只有鼠标或键盘也能玩。

	public enum InputMode
	{
		ModalUI,
		Player,
		Disabled
	}

	private InputControls input_controls;
	private InputMode mode = InputMode.Disabled;

	public InputMode Mode
	{
		get { return mode; }
		set
		{
			mode = value;

			switch (value)
			{
				case InputMode.ModalUI:
					input_controls.UI.Enable();
					input_controls.Player.Disable();
					break;

				case InputMode.Player:
					input_controls.UI.Disable();
					input_controls.Player.Enable();
					break;

				case InputMode.Disabled:
					input_controls.Disable();
					break;
			}
		}
	}

	protected override void Awake()
	{
		input_controls = new();

		input_controls.Player.MouseConfirm.performed += wait(when_mouse_confirm);
		input_controls.Player.Cursor.performed += wait(e => WhenCursorMove?.Invoke(this, new CursorEventArgs(e.ReadValue<Vector2>())));
		input_controls.Player.Zoom.performed += wait(e => WhenScroll?.Invoke(this, new InputEventArgs<float>(e.ReadValue<Vector2>().y)));

		input_controls.Misc.Console.performed += wait(e => OnConsoleOpen?.Invoke(this, EventArgs.Empty));

		Action<InputAction.CallbackContext> wait(Action<InputAction.CallbackContext> fn)
		{
			return Utils.Coroutines.WaitForFixedUpdate(this, fn);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		Assert.IsTrue(LowLevel.ExistMouse || LowLevel.ExistKeyboard, "You must be using the Brain-Machine Interface. XD");
	}

	// Update is called once per frame
	void Update()
	{

	}

	void FixedUpdate()
	{
		if (LowLevel.ExistMouse)
		{
			WhenCursorOnScreen?.Invoke(this, new CursorEventArgs(LowLevel.ReadMousePosition()));
		}
	}

	void OnDestroy()
	{
		Mode = InputMode.Disabled;
	}

	private void when_mouse_confirm(InputAction.CallbackContext ctx)
	{
		CursorEventArgs e = new(ctx.ReadValue<Vector2>());

		Debug.Log($"{e.ScreenPos} -> {e.WorldPosMain}");

		var cell_pos = Chessboard.ChessboardManager.Instance.CastToBoardPos(e.WorldPosMain);
		InvokeEmplace(this, cell_pos);
	}
}
