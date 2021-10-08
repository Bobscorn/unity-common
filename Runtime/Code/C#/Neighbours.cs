using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Stuff
	{
		[System.Serializable]
		public struct Neighbours2D<T>
		{
			public T PX;
			public T PY;
			public T NX;
			public T NY;

			public T this[Direction dir] { get => GetElement(dir); set => SetElement(dir, value); }
			public T this[int i] { get => GetElement(i); set => SetElement(i, value); }

			public T GetElement(Direction dir)
			{
				return GetElement((int)dir);
			}

			public T GetElement(int i)
			{
				switch (i)
				{
					default:
					case (int)Direction.EAST:
						return PX;
					case (int)Direction.NORTH:
						return PY;
					case (int)Direction.WEST:
						return NX;
					case (int)Direction.SOUTH:
						return NY;
				}
			}

			public T2 GetElement<T2>(Direction dir) where T2 : class, T
			{
				return GetElement(dir) as T2;
			}

			public T2 GetElement<T2>(int i) where T2 : class, T
			{
				return GetElement(i) as T2;
			}

			public void SetElement(Direction dir, T item)
			{
				SetElement((int)dir, item);
			}

			public void SetElement(int i, T item)
			{
				switch (i)
				{
					default:
					case (int)Direction.EAST:
						PX = item;
						break;
					case (int)Direction.NORTH:
						PY = item;
						break;
					case (int)Direction.WEST:
						NX = item;
						break;
					case (int)Direction.SOUTH:
						NY = item;
						break;
				}
			}

			public const int NumNeighbours = 4;
		}

		public struct Neighbours3D<T>
		{
			public T PX;
			public T PY;
			public T PZ;
			public T NX;
			public T NY;
			public T NZ;
		}
	}
}
