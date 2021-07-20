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

			private float _time;

			// Update is called once per frame
			void Update()
			{
				if ((_time += Time.deltaTime) > FuseTime)
				{
					Destroy(gameObject);
				}
			}
		}  
	}
}
