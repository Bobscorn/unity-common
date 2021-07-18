using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Pool
	{
		[DisallowMultipleComponent]
		public class PooledGameObject : MonoBehaviour
		{
			public bool InUse = false;
		}  
	}
}
