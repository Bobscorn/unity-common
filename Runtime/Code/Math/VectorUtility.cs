using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace MathStuff
	{
		public static class VectorUtility
		{
			// Converts a direction to an equivalent angular velocity that would roll a ball in the direction across an x-z plane
			public static Vector3 DirToRoll(Vector3 dir)
			{
				dir.y = 0f;
				dir.Normalize();

				Vector3 angular = new Vector3(dir.z, 0f, -dir.x);

				return angular;
			}
		}  
	}
}
