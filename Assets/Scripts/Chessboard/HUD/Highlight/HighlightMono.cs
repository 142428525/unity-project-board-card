using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[ExecuteInEditMode]
public class HighlightMono : MonoBehaviour
{
	//[Header("Debug Mutable"), SerializeField, Range(0.3f, 1.0f)]
	//private float highlight_size = DEFAULT_SIZE;

	//[Space()]
	public Bounds ALPHA_0_BOUND;
	public Bounds ALPHA_1_BOUND;

	private Transform HIGHLIGHT;
	private Material GRID_HIGHLIGHT;

	//private readonly Vector3 DEFAULT_HIGHLIGHT_TRANSFORM_SCALE = new(5, 5, 0);
	//private const float DEFAULT_SIZE = 1.0f;

	// public float Size
	// {
	// 	get { return highlight_size; }

	// 	set
	// 	{
	// 		highlight_size = value;

	// 		HIGHLIGHT.localScale = value * DEFAULT_HIGHLIGHT_TRANSFORM_SCALE;
	// 		GRID_HIGHLIGHT.SetFloat("_GraduationScale", value);
	// 	}
	// }

	void Awake()
	{
		HIGHLIGHT = gameObject.transform;
		GRID_HIGHLIGHT = gameObject.GetComponent<SpriteRenderer>().sharedMaterial;  // 以引用指向那个Material
	}

	// Start is called before the first frame update
	void Start()
	{
		add_event_listener();

		// read from setting...

		// TODO: 计划加入允许用户调整高光颜色的设置项

		void add_event_listener()
		{
			InputManager.WhenCursorMove += on_cursor_move;
		}
	}

	// Update is called once per frame
	void Update()
	{
#if UNITY_EDITOR
		//Size = highlight_size;
#endif
	}

	void FixedUpdate()
	{

	}

	// [ContextMenu("Reset Size")]
	// public void SetDefault()
	// {
	// 	Size = DEFAULT_SIZE;
	// }

	private void on_cursor_move(object sender, InputManager.CursorEventArgs e)
	{
		var point = e.WorldPos;
		var alpha_0 = ALPHA_0_BOUND.Contains(point);
		var alpha_1 = ALPHA_1_BOUND.Contains(point);
		var dist = ALPHA_1_BOUND.SqrDistance(point);

		if (alpha_0)
		{

		}
	}
}
