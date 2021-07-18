using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Pool;

namespace Common
{
	namespace Behaviour
	{
		public class AudioEvent : MonoBehaviour
		{
			[SerializeField]
			private AudioSource _source;

			// Start is called before the first frame update
			private void OnEnable()
			{
				if (_source)
					_source.Play();
			}

			private void Start()
			{
				if (_source)
					_source.Play();
			}

			// Update is called once per frame
			void Update()
			{
				if (!_source.isPlaying)
				{
					Die();
				}
			}

			void Die()
			{
				gameObject.SetActive(false);
				if (TryGetComponent(out PooledGameObject pooler))
				{
					pooler.InUse = false;
				}
			}

			public static AudioSource Produce(AudioClip clip)
			{
				var source = AudioEventCreator.GetPooledAudio();
				source.Stop();
				source.clip = clip;

				source.gameObject.SetActive(false);
				source.gameObject.SetActive(true);

				source.pitch = 1f;
				source.volume = 1f;

				return source;
			}

			public static AudioSource Produce(AudioClip clip, Transform parent)
			{
				var source = Produce(clip);
				source.transform.parent = parent;
				return source;
			}

			public static AudioSource Produce(AudioClip clip, Vector3 position)
			{
				var source = Produce(clip);
				source.transform.position = position;
				return source;
			}

			public static AudioSource Produce(AudioClip clip, Vector3 position, Transform parent)
			{
				var source = Produce(clip);
				source.transform.position = position;
				source.transform.parent = parent;
				return source;
			}
		} 
	}

	namespace Extensions
	{
		public static class AudioSourceExtensions
		{
			public static AudioSource As3D(this AudioSource source, float amount = 1f)
			{
				source.spatialBlend = amount;
				return source;
			}

			public static AudioSource As2D(this AudioSource source)
			{
				source.spatialBlend = 0f;
				return source;
			}

			public static AudioSource VaryPitch(this AudioSource source, float variance = 0.2f)
			{
				source.pitch = Random.Range(1f - (variance * 0.5f), 1f + (variance * 0.5f));
				return source;
			}

			public static AudioSource VaryVolume(this AudioSource source, float variance = 0.25f)
			{
				source.volume = Random.Range(1f - variance, 1f);
				return source;
			}

			public static AudioSource WithPitch(this AudioSource source, float pitch)
			{
				source.pitch = pitch;
				return source;
			}

			public static AudioSource WithVolume(this AudioSource source, float volume)
			{
				source.volume = volume;
				return source;
			}
		}  
	}
}