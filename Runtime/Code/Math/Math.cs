using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.MathStuff;

namespace Common
{
	public static class Math
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
			return MathMap.Map(val, minin, maxin, minout, maxout);
		}

		/// <summary>
		/// Identical to <see cref="Map(float, float, float, float, float)"/> but output is clamped to the output range
		/// </summary>
		/// <param name="val">The input value</param>
		/// <param name="minin">Lower bound of input range [inclusive]</param>
		/// <param name="maxin">Upper bound of input range [inclusize]</param>
		/// <param name="minout">Lower bound of output range [inclusize]</param>
		/// <param name="maxout">Upper bound of output range [inclusize]</param>
		/// <returns>Value converted to output range (and clamped)</returns>
		public static float MapClamped(float val, float minin, float maxin, float minout, float maxout)
		{
			return MathMap.MapClamped(val, minin, maxin, minout, maxout);
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
			return MathMap.InRange(val, rangeMin, rangeMax);
		}

		public static float InRangeClamp(float val, float rangeMin, float rangeMax)
		{
			return MathMap.InRangeClamp(val, rangeMin, rangeMax);
		}

		public static int IndexOf(float val, int rangeMin, int rangeMax)
		{
			return MathMap.IndexOf(val, rangeMin, rangeMax);
		}

		// Range Min [Inclusive] - Range Max [Exclusive]
		public static int IndexOfClamp(float val, int rangeMin, int rangeMax)
		{
			return MathMap.IndexOfClamp(val, rangeMin, rangeMax);
		}

		/// <summary>
		/// Finds the nearest multiple of <paramref name="multipleOf"/> to <paramref name="val"/>
		/// </summary>
		/// <param name="val">Target value to find nearest multiple of</param>
		/// <param name="multipleOf">The number to be a multiple of</param>
		/// <returns>The multiple of <paramref name="multipleOf"/> nearest <paramref name="val"/></returns>
		public static float NearestMultipleOf(float val, float multipleOf)
		{
			float halfMultiple = multipleOf * 0.5f;

			val += halfMultiple;

			return val - (val % multipleOf);
		}

		/// <summary>
		/// Finds the Vector2 with x and y values as multiples of <paramref name="multipleOf"/> nearest to <paramref name="val"/>'s x and y values
		/// </summary>
		/// <param name="val">Target value to find nearest multiple of</param>
		/// <param name="multipleOf">The number to be a multiple of</param>
		/// <returns>The multiple of <paramref name="multipleOf"/> nearest <paramref name="val"/></returns>
		public static Vector2 NearestMultipleOf(Vector2 val, float multipleOf)
		{
			float halfMultiple = multipleOf * 0.5f;

			val.x += (val.x < 0f ? -halfMultiple : halfMultiple);
			val.y += (val.y < 0f ? -halfMultiple : halfMultiple);

			val.x -= val.x % multipleOf;
			val.y -= val.y % multipleOf;

			return val;
		}
	} 
}
