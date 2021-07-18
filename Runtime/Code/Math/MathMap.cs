using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Math
	{
		public static class MathMap
		{
			public static float Map(float val, float minin, float maxin, float minout, float maxout)
			{
				// Convert value of minin - maxin to proportional value between minout - maxout
				float inrange = maxin - minin;
				float outrange = maxout - minout;
				if (inrange == 0f || outrange == 0f)
					return 0f; // Avoid division by zero

				val = val - minin;
				val = val / inrange;
				val = minout + (val * outrange);
				return val;
			}

			// Map but with output clamped to minout (inclusive) -> maxout (inclusive)
			public static float MapClamped(float val, float minin, float maxin, float minout, float maxout)
			{
				return Mathf.Clamp(Map(val, minin, maxin, minout, maxout), minout, maxout);
			}

			public static float InRange(float val, float rangeMin, float rangeMax)
			{
				float inRange = rangeMax - rangeMin;

				if (inRange == 0f)
					return 0f;

				val = val - rangeMin;
				return val / inRange;
			}

			public static float InRangeClamp(float val, float rangeMin, float rangeMax)
			{
				return Mathf.Clamp(InRange(val, rangeMin, rangeMax), rangeMin, rangeMax);
			}

			public static int IndexOf(float val, int rangeMin, int rangeMax)
			{
				float outRange = rangeMax - rangeMin;
				if (outRange == 0f)
					return 0;

				return rangeMin + Mathf.RoundToInt(val * outRange);
			}

			// Range Min [Inclusive] - Range Max [Exclusive]
			public static int IndexOfClamp(float val, int rangeMin, int rangeMax)
			{
				float outRange = rangeMax - rangeMin;
				if (outRange == 0f)
					return 0;

				return Mathf.Clamp(rangeMin + Mathf.RoundToInt(val * outRange), rangeMin, rangeMax - 1);
			}
		}

	} 
}