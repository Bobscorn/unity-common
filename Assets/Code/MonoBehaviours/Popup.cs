using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Common
{
	namespace Behaviour
	{
		public class Popup : MonoBehaviour
		{
			public UnityEvent OnClosed = new UnityEvent();
			public bool DestroyOnClose = false;

			public string Text { get => _text.text; set => _text.text = value; }
			public bool Active { get => gameObject.activeInHierarchy; set => gameObject.SetActive(value); }

			[SerializeField]
			private TextMeshProUGUI _text;

			private bool _clickedOnce;

			private void OnEnable()
			{
				_clickedOnce = false;
			}

			public void CloseButton()
			{
				if (DestroyOnClose)
					Destroy(gameObject);
				else
					gameObject.SetActive(false);
				OnClosed.Invoke();
			}

			public void OnClickedAnywhere()
			{
				if (_clickedOnce)
					CloseButton();
				else
					_clickedOnce = true;
			}
		}  
	}
}
