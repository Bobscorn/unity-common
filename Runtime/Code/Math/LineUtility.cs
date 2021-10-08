using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Common
{
	namespace MathStuff
	{

		public struct PointPlane
		{
			public Vector3 Point;
			public Vector3 Normal;

			public override string ToString()
			{
				return $"P: {Point} + N: {Normal}";
			}

			public override bool Equals(object obj)
			{
				PointPlane other = (PointPlane)obj;
				return Mathf.Approximately(0f, Vector3.Distance(Normal, other.Normal)) && Mathf.Approximately(0f, Vector3.Distance(this.Point, other.Point));
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public static bool operator ==(PointPlane a, PointPlane b)
			{
				return a.Equals(b);
			}

			public static bool operator !=(PointPlane a, PointPlane b)
			{
				return !a.Equals(b);
			}
		}

		public struct DistancePlane
		{
			public float D;
			public Vector3 Normal; // Normalized

			public static explicit operator PointPlane(DistancePlane me)
			{
				return new PointPlane { Normal = me.Normal, Point = me.Normal * me.D };
			}
			public static explicit operator DistancePlane(PointPlane me)
			{
				return new DistancePlane { Normal = me.Normal, D = Vector3.Dot(me.Point, me.Normal) };
			}

			public override string ToString()
			{
				return $"N: {Normal} + D: {D}";
			}

			public override bool Equals(object obj)
			{
				DistancePlane other = (DistancePlane)obj;
				return Mathf.Approximately(0f, Vector3.Distance(Normal, other.Normal)) && Mathf.Approximately(this.D, other.D);
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public static bool operator ==(DistancePlane a, DistancePlane b)
			{
				return a.Equals(b);
			}

			public static bool operator !=(DistancePlane a, DistancePlane b)
			{
				return !a.Equals(b);
			}
		}


	} 
}