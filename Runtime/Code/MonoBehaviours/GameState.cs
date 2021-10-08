using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
	namespace Behaviour
	{
		public class GameState : MonoBehaviour
		{
			public UnityEvent OnGameWin = new UnityEvent();
			public UnityEvent OnGameLose = new UnityEvent();

			public UnityEvent OnGameStart = new UnityEvent();

			public UnityEvent OnGamePause = new UnityEvent();

			public bool PauseOnLose;
			public bool PauseOnWin;

			[SerializeField]
			private bool _paused = false;

			[SerializeField]
			private float _timeScale = 1f;

			public float TimeScale { get => _timeScale; set { UpdateTimeScale(value); } }

			public bool IsPaused { get => _paused; }


			private void Start()
			{
				if (_paused)
				{
					_paused = false;
					PauseGame();
				}
				else
				{
					_paused = true;
					ResumeGame();
				}
			}

			public void Win()
			{
				OnGameWin.Invoke();

				if (PauseOnWin)
					PauseGame();
			}

			public void Lose()
			{
				OnGameLose.Invoke();

				if (PauseOnLose)
					PauseGame();
			}

			public void PauseGame()
			{
				if (_paused)
					return;

				Time.timeScale = 0f;
				_paused = true;
			}

			public void ResumeGame()
			{
				if (!_paused)
					return;

				Time.timeScale = 1f;
				_paused = false;
			}

			public void UpdateTimeScale(float timeScale)
			{
				if (!_paused)
					Time.timeScale = timeScale;
				_timeScale = timeScale;
			}
		}
	}
}
