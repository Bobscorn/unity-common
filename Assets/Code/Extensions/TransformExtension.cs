using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	namespace Extensions
	{
		public static class TransformExtensions
		{
			public static void From(this Transform transform, Matrix4x4 matrix)
			{
				transform.localPosition = matrix.ExtractTranslation();
				transform.localRotation = matrix.ExtractRotation();
				transform.localScale = matrix.ExtractScale();
			}

			public static Matrix4x4 ToMat4x4(this Transform transform)
			{
				return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
			}
		}

		public static class MatrixExtensions
		{
			public static Vector3 ExtractTranslation(this Matrix4x4 mat)
			{
				Vector3 translate;
				translate.x = mat.m03;
				translate.y = mat.m13;
				translate.z = mat.m23;
				return translate;
			}

			public static Quaternion ExtractRotation(this Matrix4x4 mat)
			{
				Vector3 forward;
				forward.x = mat.m02;
				forward.y = mat.m12;
				forward.z = mat.m22;

				Vector3 upwards;
				upwards.x = mat.m01;
				upwards.y = mat.m11;
				upwards.z = mat.m21;

				return Quaternion.LookRotation(forward, upwards);
			}

			public static Vector3 ExtractScale(this Matrix4x4 mat)
			{
				Vector3 scale;
				scale.x = new Vector4(mat.m00, mat.m10, mat.m20, mat.m30).magnitude;
				scale.y = new Vector4(mat.m01, mat.m11, mat.m21, mat.m31).magnitude;
				scale.z = new Vector4(mat.m02, mat.m12, mat.m22, mat.m32).magnitude;
				return scale;
			}
		}  
	}
}
