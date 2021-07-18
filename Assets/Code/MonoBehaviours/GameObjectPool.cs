using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Pool
	{
		public enum ExpandMode
		{
			EXPAND = 0,
			RECYCLE,
		}

		public class GameObjectPool : MonoBehaviour
		{
			public int TargetCount { get => _targetCount; }
			public ExpandMode ExpandMode { get => _expandMode; set => _expandMode = value; }

			[SerializeField]
			private int _targetCount;
			[SerializeField]
			private ExpandMode _expandMode = ExpandMode.EXPAND;
			[SerializeField]
			private GameObject _prefab;

			private List<PooledGameObject> _objects = new List<PooledGameObject>();

			private int _recycleIndex = 0;

			// Start is called before the first frame update
			void Start()
			{
				ExpandToFit();
			}

			PooledGameObject ExpandAndGet(int increase = 3)
			{
				if (increase < 1)
					throw new System.InvalidOperationException("Can not increase pool size negatively and return valid GameObject");

				int next = _targetCount;
				_targetCount += increase;
				ExpandToFit();
				return _objects[next];
			}

			public void ExpandToFit()
			{
				if (_targetCount < 0)
					_targetCount = 0;

				while (_objects.Count < _targetCount)
				{
					var go = Instantiate(_prefab);

					PooledGameObject pooled;
					if (go.GetComponent<PooledGameObject>())
						pooled = go.GetComponent<PooledGameObject>();
					else
						pooled = go.AddComponent<PooledGameObject>();

					_objects.Add(pooled);
				}
			}

			public void ShrinkToFit()
			{
				if (_targetCount < 0)
					_targetCount = 0;

				while (_objects.Count > _targetCount)
				{
					Destroy(_objects[_objects.Count - 1]);
					_objects.RemoveAt(_objects.Count - 1);
				}
			}

			public GameObject GetPooledObject()
			{
				for (int i = 0; i < _objects.Count; ++i)
				{
					if (!_objects[i].InUse)
					{
						var outgoing = _objects[i];
						outgoing.InUse = true;
						outgoing.gameObject.SetActive(true);
						return outgoing.gameObject;
					}
				}

				{
					if (_expandMode == ExpandMode.EXPAND)
					{
						var outgoing = ExpandAndGet();
						outgoing.InUse = true;
						outgoing.gameObject.SetActive(true);
						return outgoing.gameObject;
					}
					else
					{
						if (_objects.Count < 1)
							throw new System.InvalidOperationException("Can not recycle with zero objects in pool");

						return _objects[_recycleIndex++ % _objects.Count].gameObject;
					}
				}
			}

			public void Return(GameObject go)
			{
				var pooled = go.GetComponent<PooledGameObject>();
				if (!pooled)
					return;

				pooled.InUse = false;
				pooled.gameObject.SetActive(false);
			}
		}

	} 
}