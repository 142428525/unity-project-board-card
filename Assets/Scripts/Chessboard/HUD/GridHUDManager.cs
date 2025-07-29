using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[ExecuteInEditMode]
public class GridHUDManager : Utils.MonoSingleton<GridHUDManager>
{
	[Header("Debug Mutable"), SerializeField, Range(0.3f, 1.0f)]
	private double highlight_size = 1.0;
	[SerializeField]
	private float alpha_0_w = 0.7f;
	[SerializeField]
	private float alpha_0_h = 0.75f;
	[SerializeField]
	private float alpha_1_w = 0.5f;
	[SerializeField]
	private float alpha_1_h = 0.6f;

	[Space()]
	public Transform GRID_HUD_ROOT;
	public Transform GRID_LINES;
	public GameObject HIGHLIGHT_PREFAB;
	public Material GRID_HIGHLIGHT;
	public CinemachineVirtualCamera VCAM;

	private GameObject highlight = null;    // 没有考虑支持多光标的打算
	private GameObject fake_highlight = null;
	private Vector2 screen_center = new(0.5f * Screen.width, 0.5f * Screen.height);
	private Bounds alpha_0 = new();
	private Bounds alpha_1 = new();

	protected override void Awake()
	{
		update_bounds();
	}

	// Start is called before the first frame update
	void Start()
	{
		add_event_listener();

		update_gird_lines_texture();

#if UNITY_EDITOR
		update_bounds();
#endif

		void add_event_listener()
		{
			InputManager.WhenCursorOnScreen += when_cursor_on_screen;
		}
	}

	// Update is called once per frame
	void Update()
	{
#if UNITY_EDITOR
		update_bounds();
		update_gird_lines_texture();
#endif
	}

	void FixedUpdate()
	{
		update_bounds();
	}

	void OnDrawGizmos()
	{
		var alpha0 = scr2wld(alpha_0);
		var alpha1 = scr2wld(alpha_1);

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(alpha0.center, 2 * alpha0.extents);
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(alpha1.center, 2 * alpha1.extents);

		Bounds scr2wld(Bounds a)
		{
			Bounds ret = new();
			ret.SetMinMax(Utils.CameraView.ScreenToWorldPos(Utils.CameraView.Type.UI, a.min),
				Utils.CameraView.ScreenToWorldPos(Utils.CameraView.Type.UI, a.max));
			return ret;
		}
	}

	public double GetHighlightSize() => highlight_size;

	[ContextMenu("Reset HUD Status")]
	public void SetDefault()
	{
		VCAM.ForceCameraPosition(new Vector3(0, 0, -10), Quaternion.identity);
		GRID_LINES.localPosition = Vector2.zero;
		update_gird_lines_texture();
	}

	public void ForceRefresh()
	{
		when_cursor_on_screen(this, new InputManager.CursorEventArgs(InputManager.LowLevel.ReadMousePosition()));
	}

	private void when_cursor_on_screen(object sender, InputManager.CursorEventArgs e)
	{
		if (highlight != null)
		{
			VCAM.Follow = fake_highlight.transform;
			highlight.transform.localPosition = e.GetWorldPos(Utils.CameraView.Type.UI);
			fake_highlight.transform.localPosition = e.WorldPosMain;

			var delta_pos = highlight.transform.position + VCAM.transform.position;
			GRID_HIGHLIGHT.SetFloat("_DeltaX", 0.2f * delta_pos.x);
			GRID_HIGHLIGHT.SetFloat("_DeltaY", 0.2f * delta_pos.y);
			// 魔法数字0.2f为进制基数10（大小网格的比例）的0.5倍（Grid.transform的尺寸）的倒数，不会改变
		}

		var point = e.ScreenPos;
		if (alpha_0.Contains(point) /*|| alpha_1.Contains(point)*/)
		{
			if (highlight == null)
			{
				reinstantiate();
			}

			if (!alpha_1.Contains(point))
			{
				var v = alpha_0.extents - alpha_1.extents;
				var inv_min_d = 1 / math.min(v.x, v.y);
				float alpha = math.clamp(1 - alpha_1.SqrDistance(point) * inv_min_d * inv_min_d, 0, 1);
				GRID_HIGHLIGHT.SetFloat("_Alpha", alpha);
			}
			else
			{
				GRID_HIGHLIGHT.SetFloat("_Alpha", 1);
			}
		}
		else
		{
			GRID_HIGHLIGHT.SetFloat("_Alpha", 0);

			if (!e.IsOnScreen())
			{
				Destroy(highlight);
				Destroy(fake_highlight);
				highlight = fake_highlight = null;
			}
			else if (highlight == null)
			{
				reinstantiate();
			}
		}

		update_gird_lines_texture();

		void reinstantiate()
		{
			highlight = Instantiate(HIGHLIGHT_PREFAB,
								e.GetWorldPos(Utils.CameraView.Type.UI),
								Quaternion.identity,
								GRID_HUD_ROOT);

			fake_highlight = new GameObject("FakeGridHighlight");
			fake_highlight.transform.SetParent(GRID_HUD_ROOT);
		}
	}

	private void update_bounds()
	{
		alpha_0.center = alpha_1.center = screen_center = new(0.5f * Screen.width, 0.5f * Screen.height); // NOTE: 暂时不写“屏幕尺寸变化”事件
		alpha_0.size = new(alpha_0_w * Screen.width, alpha_0_h * Screen.height);
		alpha_1.size = new(alpha_1_w * Screen.width, alpha_1_h * Screen.height);
	}

	private void update_gird_lines_texture()
	{
		var mgl = GRID_LINES.GetComponent<SpriteRenderer>().sharedMaterial;
		mgl.SetFloat("_DeltaX", 0.2f * VCAM.transform.position.x);
		mgl.SetFloat("_DeltaY", 0.2f * VCAM.transform.position.y);
		// 魔法数字0.2f为进制基数10（大小网格的比例）的0.5倍（Grid.transform的尺寸）的倒数，不会改变
	}
}
