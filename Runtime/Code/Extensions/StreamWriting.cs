using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Common
{
	namespace Extensions
	{
		public static class StreamWritingExtensions
		{
			public static void Write(this BinaryWriter writer, Matrix4x4 mat)
			{
				for (int i = 0; i < 4; ++i)
				{
					var row = mat.GetRow(i);
					writer.Write(row.x);
					writer.Write(row.y);
					writer.Write(row.z);
					writer.Write(row.w);
				}
			}
			public static void WriteLineMatrix4x4(this StreamWriter writer, Matrix4x4 mat)
			{
				for (int i = 0; i < 4; ++i)
				{
					var row = mat.GetRow(i);
					writer.WriteLine($"{row.x:0.###}");
					writer.WriteLine($"{row.y:0.###}");
					writer.WriteLine($"{row.z:0.###}");
					writer.WriteLine($"{row.w:0.###}");
				}
			}

			public static Matrix4x4 ReadMatrix4x4(this BinaryReader reader)
			{
				var mat = new Matrix4x4();
				for (int i = 0; i < 4; ++i)
				{
					Vector4 row = new Vector4();
					row.x = reader.ReadSingle();
					row.y = reader.ReadSingle();
					row.z = reader.ReadSingle();
					row.w = reader.ReadSingle();
					mat.SetRow(i, row);
				}
				return mat;
			}

			public static Matrix4x4 ReadMatrix4x4(this StreamReader reader)
			{
				var mat = new Matrix4x4();
				for (int i = 0; i < 4; ++i)
				{
					Vector4 row = new Vector4();
					row.x = float.Parse(reader.ReadLine());
					row.y = float.Parse(reader.ReadLine());
					row.z = float.Parse(reader.ReadLine());
					row.w = float.Parse(reader.ReadLine());
					mat.SetRow(i, row);
				}
				return mat;
			}
		}
	} 
}