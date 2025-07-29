using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLoopManager : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		InputManager.Instance.Mode = InputManager.InputMode.Player; // for test only
	}

	// Update is called once per frame
	void Update()
	{

	}
}
