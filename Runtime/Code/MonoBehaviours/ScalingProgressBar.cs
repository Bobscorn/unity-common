using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.MathStuff;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Common
{
	namespace Behaviour
	{
		public class ScalingProgressBar : MonoBehaviour
		{
			public Transform End;
			public Transform Target;
			[Tooltip("Size of the Target Mesh at a scale of 1")]
			[Range(0.001f, 100f)]
			public float TargetSize;

			[SerializeField]
			private float _targetScale = 1f;

			[SerializeField]
			[Range(0f, 1f)]
			private float _value;

			public float Value { get => _value; set => _value = value; }

			// Start is called before the first frame update
			void Start()
			{
				RotateToDirection(End.position - transform.position);
			}

			void RotateToDirection(Vector3 direction)
			{
				Target.Rotate(Vector3.Cross(Target.up, direction), Vector3.Angle(Target.up, direction));
			}

			// Update is called once per frame
			void Update()
			{
				if (Value <= 0f)
				{
					Target.gameObject.SetActive(false);
					return;
				}

				Target.gameObject.SetActive(true);
				Vector3 toEnd = End.position - transform.position;
				float mag = toEnd.magnitude;

				var scale = Target.localScale;
				scale.x = scale.z = _targetScale;
				scale.y = MathMap.Map(Value, 0f, 1f, 0f, mag / (TargetSize));
				Target.localScale = scale;

				Target.localPosition = toEnd / mag * scale.y * 0.5f;

				RotateToDirection(toEnd);
			}

#if UNITY_EDITOR
			private void OnValidate()
			{
				Update();
			}
#endif
		} 
	}

}