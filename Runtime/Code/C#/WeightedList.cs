using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Common
{
	namespace Stuff
	{
		[System.Serializable]
		public struct WeightedItem<T>
		{
			public T Item;
			public float Weight;
		}

		[System.Serializable]
		public class WeightedList<T> : List<WeightedItem<T>>
		{
			public T GetRandomItem()
			{
				if (Count < 1)
					throw new System.InvalidOperationException();
				var totalWeight = TotalWeight;

				var rand = Random.value;
				float offset = 0f;
				for (int i = 0; i < Count; ++i)
				{
					var item = this[i];
					var inverseWeight = item.Weight / totalWeight;
					if ((offset + inverseWeight) > rand)
						return item.Item;
					offset += inverseWeight;
				}
				throw new System.ArithmeticException("Failed to find item in list??");
			}

			public T GetRandomItemWhere(System.Predicate<T> predicate)
			{
				List<WeightedItem<T>> matching = new List<WeightedItem<T>>();

				for (int i = 0; i < Count; ++i)
				{
					if (predicate(this[i].Item))
						matching.Add(this[i]);
				}

				float totalWeight = 0f;
				matching.ForEach((WeightedItem<T> item) => totalWeight += item.Weight);

				var rand = Random.value;
				float offset = 0f;
				for (int i = 0; i < matching.Count; ++i)
				{
					var item = matching[i];
					var inverseWeight = item.Weight / totalWeight;
					if ((offset + inverseWeight) > rand)
						return item.Item;
					offset += inverseWeight;
				}
				throw new System.ArithmeticException("Failed to randomly generate a weighted item, double check you have set weights appropriately");
			}

			public bool TryGetRandomItemWhere(System.Predicate<T> predicate, out T matchingItem)
			{
				List<WeightedItem<T>> matching = new List<WeightedItem<T>>(this.Where((WeightedItem<T> item) => predicate(item.Item)));

				if (matching.Count < 1)
				{
					matchingItem = default;
					return false;
				}

				float totalWeight = 0f;
				matching.ForEach((WeightedItem<T> item) => totalWeight += item.Weight);

				var rand = Random.value;
				float offset = 0f;
				for (int i = 0; i < matching.Count; ++i)
				{
					var item = matching[i];
					var inverseWeight = item.Weight / totalWeight;
					if ((offset + inverseWeight) > rand)
					{
						matchingItem = item.Item;
						return true;
					}
					offset += inverseWeight;
				}

				matchingItem = default;
				return false;
			}

			public float TotalWeight { get { float weight = 0f; this.ForEach((WeightedItem<T> item) => weight += item.Weight); return weight; } }
		}  
	}
}
