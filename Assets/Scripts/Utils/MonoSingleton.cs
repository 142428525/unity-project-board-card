using UnityEngine;

namespace Utils
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;
		private static bool is_quitting = false;

		/// <summary>
		/// 获取单例对象
		/// </summary>
		public static T Instance
		{
			get
			{
				if (is_quitting)
				{
					Debug.LogWarning($"The singleton instance of {typeof(T)} has already destroyed.");
					return null;
				}

				if (instance == null)
				{
					instance = FindObjectOfType<T>();

					if (instance == null)
					{
						instance = new GameObject($"[Singleon] {typeof(T).Name}").AddComponent<T>();
					}
				}

				return instance;
			}
		}

		protected virtual void Awake()
		{
			if (instance == null)
			{
				instance = this as T;
				//DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		protected virtual void OnApplicationQuit()
		{
			is_quitting = true;
		}
	}
}