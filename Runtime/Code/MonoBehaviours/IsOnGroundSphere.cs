using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Common
{
	namespace Behaviour
	{
		public class IsOnGroundSphere : IsOnGround
		{
			[Header("Config")]
			public Vector3 Offset;
			public float Radius;
			[Tooltip("The minimum required dot product with the UP vector that a surface normal must have to be considered 'flat enough' (The arc-sine of the smallest angle considered flat, value of 1 means only 0* difference, value of 0.707 means up to 45*)")]
			public float MinNormalDot;

			[Header("Filtering")]
			public LayerMask Layers;
			public string IgnoreTag;
			public List<Collider> IgnoreColliders;

			[Header("Update Mode")]
			public UpdateMode UpdateMode;

			[SerializeField]
			[Header("OnGround Value")]
			private bool _onGround = false;
			[SerializeField]
			[Header("Last Frame Count we were on the ground")]
			private int _lastFrameOnGround = 0;

			public override bool OnGround { get => _onGround; }
			public override int LastFrameOnGround { get => _lastFrameOnGround; }


			private RaycastHit[] _hits = new RaycastHit[20];


			void Update()
			{
				if (UpdateMode == UpdateMode.UPDATE)
				{
					Recheck();
				}
			}

			void FixedUpdate()
			{
				if (UpdateMode == UpdateMode.FIXED_UPDATE)
				{
					Recheck();
				}
			}

			void LateUpdate()
			{
				if (UpdateMode == UpdateMode.LATE_UPDATE)
				{
					Recheck();
				}
			}


			public void Recheck()
			{
				int numHit = Physics.SphereCastNonAlloc(transform.position + Offset, Radius, Vector3.down, _hits, 0.001f, Layers);

				for (int i = 0; i < numHit; ++i)
				{
					var hit = _hits[i];

					if (IgnoreColliders.Contains(hit.collider))
						continue;

					if (IgnoreTag.Length > 0 && hit.collider.CompareTag(IgnoreTag))
						continue;

					if (Vector3.Dot(hit.normal, Vector3.up) >= MinNormalDot)
					{
						_onGround = true;
						_lastFrameOnGround = Time.frameCount;
						break;
					}
				}
			}
		}

#if UNITY_EDITOR
		[CustomEditor(typeof(IsOnGroundSphere))]
		[CanEditMultipleObjects]
		public class IsOnGroundSphereEditor : Editor
		{
			private void OnSceneGUI()
			{
				var p = target as IsOnGroundSphere;

				Handles.color = Color.green;
				p.Radius = Handles.RadiusHandle(p.transform.rotation, p.transform.position + p.Offset, p.Radius, false);
			}
		}
#endif
	}
}
