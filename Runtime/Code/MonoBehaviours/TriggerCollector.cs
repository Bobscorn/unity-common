using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Behaviour
	{
		public class TriggerCollector : MonoBehaviour
		{
			public List<Collider> CollidingWith;

			private void OnTriggerEnter(Collider other)
			{
				CollidingWith.Add(other);
			}

			private void OnTriggerExit(Collider other)
			{
				CollidingWith.Remove(other);
			}
		}  
	}
}
