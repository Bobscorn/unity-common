using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Common.Math;

namespace Common
{
	namespace Path
	{
		public class Line : IPathObject
		{
			[SerializeField]
			private Vector3 _a, _b;

			public Vector3 A { get => _a; set { _a = value; _length = CalculateLength(); } }
			public Vector3 B { get => _b; set { _b = value; _length = CalculateLength(); } }

			[SerializeField]
			private float _length;

			public override float Length { get => _length; }

			public override Vector3 GetPositionAt(float t)
			{
				return transform.TransformPoint(Vector3.Lerp(A, B, t));
			}

			public override Vector3 GetTangentAt(float t)
			{
				return transform.TransformDirection(B - A).normalized;
			}

			public float CalculateLength()
			{
				return (transform.TransformPoint(B) - transform.TransformPoint(A)).magnitude;
			}

			public override void RecalculateLength()
			{
				_length = CalculateLength();
			}

			public override Vector3 GetStart()
			{
				return transform.TransformPoint(A);
			}

			public override Vector3 GetEnd()
			{
				return transform.TransformPoint(B);
			}

			public override void SetStart(Vector3 Point)
			{
				A = transform.InverseTransformPoint(Point);
			}

			public override void SetEnd(Vector3 Point)
			{
				B = transform.InverseTransformPoint(Point);
			}

#if UNITY_EDITOR
			private void OnValidate()
			{
				_length = CalculateLength();
			}
#endif
		}

#if UNITY_EDITOR
		[CustomEditor(typeof(Line))]
		public class LineEditor : Editor
		{
			private void OnSceneGUI()
			{
				Line line = target as Line;
				Transform trans = line.transform;
				Quaternion rot = trans.rotation;
				Vector3 a = trans.TransformPoint(line.A);
				Vector3 b = trans.TransformPoint(line.B);

				Handles.color = Color.white;
				Handles.DrawLine(a, b);

				EditorGUI.BeginChangeCheck();
				a = Handles.DoPositionHandle(a, rot);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(line, "Move Point");
					EditorUtility.SetDirty(line);
					line.A = trans.InverseTransformPoint(a);
				}

				EditorGUI.BeginChangeCheck();
				b = Handles.DoPositionHandle(b, rot);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(line, "Move Point");
					EditorUtility.SetDirty(line);
					line.B = trans.InverseTransformPoint(b);
				}
				//Handles.DoPositionHandle(b, rot);
			}
		}
#endif

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

		public struct LineStruct
		{
			// World Space Coords
			public Vector3 A;
			public Vector3 B;

			public static implicit operator LineStruct(Line me)
			{
				return new LineStruct { A = me.transform.TransformPoint(me.A), B = me.transform.TransformPoint(me.B) };
			}
		}

		public static class LineUtility
		{
			public static float DistanceFromLine(LineStruct line, Vector3 point, out float position)
			{
				// Normal for the planes
				Vector3 plane_normal = (line.B - line.A).normalized;
				DistancePlane A_plane = (DistancePlane)new PointPlane { Normal = -plane_normal, Point = line.A };
				DistancePlane B_plane = (DistancePlane)new PointPlane { Normal = plane_normal, Point = line.B };

				// Test if point is front of first plane
				float a_dot = Vector3.Dot(point, A_plane.Normal);
				if (a_dot >= A_plane.D)
				{
					Vector3 difference = (line.A - point);
					position = 0f;
					return difference.magnitude;
				}
				// Test if point is front of second plane
				else
				{
					float b_dot = Vector3.Dot(point, B_plane.Normal);
					if (b_dot >= B_plane.D)
					{
						Vector3 difference = (line.B - point);
						position = 1f;
						return difference.magnitude;
					}
					else
					{
						// Point is in between the planes
						float line_length = (line.B - line.A).magnitude;

						// Reuse the first plane
						A_plane.Normal = -A_plane.Normal;
						A_plane.D = -A_plane.D;

						float pos_along_line = (Vector3.Dot(point, A_plane.Normal) - A_plane.D);
						position = pos_along_line / line_length;
						return (point - Vector3.Lerp(line.A, line.B, position)).magnitude;
					}
				}
			}

			public static float DistanceFromLine(LineStruct line, Vector3 point)
			{
				// Normal for the planes
				Vector3 plane_normal = (line.B - line.A).normalized;
				DistancePlane A_plane = (DistancePlane)new PointPlane { Normal = -plane_normal, Point = line.A };
				DistancePlane B_plane = (DistancePlane)new PointPlane { Normal = plane_normal, Point = line.B };

				// Test if point is front of first plane
				float a_dot = Vector3.Dot(point, A_plane.Normal);
				if (a_dot >= A_plane.D)
				{
					Vector3 difference = (line.A - point);
					return difference.magnitude;
				}
				// Test if point is front of second plane
				else
				{
					float b_dot = Vector3.Dot(point, B_plane.Normal);
					if (b_dot >= B_plane.D)
					{
						Vector3 difference = (line.B - point);
						return difference.magnitude;
					}
					else
					{
						// Point is in between the planes
						float line_length = (line.B - line.A).magnitude;

						// Reuse the first plane
						A_plane.Normal = -A_plane.Normal;
						A_plane.D = -A_plane.D;

						float pos_along_line = (Vector3.Dot(point, A_plane.Normal) - A_plane.D);
						float position = pos_along_line / line_length;
						return (point - Vector3.Lerp(line.A, line.B, position)).magnitude;
					}
				}
			}

			public static float DistanceFromLine(LineStruct line, Vector3 point, out Vector3 closestPoint)
			{// Normal for the planes
				Vector3 plane_normal = (line.B - line.A).normalized;
				DistancePlane A_plane = (DistancePlane)new PointPlane { Normal = -plane_normal, Point = line.A };
				DistancePlane B_plane = (DistancePlane)new PointPlane { Normal = plane_normal, Point = line.B };

				// Test if point is front of first plane
				if (Vector3.Dot(point, A_plane.Normal) > A_plane.D)
				{
					closestPoint = line.A;
					return (line.A - point).magnitude;
				}
				// Test if point is front of second plane
				else if (Vector3.Dot(point, B_plane.Normal) > B_plane.D)
				{
					closestPoint = line.B;
					return (line.B - point).magnitude;
				}
				else
				{
					// Point is in between the planes
					float line_length = (line.B - line.A).magnitude;

					// Reuse the first plane
					A_plane.Normal = -A_plane.Normal;

					float pos_along_line = (Vector3.Dot(A_plane.Normal, point) - A_plane.D);
					float position = pos_along_line / line_length;
					closestPoint = Vector3.Lerp(line.A, line.B, position);
					return (point - closestPoint).magnitude;
				}
			}
		}  
	}
}