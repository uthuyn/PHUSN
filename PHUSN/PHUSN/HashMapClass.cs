﻿using System.Collections.Generic;
internal static class HashMapClass
{
	internal static HashSet<KeyValuePair<TKey, TValue>> SetOfKeyValuePairs<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
	{
		HashSet<KeyValuePair<TKey, TValue>> entries = new HashSet<KeyValuePair<TKey, TValue>>();
		foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
		{
			entries.Add(keyValuePair);
		}
		return entries;
	}
}