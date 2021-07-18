using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Behaviour
	{
		public class SingleInstanceDontDestroy : MonoBehaviour
		{
			private static SingleInstanceDontDestroy _instance;

			void Awake()
			{
				if (_instance)
					Destroy(gameObject);
				else
				{
					_instance = this;
					DontDestroyOnLoad(gameObject);
				}
			}

			// Update is called once per frame
			void Update()
			{

			}
		}  
	}
}
