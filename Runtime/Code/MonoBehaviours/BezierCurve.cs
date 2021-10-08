using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Common.MathStuff;

namespace Common
{
	namespace Path
	{
		public class BezierCurve : IPathObject
		{
			[SerializeField]
			private List<Vector3> _points;

			[SerializeField]
			private int _lineSteps;

			[SerializeField]
			private float _totalLength;

			// Points in Local Space
			public List<Vector3> Points { get => _points; set { _points = value; _totalLength = ApproximateLength(LineSteps); } }
			public int LineSteps { get => _lineSteps; set { _lineSteps = value; _totalLength = ApproximateLength(value); } }
			public override float Length { get { return _totalLength; } }

			void Awake()
			{
				_totalLength = ApproximateLength(LineSteps);
			}

			/// <summary>
			/// Finds the shortest distance between a point and the curve
			/// </summary>
			/// <param name="point">The point to find the distance from in World Space</param>
			/// <param name="subdivision_depth">Number of lines to subdivide the curve into lines</param>
			/// <returns>Shortest position to the Point</returns>
			public float ShortestDistanceFrom(Vector3 point, int subdivision_depth)
			{
				float shortest_distance = float.MaxValue;
				for (int i = 0; i < subdivision_depth; ++i)
				{
					LineStruct line = new LineStruct { A = GetPositionAt((float)i / subdivision_depth), B = GetPositionAt((float)(i + 1) / subdivision_depth) };

					float distance = LineUtility.DistanceFromLine(line, point);

					if (distance < shortest_distance)
					{
						shortest_distance = distance;
					}
				}

				return shortest_distance;
			}

			public float ShortestDistanceFrom(Vector3 point, int subdivision_depth, out float position)
			{
				float shortest_distance = float.MaxValue;
				float shortest_position = 0f;
				for (int i = 0; i < subdivision_depth; ++i)
				{
					LineStruct line = new LineStruct { A = GetPositionAt((float)i / subdivision_depth), B = GetPositionAt((float)(i + 1) / subdivision_depth) };

					float line_position = 0f;
					float distance = LineUtility.DistanceFromLine(line, point, out line_position);

					if (distance < shortest_distance)
					{
						shortest_distance = distance;
						shortest_position = MathMap.Map(line_position, 0f, 1f, (float)i / subdivision_depth, (float)(i + 1) / subdivision_depth);
					}
				}

				position = shortest_position;

				return shortest_distance;
			}

			public override Vector3 GetPositionAt(float t)
			{
				return transform.TransformPoint(Bezier.GetElement(Points.ToArray(), Mathf.Clamp01(t)));
			}

			public override Vector3 GetTangentAt(float t)
			{
				return transform.TransformDirection(Bezier.GetDerived(Points.ToArray(), t)).normalized;
			}

#if UNITY_EDITOR
			private void OnValidate()
			{
				_totalLength = ApproximateLength(LineSteps);
			}
#endif

			float ApproximateLength(int steps)
			{
				var length = 0f;
				Vector3 lastPos = GetPositionAt(0f);
				for (int i = 1; i <= steps; ++i)
				{
					float fac = i / (float)steps;

					Vector3 nextPos = GetPositionAt(fac);
					Vector3 diff = nextPos - lastPos;
					lastPos = nextPos;
					length += diff.magnitude;
				}
				return length;
			}

			public override void RecalculateLength()
			{
				_totalLength = ApproximateLength(LineSteps);
			}

			public void Reset()
			{
				_points = new List<Vector3>(new Vector3[]
				{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f)
				});
				LineSteps = 10;
			}

			public override Vector3 GetStart()
			{
				if (Points.Count < 1)
					return transform.position;

				return Points.First();
			}

			public override Vector3 GetEnd()
			{
				if (Points.Count < 1)
					return transform.position;

				return Points.Last();
			}

