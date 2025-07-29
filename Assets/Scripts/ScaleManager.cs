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
	[Header("Debug Mutable"), SerializeField, Range(0.2f, 1.0f)]
	private double scale_factor = DEFAULT_SCALE_FACTORS;
	[SerializeField, Range(0.01f, 0.5f)]
	private double scroll_speed = 0.1;

	[Space()]
	public Transform GRID;
	public CinemachineVirtualCamera VCAM;
	public Material GRID_LINES;
	public Material GRID_HIGHLIGHT;

	private readonly double[] DEFAULT_GRID_TRANSFORM_SCALE = { 0.5, 0.5 };  // 魔法数字0.5使它看起来大小合适
	private const double DEFAULT_SCALE_FACTORS = 1.0;

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
		set_scale_factors(scale_factor);
#endif
	}

	private void set_scale_factors(double f)
	{
		f = math.clamp(f, 0.2f, 1.0f);

		var grid_scale = DEFAULT_GRID_TRANSFORM_SCALE.Select(val => f * val).ToArray();
		GRID.localScale = new((float)grid_scale[0], (float)grid_scale[1]);
		GRID_LINES.SetFloat("_Scale", (float)f);
		GRID_HIGHLIGHT.SetFloat("_Scale", (float)f);

		var ratio = f / scale_factor;
		double[] v = { VCAM.transform.position.x * ratio, VCAM.transform.position.y * ratio };
		VCAM.ForceCameraPosition(new Vector3((float)v[0], (float)v[1], -10), Quaternion.identity);

		var g = f * GridHUDManager.Instance.GetHighlightSize();
		var inv_g = 1 / g;
		GRID_HIGHLIGHT.SetTextureScale("_MaskTexture", new Vector2((float)inv_g, (float)inv_g));
		GRID_HIGHLIGHT.SetTextureOffset("_MaskTexture", new Vector2((float)(0.5 * (1 - inv_g)), (float)(0.5 * (1 - inv_g))));

		scale_factor = f;
	}

	[ContextMenu("Reset Scale")]
	public void SetDefault() => set_scale_factors(DEFAULT_SCALE_FACTORS);

	public void Apply(double f) => set_scale_factors(f);

	public void Apply(Func<double, double> map) => set_scale_factors(map(scale_factor));

	private void on_scroll(object sender, InputManager.InputEventArgs<float> e)
	{
		Apply(f => f * math.pow(1 + scroll_speed, e.Value)); // exp
		GridHUDManager.Instance.ForceRefresh();
	}
}
