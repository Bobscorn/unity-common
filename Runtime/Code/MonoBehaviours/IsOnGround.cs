using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Behaviour
	{
		public abstract class IsOnGround : MonoBehaviour
		{
			public abstract bool OnGround { get; }
			public abstract int LastFrameOnGround { get; }
		}  
	}
}
