using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Extensions
	{
		public static class DictionaryExtensions
		{
			public static TValue GetOr<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue or)
			{
				if (!dict.ContainsKey(key))
					return or;

				return dict[key];
			}

			public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
			{
				if (!dict.ContainsKey(key))
					return default;

				return dict[key];
			}
		}
	}
}
