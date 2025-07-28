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
	private float highlight_size = 1.0f;
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
	public CinemachineVirtualCamera VCAM;

	private GameObject highlight = null;    // 没有考虑支持多光标的打算
	private Vector2 screen_center = new(0.5f * Screen.width, 0.5f * Screen.height);
	private Bounds alpha_0 = new();
	private Bounds alpha_1 = new();

	protected override void Awake()
	{
		alpha_0.center = alpha_1.center = screen_center;
		update_bounds();
	}

	// Start is called before the first frame update
	void Start()
	{
		add_event_listener();

		update_gird_lines_texture();

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
		// for test only
		screen_center = new(0.5f * Screen.width, 0.5f * Screen.height); // NOTE: 暂时不写“屏幕尺寸变化”事件
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
			ret.SetMinMax(Camera.main.ScreenToWorldPoint(a.min), Camera.main.ScreenToWorldPoint(a.max));
			return ret;
		}
	}

	public float GetHighlightSize()
	{
		return highlight_size;
	}

	[ContextMenu("Reset HUD Status")]
	public void SetDefault()
	{
		VCAM.ForceCameraPosition(new Vector3(0, 0, -10), Quaternion.identity);
		GRID_LINES.localPosition = Vector2.zero;
		update_gird_lines_texture();
	}

	private void when_cursor_on_screen(object sender, InputManager.CursorEventArgs e)
	{
		Material m = null;
		if (highlight != null)
		{
			m = highlight.GetComponent<SpriteRenderer>().sharedMaterial;
		}

		if (highlight != null)
		{
			VCAM.Follow = highlight.transform;
			highlight.transform.position = e.WorldPos;

			m.SetFloat("_DeltaX", 0.2f * highlight.transform.position.x);
			m.SetFloat("_DeltaY", 0.2f * highlight.transform.position.y);
			// 魔法数字0.2f为进制基数10（大小网格的比例）的0.5倍（Grid.transform的尺寸）的倒数，不会改变
		}

		var point = e.ScreenPos;
		if (alpha_0.Contains(point) && m != null)
		{
			if (!alpha_1.Contains(point))
			{
				var v = alpha_0.extents - alpha_1.extents;
				var inv_min_d = 1 / math.min(v.x, v.y);
				float alpha = math.clamp(1 - alpha_1.SqrDistance(point) * inv_min_d * inv_min_d, 0, 1);
				m.SetFloat("_Alpha", alpha);
			}
			else
			{
				m.SetFloat("_Alpha", 1);
			}
		}
		else
		{
			if (highlight != null)
			{
				highlight.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_Alpha", 0);
			}

			Bounds screen_border = new();
			screen_border.SetMinMax(Vector2.zero, 2 * screen_center);
			if (!screen_border.Contains(point))
			{
				Destroy(highlight);
				highlight = null;
			}
			else if (highlight == null)
			{
				highlight = Instantiate(HIGHLIGHT_PREFAB, e.WorldPos, Quaternion.identity, GRID_HUD_ROOT);
			}
		}

		var cam_pos = VCAM.transform.position;
		GRID_LINES.localPosition = new(cam_pos.x, cam_pos.y);
		update_gird_lines_texture();
	}

	private void update_bounds()
	{
		alpha_0.size = new Vector2(alpha_0_w * Screen.width, alpha_0_h * Screen.height);
		alpha_1.size = new Vector2(alpha_1_w * Screen.width, alpha_1_h * Screen.height);
	}

	private void update_gird_lines_texture()
	{
		var mgl = GRID_LINES.GetComponent<SpriteRenderer>().sharedMaterial;
		mgl.SetFloat("_DeltaX", 0.2f * GRID_LINES.position.x);
		mgl.SetFloat("_DeltaY", 0.2f * GRID_LINES.position.y);
		// 魔法数字0.2f为进制基数10（大小网格的比例）的0.5倍（Grid.transform的尺寸）的倒数，不会改变
	}
}
