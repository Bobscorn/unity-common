using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Behaviour
	{
		public class AudioEventCreator : MonoBehaviour
		{
			public Pool.GameObjectPool AudioEventPool;

			private static AudioEventCreator _instance;

			private void OnEnable()
			{
				_instance = this;
			}

			private void OnDisable()
			{
				if (_instance == this)
					_instance = null;
			}

			public static AudioSource GetPooledAudio()
			{
				return _instance.AudioEventPool.GetPooledObject().GetComponent<AudioSource>();
			}
		}  
	}
}
