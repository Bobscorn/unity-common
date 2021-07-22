using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Behaviour
	{
		public class SelfDestruct : MonoBehaviour
		{
			public float FuseTime;

			[Tooltip("Instead of Destroying this GameObject return it to a GameObjectPool")]
			public bool ReturnToPool;

			private float _time;

			// Update is called once per frame
			void Update()
			{
				if ((_time += Time.deltaTime) > FuseTime)
				{
					if (!ReturnToPool)
					{
						Destroy(gameObject);
					}
					else
					{
						if (TryGetComponent(out Pool.PooledGameObject pooledObj))
						{
							pooledObj.Return();
						}
						else
						{
							Debug.LogWarning($"Destroying '{gameObject.name}' instead of returning to GameObjectPool as no PooledGameObject component found");
							Destroy(gameObject);
						}
					}
				}
			}
		}  
	}
}
