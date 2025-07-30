using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class ScaleManager : Utils.MonoSingleton<ScaleManager>
{
	[Header("Debug Mutable"), SerializeField, Range(0.1001f, 1.0f)]
	private double scale_factor = DEFAULT_SCALE_FACTORS;
	[SerializeField, Range(0.05f, 0.5f)]
	private double scroll_speed = 0.15;

	[Space()]
	public Transform GRID;
	public CinemachineVirtualCamera VCAM;
	public Material GRID_LINES;
	public Material GRID_HIGHLIGHT;

	private const double DEFAULT_SCALE_FACTORS = 1.0;

	public double ScaleFactor
	{
		get { return scale_factor; }

		set
		{
			var f = math.clamp(value, 0.1001, 1.0);
			var inv_f = 1 / f;

			GRID_LINES.SetFloat("_Scale", (float)f);
			GRID_HIGHLIGHT.SetFloat("_Scale", (float)f);

			var g = f * GridHUDManager.Instance.GetHighlightSize();
			var inv_g = 1 / g;
			GRID_HIGHLIGHT.SetTextureScale("_MaskTexture", new Vector2((float)inv_g, (float)inv_g));
			GRID_HIGHLIGHT.SetTextureOffset("_MaskTexture", new Vector2((float)(0.5 * (1 - inv_g)), (float)(0.5 * (1 - inv_g))));

			var mouse_pos = Utils.CameraView.ScreenToWorldPos(Utils.CameraView.Type.Board, InputManager.LowLevel.ReadMousePosition());
			var dv = (Vector2)VCAM.transform.position - mouse_pos;
			Vector3 v = new((float)(mouse_pos.x + scale_factor * inv_f * dv.x), (float)(mouse_pos.y + scale_factor * inv_f * dv.y), -10);
			VCAM.ForceCameraPosition(v, Quaternion.identity);

			VCAM.m_Lens.OrthographicSize = (float)(5 * inv_f);

			scale_factor = f;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		add_event_listener();

#if !UNITY_EDITOR
		SetDefault();
#endif

		void add_event_listener()
		{
			InputManager.WhenScroll += on_scroll;
		}
	}

	// Update is called once per frame
	void Update()
	{
#if UNITY_EDITOR
		ScaleFactor = scale_factor;
#endif
	}

	[ContextMenu("Reset Scale")]
	public void SetDefault() => ScaleFactor = DEFAULT_SCALE_FACTORS;

	private void on_scroll(object sender, InputManager.ScrollEventArgs e)
	{
		Debug.Log($"{e.IsUp}, {e.Normalized}, {e.Value}");

		var ratio = 1 + scroll_speed * math.abs(e.Value) / 120;
		if (e.IsUp) // exp
		{
			ScaleFactor *= ratio;
		}
		else
		{
			ScaleFactor /= ratio;
		}

		GridHUDManager.Instance.ForceRefresh();
	}
}