			public override void SetStart(Vector3 Point)
			{
				if (Points.Count < 1)
					transform.position = Point;

				Points[0] = transform.InverseTransformPoint(Point);
			}

			public override void SetEnd(Vector3 Point)
			{
				if (Points.Count < 1)
					transform.position = Point;

				Points[Points.Count - 1] = Point;
			}
		}

#if UNITY_EDITOR

		[CustomEditor(typeof(BezierCurve))]
		[CanEditMultipleObjects]
		public class BezierCurveInspector : Editor
		{
			private BezierCurve m_Curve;
			private Transform m_Trans;
			private Quaternion m_Rot;

			private const int DefaultLineSteps = 10;

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				if (GUILayout.Button("Recalculate Length"))
				{
					(target as BezierCurve).RecalculateLength();
				}
			}

			private void OnSceneGUI()
			{
				m_Curve = target as BezierCurve;
				if (m_Curve.Points.Count < 1)
					return;
				m_Trans = m_Curve.transform;
				m_Rot = Tools.pivotRotation == PivotRotation.Local ? m_Trans.rotation : Quaternion.identity;

				// Draw Transform Points
				Vector3[] new_points = new Vector3[m_Curve.Points.Count];
				for (int i = 0; i < m_Curve.Points.Count; ++i)
					new_points[i] = ShowPoint(i);


				// Draw Lines between Points
				Handles.color = Color.gray;

				Vector3 lastPoint = new_points[0];
				for (int i = 1; i < m_Curve.Points.Count; ++i)
				{
					Handles.DrawLine(lastPoint, new_points[i]);
					lastPoint = new_points[i];
				}


				// Draw The Bezier and Tangents
				Handles.color = Color.white;
				lastPoint = m_Curve.GetPositionAt(0f);
				int steps = m_Curve.LineSteps;
				for (int i = 1; i <= steps; ++i)
				{
					Vector3 nextPoint = m_Curve.GetPositionAt(i / (float)steps);
					// Draw Bezier Segment
					Handles.color = Color.white;
					Handles.DrawLine(lastPoint, nextPoint);

					// Draw Normal
					Vector3 gradient = m_Curve.GetTangentAt(i / (float)steps).normalized;

					Handles.color = Color.Lerp(Color.magenta, Color.yellow, i / (float)steps);
					Handles.DrawLine(nextPoint, nextPoint - gradient);

					lastPoint = nextPoint;
				}
			}

			private Vector3 ShowPoint(int index)
			{
				Vector3 point = m_Trans.TransformPoint(m_Curve.Points[index]);

				EditorGUI.BeginChangeCheck();
				point = Handles.DoPositionHandle(point, m_Rot);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(m_Curve, "Move Point");
					EditorUtility.SetDirty(m_Curve);
					m_Curve.Points[index] = m_Trans.InverseTransformPoint(point);
				}
				return point;
			}
		}
