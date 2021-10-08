using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace MathStuff
	{
		public static class MathMap
		{
			/// <summary>
			/// Given an input value expected between range [<paramref name="minin"/>, <paramref name="maxin"/>] outputs the equivalent value in range [<paramref name="maxin"/>, <paramref name="maxout"/>]
			/// </summary>
			/// <param name="val">The input value</param>
			/// <param name="minin">Lower bound of input range [inclusive]</param>
			/// <param name="maxin">Upper bound of input range [inclusize]</param>
			/// <param name="minout">Lower bound of output range [inclusize]</param>
			/// <param name="maxout">Upper bound of output range [inclusize]</param>
			/// <returns>Value converted to output range</returns>
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

			/// <summary>
			/// Finds the percentage that <paramref name="val"/> is between <paramref name="rangeMin"/> and <paramref name="rangeMax"/>
			/// </summary>
			/// <param name="val">Position between <paramref name="rangeMin"/> and <paramref name="rangeMax"/> (can be outside but result will be non-normalized)</param>
			/// <param name="rangeMin">The lower bound of the range</param>
			/// <param name="rangeMax">The upper bound of the range</param>
			/// <returns>0..1 when <paramref name="val"/> is between <paramref name="rangeMin"/> and <paramref name="rangeMax"/></returns>
			public static float InRange(float val, float rangeMin, float rangeMax)
			{
				float inRange = rangeMax - rangeMin;

				if (inRange == 0f)
					return 0f;

				val = val - rangeMin;
				return val / inRange;
			}


			/// <summary>
			/// Finds the percentage that <paramref name="val"/> is between <paramref name="rangeMin"/> and <paramref name="rangeMax"/> and clamps output to 0..1
			/// </summary>
			/// <param name="val">Position between <paramref name="rangeMin"/> and <paramref name="rangeMax"/> (can be outside but result will be non-normalized)</param>
			/// <param name="rangeMin">The lower bound of the range</param>
			/// <param name="rangeMax">The upper bound of the range</param>
			/// <returns>Guaranteed normalized percentage that <paramref name="val"/> is through range <paramref name="rangeMin"/>..<paramref name="rangeMax"/></returns>
			public static float InRangeClamp(float val, float rangeMin, float rangeMax)
			{
				return Mathf.Clamp(InRange(val, rangeMin, rangeMax), rangeMin, rangeMax);
			}

			/// <summary>
			/// Finds nearest integer that is <paramref name="val"/> percentage through the range <paramref name="rangeMin"/> -> <paramref name="rangeMax"/>
			/// </summary>
			/// <param name="val">The input value between 0..1</param>
			/// <param name="rangeMin">Lower bound of Range (inclusive)</param>
			/// <param name="rangeMax">Upper bound of Range (inclusize)</param>
			/// <returns>The index between the range or 0 if <paramref name="rangeMin"/> == <paramref name="rangeMax"/></returns>
			public static int IndexOf(float val, int rangeMin, int rangeMax)
			{
				float outRange = rangeMax - rangeMin;
				if (outRange == 0f)
					return 0;

				return rangeMin + Mathf.RoundToInt(val * outRange);
			}

			/// <summary>
			/// Identical to <see cref="IndexOf(float, int, int)"/> but clamps output between <paramref name="rangeMin"/> and <paramref name="rangeMax"/>
			/// </summary>
			/// <param name="val">The input value between 0..1</param>
			/// <param name="rangeMin">Lower bound of Range (inclusive)</param>
			/// <param name="rangeMax">Upper bound of Range (inclusize)</param>
			/// <returns>The index between the range (clamped) or 0 if <paramref name="rangeMin"/> == <paramref name="rangeMax"/></returns>
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