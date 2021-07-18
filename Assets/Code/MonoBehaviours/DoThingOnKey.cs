using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
	namespace Behaviour
	{
		public class DoThingOnKey : MonoBehaviour
		{
			public KeyCode Key;
			public UnityEvent OnKeyDown = new UnityEvent();

			// Update is called once per frame
			void Update()
			{
				if (Input.GetKeyDown(Key))
					OnKeyDown.Invoke();
			}
		}  
	}
}
