using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Stuff
	{
		[System.Serializable]
		public enum Direction
		{
			NORTH = 0,
			EAST,
			SOUTH,
			WEST
		}

		[System.Serializable]
		public enum DirectionWithNone
		{
			NONE = 0,
			NORTH,
			EAST,
			SOUTH,
			WEST
		}

		public static class DirectionExtensions
		{
			public static bool IsNotNone(this DirectionWithNone dir)
			{
				return dir != DirectionWithNone.NONE;
			}

			public static bool IsNone(this DirectionWithNone dir)
			{
				return dir == DirectionWithNone.NONE;
			}

			public static bool TryGetDirection(this DirectionWithNone dir, out Direction outDir)
			{
				switch (dir)
				{
					case DirectionWithNone.NORTH:
						outDir = Direction.NORTH;
						return true;
					case DirectionWithNone.EAST:
						outDir = Direction.EAST;
						return true;
					case DirectionWithNone.SOUTH:
						outDir = Direction.SOUTH;
						return true;
					case DirectionWithNone.WEST:
						outDir = Direction.WEST;
						return true;
					default:
					case DirectionWithNone.NONE:
						outDir = default;
						return false;
				}
			}

			public static Direction GetDirection(this DirectionWithNone dir)
			{
				switch (dir)
				{
					case DirectionWithNone.NORTH:
						return Direction.NORTH;
					case DirectionWithNone.EAST:
						return Direction.EAST;
					case DirectionWithNone.SOUTH:
						return Direction.SOUTH;
 					case DirectionWithNone.WEST:
						return Direction.WEST;
					default:
					case DirectionWithNone.NONE:
						throw new System.ArgumentException("Can not convert was None to Direction", nameof(dir));
 				}
			}

			public static DirectionWithNone ToNoneDir(this Direction dir)
			{
				switch (dir)
				{
					default:
					case Direction.NORTH:
						return DirectionWithNone.NORTH;
					case Direction.EAST:
						return DirectionWithNone.EAST;
					case Direction.SOUTH:
						return DirectionWithNone.SOUTH;
					case Direction.WEST:
						return DirectionWithNone.WEST;
				}
			}

			public static Direction FromNum(int num)
			{
				switch ((num % 4 + 4) % 4)
				{
					default:
					case (int)Direction.NORTH:
						return Direction.NORTH;
					case (int)Direction.EAST:
						return Direction.EAST;
					case (int)Direction.SOUTH:
						return Direction.SOUTH;
					case (int)Direction.WEST:
						return Direction.WEST;
				}
			}

			public static int ToInt(this Direction dir)
			{
				return (int)dir;
			}

			public static Direction TurnedBy(this Direction dir, int turns)
			{
				return FromNum((int)dir + turns);
			}

			public static Direction Inverted(this Direction dir) => TurnedBy(dir, 2);

			public static Direction TurnedClockwise(this Direction dir, int turns = 1)
			{
				return dir.TurnedBy(turns);
			}

			public static Direction TurnedCounterClockwise(this Direction dir, int turns = 1)
			{
				return dir.TurnedBy(-turns);
			}

			public static T NeighbourIn<T>(this Neighbours2D<T> neighbours, Direction dir)
			{
				switch (dir)
				{
					default:
					case Direction.NORTH:
						return neighbours.PY;
					case Direction.EAST:
						return neighbours.PX;
					case Direction.SOUTH:
						return neighbours.NY;
					case Direction.WEST:
						return neighbours.NX;
				}
			}

			public static Vector2 ToVector(this Direction dir)
			{
				switch (dir)
				{
					default:
					case Direction.NORTH:
						return Vector2.up;
					case Direction.EAST:
						return Vector2.right;
					case Direction.SOUTH:
						return Vector2.down;
					case Direction.WEST:
						return Vector2.left;
				}
			}

			public static Vector2Int ToVecInt(this Direction dir)
			{
				switch (dir)
				{
					default:
					case Direction.NORTH:
						return Vector2Int.up;
					case Direction.EAST:
						return Vector2Int.right;
					case Direction.SOUTH:
						return Vector2Int.down;
					case Direction.WEST:
						return Vector2Int.left;
				}
			}

			public static Quaternion RotationBetween(this Direction dir, Direction other)
			{
				return Quaternion.AngleAxis(Vector3.SignedAngle(dir.ToVector(), other.ToVector(), Vector3.forward), Vector3.forward);
			}

			public static Quaternion ToRotation(this Direction dir)
			{
				return RotationBetween(default, dir);
			}
		}
	}
}
