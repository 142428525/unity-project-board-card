using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleManager : Utils.MonoSingleton<ScaleManager>
{
	[Header("Debug Mutable"), SerializeField, Range(0.2f, 1.0f)]
	private float scale_factor = DEFAULT_SCALE_FACTORS;
	[SerializeField, Range(0.01f, 0.5f)]
	private float scroll_speed = 0.1f;

	[Space()]
	public Transform GRID;
	public CinemachineVirtualCamera VCAM;
	public Material GRID_LINES;
	public Material GRID_HIGHLIGHT;

	private readonly Vector3 DEFAULT_GRID_TRANSFORM_SCALE = new(0.5f, 0.5f, 0.0f);  // 魔法数字0.5f使它看起来大小合适
	private const float DEFAULT_SCALE_FACTORS = 1.0f;

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

	private void set_scale_factors(float f)
	{
		f = math.clamp(f, 0.2f, 1.0f);

		VCAM.ForceCameraPosition(VCAM.transform.position * f / scale_factor, Quaternion.identity);

		scale_factor = f;
		GRID.localScale = f * DEFAULT_GRID_TRANSFORM_SCALE;
		GRID_LINES.SetFloat("_Scale", f);
		GRID_HIGHLIGHT.SetFloat("_Scale", f);

		f *= GridHUDManager.Instance.GetHighlightSize();
		float inv_f = 1 / f;
		GRID_HIGHLIGHT.SetTextureScale("_MaskTexture", new Vector2(inv_f, inv_f));
		GRID_HIGHLIGHT.SetTextureOffset("_MaskTexture", new Vector2(0.5f * (1 - inv_f), 0.5f * (1 - inv_f)));
	}

	[ContextMenu("Reset Scale")]
	public void SetDefault()
	{
		set_scale_factors(DEFAULT_SCALE_FACTORS);
	}

	public void Apply(float f)
	{
		set_scale_factors(f);
	}

	public void Apply(Func<float, float> map)
	{
		set_scale_factors(map(scale_factor));
	}

	private void on_scroll(object sender, InputManager.InputEventArgs<float> e)
	{
		//Debug.Log(e.Value);

		Apply(f => f + f * scroll_speed * e.Value); // exp
	}
}
