using System;

namespace Utils
{
	public class Singleton<T> where T : class, new()
	{
		private static readonly Lazy<Singleton<T>> lazy = new(() => new Singleton<T>());

		public static Singleton<T> Instance { get { return lazy.Value; } }
		
		protected Singleton()
		{
		}
	}
}