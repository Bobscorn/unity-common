using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Stuff
	{
		public enum Orientation
		{
			NORTH_SOUTH = 0,
			EAST_WEST,
		}

		public static class OrientationExtensions
		{
			public static Quaternion RotationBetween(this Orientation orientation, Orientation other)
			{
				if (orientation == other)
					return Quaternion.identity;

				if (orientation == Orientation.NORTH_SOUTH)
					return Quaternion.AngleAxis(90f, Vector3.forward);

				return Quaternion.AngleAxis(-90f, Vector3.forward);
			}

			public static Quaternion ToRotation(this Orientation orientation)
			{
				switch (orientation)
				{
					default:
					case Orientation.NORTH_SOUTH:
						return Quaternion.identity;
					case Orientation.EAST_WEST:
						return Quaternion.AngleAxis(90f, Vector3.forward);
				}
			}

			public static Orientation TurnedBy(this Orientation orient, int turns)
			{
				if (Mathf.Abs(turns % 2) == 1)
					if (orient == Orientation.NORTH_SOUTH)
						return Orientation.EAST_WEST;
					else
						return Orientation.NORTH_SOUTH;

				return orient;
			}

			public static void GetDirections(this Orientation orient, out Direction upperDir, out Direction lowerDir)
			{
				if (orient == Orientation.NORTH_SOUTH)
				{
					upperDir = Direction.NORTH;
					lowerDir = Direction.SOUTH;
				}
				else
				{
					upperDir = Direction.EAST;
					lowerDir = Direction.WEST;
				}
			}
		}
	}
}
