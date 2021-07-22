using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Behaviour
	{
		[RequireComponent(typeof(Rigidbody))]
		public class SetCenterOfMass : MonoBehaviour
		{
			[Tooltip("Relative to this transform's origin")]
			public Vector3 CenterOfMass;

			// Start is called before the first frame update
			void Start()
			{
				GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
			}

			private void OnDisable()
			{
				GetComponent<Rigidbody>().ResetCenterOfMass();
			}
		}  
	}
}
