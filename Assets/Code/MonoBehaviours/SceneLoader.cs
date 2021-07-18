using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Common
{
	namespace Behaviour
	{
		public class SceneLoader : MonoBehaviour
		{
			public UnityEvent OnSceneStartLoad = new UnityEvent();
			public UnityEvent OnSceneLoaded = new UnityEvent();

			private AsyncOperation _loadOperation = null;


			// Update is called once per frame
			void Update()
			{
				if (_loadOperation != null)
				{
					if (_loadOperation.progress >= 0.9f)
					{
						_loadOperation.allowSceneActivation = true;

						_loadOperation = null;

						OnSceneLoaded.Invoke();
					}
				}
			}

			public void LoadScene(string name)
			{
				Debug.Log($"Loading Scene: {name}");
				if (_loadOperation != null)
				{
					Debug.LogWarning("Tried to load scene while another is loading");
					return;
				}

				_loadOperation = SceneManager.LoadSceneAsync(name);
				_loadOperation.allowSceneActivation = false;

				OnSceneStartLoad.Invoke();
			}
		}  
	}
}