#endif

		public class Bezier
		{
			public static Vector3 GetElement(Vector3[] weights, float t)
			{
				var n = weights.Length - 1;
				if (n <= 1)
				{
					return Vector3.zero;
				}

				if (n == 1)
				{
					return weights[0] * t;
				}
				if (n == 2)
				{
					float t2 = t * t;
					float mt = 1.0f - t;
					float mt2 = mt * mt;
					float term1 = mt2; // 1 * t^3
					float term2 = 2 * mt * t;
					float term3 = t2;
					return weights[0] * term1 + weights[1] * term2 + weights[2] * term3;
				}
				if (n == 3)
				{
					float t2 = t * t;
					float t3 = t2 * t;
					float mt = 1.0f - t;
					float mt2 = mt * mt;
					float mt3 = mt2 * mt;
					float term1 = mt3;        // 1 * (1 - t)^3 * t^0
					float term2 = 3 * mt2 * t; // 3 * (1 - t)^2 * t^1
					float term3 = 3 * mt * t2; // 3 * (1 - t)^1 * t^2
					float term4 = t3;         // 1 * (1 - t)^0 * t^3
					return weights[0] * term1 + weights[1] * term2 + weights[2] * term3 + weights[3] * term4;
				}
				else
				{
					Vector3 val = Vector3.zero;
					for (int k = 0; k < weights.Length; ++k)
					{
						float term = Binomial.Gimme((uint)n, (uint)k) * Mathf.Pow(1.0f - t, n - k) * Mathf.Pow(t, k);
						val += weights[k] * term;
					}
					return val;
				}
			}

			public static Vector3 GetDerived(Vector3[] weights, float t)
			{
				var n = weights.Length - 1;
				if (n <= 1)
				{
					return Vector3.zero;
				}

				if (n == 1)
				{
					return Vector3.one;
				}
				if (n == 2)
				{
					float t2 = t * t;
					float mt = 1.0f - t;
					float term1 = 2 * -mt; // 1 * t^3
					float term2 = -2 * t + 2 * mt;
					float term3 = 2 * t;
					return Vector3.Normalize(weights[0] * term1 + weights[1] * term2 + weights[2] * term3);
				}
				if (n == 3)
				{
					//float t2 = t * t;
					float mt = 1.0f - t;
					//float mt2 = mt * mt;
					//float _3mt2 = 3 * mt2;
					//float term1 = -_3mt2;        // 1 * (1 - t)^3 * t^0
					//float term2 = 3 * mt * (3 * t - 1); // 3 * (1 - t)^2 * t^1
					//float term3 = 6 * t - 9 * t2; // 3 * (1 - t)^1 * t^2
					//float term4 = 3 * t2;         // 1 * (1 - t)^0 * t^3
					return Vector3.Normalize(3f * mt * mt * (weights[1] - weights[0]) + 6f * mt * t * (weights[2] - weights[1]) + 3f * t * t * (weights[3] - weights[2]));
					//return weights[0] * term1 + weights[1] * term2 + weights[2] * term3 + weights[3] * term4;
				}
				else
				{
					Vector3 val = Vector3.zero;
					for (int k = 0; k < weights.Length; ++k)
					{
						float a = Binomial.Gimme((uint)n, (uint)k);
						float mt = (1.0f - t);
						float b = n;
						float c = k;
						if (t == 1f)
							t = 0.9999999999f;
						float term = (a * Mathf.Pow(mt, b - c) * Mathf.Pow(t, c - 1) * (b * t - c)) / (t - 1);
						val += weights[k] * term;
					}
					return val.normalized;
				}
			}
		}

		public static class Binomial
		{
			private static List<List<uint>> CoEffs = new List<List<uint>>(new List<uint>[]{
						  new List<uint>(new uint[]{ 1u }),
						new List<uint>(new uint[]{ 1u, 1u }),
					  new List<uint>(new uint[]{ 1u, 2u, 1u }),
					new List<uint>(new uint[]{ 1u, 3u, 3u, 1u }),
				  new List<uint>(new uint[]{ 1u, 4u, 6u, 4u, 1u }),
				new List<uint>(new uint[]{ 1u, 5u, 10u, 10u, 5u, 1u }),
			  new List<uint>(new uint[]{ 1u, 6u, 15u, 20u, 15u, 6u, 1u }),
			new List<uint>(new uint[]{ 1u, 7u, 21u, 35u, 35u, 21u, 7u, 1u })
		});

			public static float Gimme(uint n, uint k)
			{
				while (n >= CoEffs.Count)
				{
					int s = CoEffs.Count;
					uint[] nextRow = new uint[s + 1];
					nextRow[0] = nextRow[s] = 1u;
					for (int i = s; i-- > 1;)
						nextRow[i] = CoEffs.Last()[i - 1] + CoEffs.Last()[i];
					CoEffs.Add(new List<uint>(nextRow));
				}
				return CoEffs[(int)n][(int)k];
			}
		}  
	}
}