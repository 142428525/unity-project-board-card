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
	[Header("Debug Mutable"), SerializeField, Range(0.5f, 1.0f)]
	private double grid_lines_size = 0.9;
	[SerializeField, Range(0.2f, 1.0f)]
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
	public Transform GRID_LINES_TRANSFORM;
	public GameObject HIGHLIGHT_PREFAB;
	public Material GRID_LINES;
	public Material GRID_HIGHLIGHT;
	public CinemachineVirtualCamera VCAM;

	private GameObject highlight = null; // 没有考虑支持多光标的打算
	private GameObject fake_highlight = null;
	private Bounds alpha_0 = new();
	private Bounds alpha_1 = new();

	// Start is called before the first frame update
	void Start()
	{
		add_event_listener();

		update_bounds();
		update_gird_lines();

		void add_event_listener()
		{
			//InputManager.WhenCursorOnScreen += when_cursor_on_screen;
		}
	}

	// Update is called once per frame
	void Update()
	{
#if UNITY_EDITOR
		update_bounds();
		update_gird_lines();
#endif
	}

	void FixedUpdate()
	{
		//InputManager.WhenCursorOnScreen -= when_cursor_on_screen;
		update_bounds();
		update_gird_lines();
		ForceRefresh();
	}

	void LateUpdate()
	{
#if !UNITY_EDITOR
		//ForceRefresh();

		// StartCoroutine(wait());
		// IEnumerator wait()
		// {
		// 	yield return new WaitForEndOfFrame();
		// 	ForceRefresh();
		// }
#endif
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
		grid_lines_size = 0.9;
		VCAM.ForceCameraPosition(new Vector3(0, 0, -10), Quaternion.identity);
		update_gird_lines();
	}

	public void ForceRefresh()
	{
		when_cursor_on_screen(null, new InputManager.CursorEventArgs(InputManager.LowLevel.ReadMousePosition()));
	}

	private void when_cursor_on_screen(object sender, InputManager.CursorEventArgs e)
	{
		if (highlight != null)
		{
			highlight.transform.localPosition = e.GetWorldPos(Utils.CameraView.Type.UI);
			fake_highlight.transform.localPosition = e.WorldPosMain;

			var delta_pos = highlight.transform.position + ((float)ScaleManager.Instance.ScaleFactor) * VCAM.transform.position;
			GRID_HIGHLIGHT.SetFloat("_DeltaX", (float)(0.1 * delta_pos.x));
			GRID_HIGHLIGHT.SetFloat("_DeltaY", (float)(0.1 * delta_pos.y));
			// 魔法数字0.1为大小网格线嵌套比例的倒数
		}

		var point = e.ScreenPos;
		if (alpha_0.Contains(point))
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

		update_gird_lines();Camera.main.GetComponent<CinemachineBrain>().ManualUpdate();

		void reinstantiate()
		{
			highlight = Instantiate(HIGHLIGHT_PREFAB,
				e.GetWorldPos(Utils.CameraView.Type.UI),
				Quaternion.identity,
				GRID_HUD_ROOT);

			var tmp = new GameObject("FakeGridHighlight");
			tmp.transform.SetParent(GRID_HUD_ROOT);
			tmp.transform.position = new(0, 0, GRID_HUD_ROOT.position.z);
			fake_highlight = tmp;

			VCAM.Follow = fake_highlight.transform;
		}
	}

	private void update_bounds()
	{
		alpha_0.center = alpha_1.center = new(0.5f * Screen.width, 0.5f * Screen.height); // NOTE: 暂时不写“屏幕尺寸变化”事件
		alpha_0.size = new(alpha_0_w * Screen.width, alpha_0_h * Screen.height);
		alpha_1.size = new(alpha_1_w * Screen.width, alpha_1_h * Screen.height);
	}

	private void update_gird_lines()
	{
		GRID_LINES_TRANSFORM.localPosition = VCAM.transform.position;
		GRID_LINES_TRANSFORM.localScale = new((float)(20 / ScaleManager.Instance.ScaleFactor), (float)(20 / ScaleManager.Instance.ScaleFactor));

		GRID_LINES.SetFloat("_DeltaX", (float)(0.1 * ScaleManager.Instance.ScaleFactor * VCAM.transform.position.x));
		GRID_LINES.SetFloat("_DeltaY", (float)(0.1 * ScaleManager.Instance.ScaleFactor * VCAM.transform.position.y));
		// 魔法数字0.1为大小网格线嵌套比例的倒数

		var inv = 1 / grid_lines_size;
		GRID_LINES.SetTextureScale("_MaskTexture", new Vector2((float)inv, (float)inv));
		GRID_LINES.SetTextureOffset("_MaskTexture", new Vector2((float)(0.5 * (1 - inv)), (float)(0.5 * (1 - inv))));
	}
}
