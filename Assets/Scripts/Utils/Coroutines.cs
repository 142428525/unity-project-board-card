using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	public static class Coroutines
	{
		public static Action<T> WaitForFixedUpdate<T>(MonoBehaviour caller, Action<T> fn)
		{
			return new Action<T>(e =>
			{
				caller.StartCoroutine(wait());

				IEnumerator wait()
				{
					yield return new WaitForFixedUpdate();
					fn(e);
				}
			});
		}
	}
}
