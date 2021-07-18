using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Common
{
	namespace Behaviour
	{
		public class FPSCounter : MonoBehaviour
		{
			public TextMeshProUGUI CounterText;

			// Start is called before the first frame update
			void Start()
			{

			}

			// Update is called once per frame
			void Update()
			{
				CounterText.text = $"FPS: {1f / Time.unscaledDeltaTime}";
			}
		}  
	}
}
