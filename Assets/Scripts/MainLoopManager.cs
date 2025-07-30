using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLoopManager : MonoBehaviour
{
	[Header("Debug Mutable"), SerializeField]
	private uint tps = DEFAULT_TPS;

	private const int DEFAULT_TPS = 50; // unity did this

	// Start is called before the first frame update
	void Start()
	{
		InputManager.Instance.Mode = InputManager.InputMode.Player; // for test only
	}

	// Update is called once per frame
	void Update()
	{

	}

	void FixedUpdate()
	{
		Time.fixedDeltaTime = (float)(1.0 / tps);
	}
}
