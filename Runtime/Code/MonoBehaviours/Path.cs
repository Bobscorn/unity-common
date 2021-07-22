using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Common.Math;

namespace Common
{
	namespace Path
	{
		public abstract class IPathObject : MonoBehaviour
		{
			// World Space
			public abstract float Length { get; }

			public float DistanceToT(float distance)
			{
				return distance / Length;
			}

			public abstract void RecalculateLength();
			// World Space
			public abstract Vector3 GetPositionAt(float t); // 0..1 range
															// World Space
			public abstract Vector3 GetTangentAt(float t); // 0..1 range

			// World Space
			public abstract Vector3 GetStart();
			// World Space
			public abstract Vector3 GetEnd();
			// World Space
			public abstract void SetStart(Vector3 Point);
			// World Space
			public abstract void SetEnd(Vector3 Point);
		}

		[System.Serializable]
		public struct PathDistanceInfo
		{
			public Vector3 Point;
			public float Distance;
		}

		public class Path : IPathObject
		{
			[SerializeField]
			private List<IPathObject> m_PathSegments;

			private List<IPathObject> m_RealSegments = new List<IPathObject>();

			[SerializeField]
			[Tooltip("The length of the entire path excluding spaces between (The path does not traverse the spaces)")]
			private float m_Length;

			[SerializeField]
			[Tooltip("The length of the entire path including spaces between (The path does not traverse the spaces)")]
			private float m_InclusiveLength;

			public float InclusiveLength { get => m_InclusiveLength; }
			public override float Length { get => m_Length; }
			public IReadOnlyList<IPathObject> PathSegments { get => m_RealSegments; }

			void Start()
			{
				GenerateRealList();
				RecalculateLength();
			}

			void Reset()
			{
				m_PathSegments = new List<IPathObject>();
				m_Length = 0;
				GenerateRealList();
			}

#if UNITY_EDITOR
			private void OnValidate()
			{
				GenerateRealList();
			}
#endif

			void GenerateRealList()
			{
				m_RealSegments.Clear();
				foreach (var obj in m_PathSegments)
				{
					if (obj)
						m_RealSegments.Add(obj);
				}
			}

			public void AddObject(IPathObject obj)
			{
				m_PathSegments.Add(obj);
				GenerateRealList();
				if (obj is BezierCurve curve)
					curve.RecalculateLength();
				m_Length += obj.Length;
			}

			public void Clear()
			{
				m_PathSegments.Clear();
				GenerateRealList();
			}

			/// <summary>
			/// Finds the normalized floating point representing the nearest point on the Path
			/// </summary>
			/// <param name="position">The World Space position to find the nearest position to</param>
			/// <returns>Normalized Position along this Path closest to the position</returns>
			public float GetTFromPosition(Vector3 position, float bezier_precision = 0.5f)
			{
				if (PathSegments.Count < 1)
					return 0f;
				// Basic search
				// Keep iterating over segments until the distance starts increasing
				IPathObject nearest_segment = null;
				float shortest_distance = float.MaxValue;
				float path_pos = 0f;
				float nearest_position = 0f;
				float nearest_path_start = 0f, nearest_path_length = 0f;

				for (int i = 0; i < PathSegments.Count; ++i)
				{
					var segment = PathSegments[i];
					float distance = 0f, local_pos = 0f;
					if (segment is BezierCurve curve)
					{
						distance = curve.ShortestDistanceFrom(position, 5, out local_pos);
					}
					else if (segment is Line line)
					{
						distance = LineUtility.DistanceFromLine(line, position, out local_pos);
					}

					float segment_length = (segment.Length / m_Length);
					if (distance < shortest_distance)
					{
						if (segment is BezierCurve)
						{
							nearest_path_start = path_pos;
							nearest_path_length = segment.Length / m_Length;
						}
						shortest_distance = distance;
						nearest_segment = segment;
						nearest_position = MathMap.Map(local_pos, 0f, 1f, path_pos, path_pos + segment_length);
					}
					path_pos += segment_length;
				}

				// Refine the search if its a bezier
				if (nearest_segment is BezierCurve pcurve)
				{
					int subdivisions = Mathf.Max((int)(pcurve.Length / bezier_precision), 5);
					float local_pos;
					shortest_distance = pcurve.ShortestDistanceFrom(position, subdivisions, out local_pos);
					nearest_position = MathMap.Map(local_pos, 0f, 1f, nearest_path_start, nearest_path_start + nearest_path_length);
				}

				return nearest_position;
			}

			public Vector3 GetNearestPoint(Vector3 position, float bezier_precision = 0.5f)
			{
				if (PathSegments.Count < 1)
					return Vector3.positiveInfinity;
				// Basic search
				// Keep iterating over segments until the distance starts increasing
				IPathObject nearest_segment = null;
				float shortest_distance = float.MaxValue;
				float path_pos = 0f;
				float nearest_position = 0f;
				Vector3 nearest_point = Vector3.zero;

				for (int i = 0; i < PathSegments.Count; ++i)
				{
					var segment = PathSegments[i];
					float distance = 0f, local_pos = 0f;
					if (segment is BezierCurve curve)
					{
						distance = curve.ShortestDistanceFrom(position, 5, out local_pos);
					}
					else if (segment is Line line)
					{
						distance = LineUtility.DistanceFromLine(line, position, out local_pos);
					}

					float segment_length = (segment.Length / m_Length);
					if (distance < shortest_distance)
					{
						shortest_distance = distance;
						nearest_segment = segment;
						nearest_position = MathMap.Map(local_pos, 0f, 1f, path_pos, path_pos + segment_length);
						nearest_point = segment.GetPositionAt(local_pos);
					}
					path_pos += segment_length;
				}

				// Refine the search if its a bezier
				if (nearest_segment is BezierCurve pcurve)
				{
					int subdivisions = Mathf.Max((int)(pcurve.Length / bezier_precision), 5);
					float local_pos;
					pcurve.ShortestDistanceFrom(position, subdivisions, out local_pos);
					return pcurve.GetPositionAt(local_pos);
				}

				return nearest_point;
			}

			public Vector3 GetNearestPoint(Vector3 position, out float nearest_t, float bezier_precision = 0.5f)
			{
				if (PathSegments.Count < 1)
				{
					nearest_t = default;
					return Vector3.positiveInfinity;
				}
				// Basic search
				// Keep iterating over segments until the distance starts increasing
				IPathObject nearest_segment = null;
				float shortest_distance = float.MaxValue;
				float path_pos = 0f;
				nearest_t = 0f;
				Vector3 nearest_point = Vector3.zero;

				for (int i = 0; i < PathSegments.Count; ++i)
				{
					var segment = PathSegments[i];
					float distance = 0f, local_pos = 0f;
					if (segment is BezierCurve curve)
					{
						distance = curve.ShortestDistanceFrom(position, 5, out local_pos);
					}
					else if (segment is Line line)
					{
						distance = LineUtility.DistanceFromLine(line, position, out local_pos);
					}

					float segment_length = (segment.Length / m_Length);
					if (distance < shortest_distance)
					{
						shortest_distance = distance;
						nearest_segment = segment;
						nearest_t = MathMap.Map(local_pos, 0f, 1f, path_pos, path_pos + segment_length);
						nearest_point = segment.GetPositionAt(local_pos);
					}
					path_pos += segment_length;
				}

				// Refine the search if its a bezier
				if (nearest_segment is BezierCurve pcurve)
				{
					int subdivisions = Mathf.Max((int)(pcurve.Length / bezier_precision), 5);
					float local_pos;
					pcurve.ShortestDistanceFrom(position, subdivisions, out local_pos);
					return pcurve.GetPositionAt(local_pos);
				}

				return nearest_point;
			}

			public Vector3 GetNearestPoint(Vector3 position, out Vector3 tangent, float bezier_precision = 0.5f)
			{
				tangent = Vector3.zero;
				if (PathSegments.Count < 1)
					return Vector3.positiveInfinity;

				// Basic search
				// Keep iterating over segments until the distance starts increasing
				IPathObject nearest_segment = null;
				float shortest_distance = float.MaxValue;
				float path_pos = 0f;
				float nearest_position = 0f;
				Vector3 nearest_point = Vector3.zero;

				for (int i = 0; i < PathSegments.Count; ++i)
				{
					var segment = PathSegments[i];
					float distance = 0f, local_pos = 0f;
					if (segment is BezierCurve curve)
					{
						distance = curve.ShortestDistanceFrom(position, 5, out local_pos);
					}
					else if (segment is Line line)
					{
						distance = LineUtility.DistanceFromLine(line, position, out local_pos);
					}

					float segment_length = (segment.Length / m_Length);
					if (distance < shortest_distance)
					{
						shortest_distance = distance;
						nearest_segment = segment;
						nearest_position = MathMap.Map(local_pos, 0f, 1f, path_pos, path_pos + segment_length);
						nearest_point = segment.GetPositionAt(local_pos);
						tangent = segment.GetTangentAt(local_pos);
					}
					path_pos += segment_length;
				}

				// Refine the search if its a bezier
				if (nearest_segment is BezierCurve pcurve)
				{
					int subdivisions = Mathf.Max((int)(pcurve.Length / bezier_precision), 5);
					float local_pos;
					pcurve.ShortestDistanceFrom(position, subdivisions, out local_pos);
					return pcurve.GetPositionAt(local_pos);
				}

				return nearest_point;
			}

			public List<PathDistanceInfo> CalculateDistances(Vector3 position)
			{
				if (PathSegments.Count < 1)
					return new List<PathDistanceInfo>();

				List<PathDistanceInfo> info = new List<PathDistanceInfo>();

				// Basic search

				for (int i = 0; i < PathSegments.Count; ++i)
				{
					var segment = PathSegments[i];
					float distance = 0f, local_pos = 0f;
					if (segment is BezierCurve curve)
					{
						distance = curve.ShortestDistanceFrom(position, 5, out local_pos);
						info.Add(new PathDistanceInfo { Distance = distance, Point = segment.GetPositionAt(local_pos) });
					}
					else if (segment is Line line)
					{
						distance = LineUtility.DistanceFromLine(line, position, out local_pos);
						info.Add(new PathDistanceInfo { Distance = distance, Point = segment.GetPositionAt(local_pos) });
					}
				}

				return info;
			}

			/// <summary>
			/// Calculate a vector pointing to a point distance units (world space) along the curve ahead of position
			/// </summary>
			/// <param name="position">The position along the path the distance is ahead of/behind</param>
			/// <param name="distance">The World Space distance along the curve the position is trying to move to</param>
			/// <returns></returns>
			[Obsolete("Function is now obsolete - it will not return what you expect")]
			public Vector3 GetTargetVelocityTo(float position, float distance)
			{
				// Normalize distance
				distance /= m_Length;

				Vector3 pos_vec = GetPositionAt(position);
				Vector3 dis_vec = GetPositionAt(position + distance);

				return dis_vec - pos_vec;
			}

			/// <summary>
			/// Finds the nearest position on the path
			/// </summary>
			/// <param name="position">World Space Position to find closest point to</param>
			/// <returns>Nearest Position</returns>
			public Vector3 GetNearestPosition(Vector3 position, float bezier_precision = 0.5f)
			{
				return GetPositionAt(GetTFromPosition(position, bezier_precision));
			}

			/// <summary>
			/// Converts a length along the path from world space to normalized
			/// Equivalent to going length / path.Length
			/// </summary>
			/// <param name="length"></param>
			/// <returns>Normalized length along the path</returns>
			public float ConvertLength(float length)
			{
				return length / Length;
			}

			// World Space
			public override Vector3 GetPositionAt(float t)
			{
				t = Mathf.Clamp01(t);
				float t_offset = 0f;
				foreach (var segment in PathSegments)
				{
					float segment_length_local = segment.Length / m_Length;
					bool above_floor = t > (t_offset - float.Epsilon);
					bool below_roof = t <= (t_offset + segment_length_local + 0.00001f);
					if (above_floor && below_roof)
					{
						// Map global t to segment local t
						float local_t = MathMap.Map(t, t_offset, t_offset + segment_length_local, 0f, 1f);
						Vector3 pos = segment.GetPositionAt(local_t);
						return pos;
					}
					t_offset += segment_length_local;
				}

				Debug.Log("Path couldn't Return position, likely due to incorrect Length");
				return Vector3.zero;
			}

			// World Space
			public override Vector3 GetTangentAt(float t)
			{
				float pos = 0f;
				for (int i = 0; i < PathSegments.Count; ++i)
				{
					float segment_length_local = PathSegments[i].Length / m_Length;
					bool above_floor = t > (pos - float.Epsilon);
					bool below_roof = t <= (pos + segment_length_local + 0.000001f);
					if (above_floor && below_roof)
					{
						float local_t = MathMap.Map(t, pos, pos + segment_length_local, 0f, 1f);
						return PathSegments[i].GetTangentAt(local_t);
					}
					pos += segment_length_local;
				}

				return Vector3.zero;
			}

			// World Space
			public override Vector3 GetStart()
			{
				if (PathSegments.Count < 1)
					return Vector3.zero;

				return PathSegments.First().GetStart();
			}

			// World Space
			public override Vector3 GetEnd()
			{
				if (PathSegments.Count < 1)
					return Vector3.zero;

				return PathSegments.Last().GetEnd();
			}

			public override void SetEnd(Vector3 Point)
			{
				if (PathSegments.Count < 1)
					return;

				PathSegments.Last().SetEnd(Point);
			}

			public override void SetStart(Vector3 Point)
			{
				if (PathSegments.Count < 1)
					return;

				PathSegments.First().SetStart(Point);
			}

			// Calculates Length including spaces between Path Segments
			public float CalculateInclusiveLength()
			{
				float length = 0f;
				Vector3 lastObjEnd = (PathSegments.Count > 0 ? PathSegments[0].GetPositionAt(0f) : Vector3.zero);
				foreach (var obj in PathSegments)
				{
					length += (obj.GetPositionAt(0f) - lastObjEnd).magnitude;
					if (obj is BezierCurve curve)
						curve.RecalculateLength();
					length += obj.Length;
					lastObjEnd = obj.GetPositionAt(1f);
				}
				return length;
			}

			// Calculates the sum of all Segment Lengths (excludes space between)
			public float CalculateExclusiveLength()
			{
				float length = 0f;
				foreach (var obj in PathSegments)
				{
					if (obj is BezierCurve curve)
						curve.RecalculateLength();
					length += obj.Length;
				}
				return length;
			}

			public override void RecalculateLength()
			{
				m_InclusiveLength = CalculateInclusiveLength();
				m_Length = CalculateExclusiveLength();
			}

			public int GetPointCount()
			{
				int count = 0;
				foreach (var segment in PathSegments)
				{
					if (segment is BezierCurve curve)
						count += curve.Points.Count;
					else if (segment is Line)
						count += 2;
				}
				return count;
			}

			// World Space
			public Vector3 GetPoint(int index)
			{
				int count = 0;
				Vector3 point = Vector3.zero;
				foreach (var segment in PathSegments)
				{
					if (segment is BezierCurve curve)
					{
						if (index >= count && index < count + curve.Points.Count)
						{
							int segment_index = index - count;
							point = curve.transform.TransformPoint(curve.Points[segment_index]);
							break;
						}
						count += curve.Points.Count;
					}
					else if (segment is Line line)
					{
						if (index >= count && index < count + 2)
						{
							if (index == count)
								point = line.transform.TransformPoint(line.A);
							else
								point = line.transform.TransformPoint(line.B);
							break;
						}
						count += 2;
					}
				}
				return point;
			}

			// World Space
			public void SetPoint(int index, Vector3 vec)
			{
				int count = 0;
				foreach (var segment in PathSegments)
				{
					if (segment is BezierCurve curve)
					{
						if (index >= count && index < count + curve.Points.Count)
						{
							int segment_index = index - count;
							curve.Points[segment_index] = curve.transform.InverseTransformPoint(vec);
							break;
						}
						count += curve.Points.Count;
					}
					else if (segment is Line line)
					{
						if (index >= count && index < count + 2)
						{
							if (index == count)
								line.A = line.transform.InverseTransformPoint(vec);
							else
								line.B = line.transform.InverseTransformPoint(vec);
							break;
						}
						count += 2;
					}
				}
			}

			public IPathObject GetAffectedObjectAt(int index)
			{
				int count = 0;
				foreach (var segment in PathSegments)
				{
					if (segment is BezierCurve curve)
					{
						if (index >= count && index < count + curve.Points.Count)
						{
							return segment;
						}
						count += curve.Points.Count;
					}
					else if (segment is Line line)
					{
						if (index >= count && index < count + 2)
						{
							return segment;
						}
						count += 2;
					}
				}
				return null;
			}
		}

#if UNITY_EDITOR
		[CustomEditor(typeof(Path))]
		[CanEditMultipleObjects]
		public class PathEditor : Editor
		{
			SerializedProperty PathObjs;
			SerializedProperty LengthObj;
			SerializedProperty InclusiveLengthObj;

			// Contains the first index of any pair that overlap
			Dictionary<int, int> m_DoubleTransformPoint = new Dictionary<int, int>();

			private void OnEnable()
			{
				PathObjs = serializedObject.FindProperty("m_PathObjects");
				LengthObj = serializedObject.FindProperty("m_Length");
				InclusiveLengthObj = serializedObject.FindProperty("m_InclusiveLength");
			}

			public override void OnInspectorGUI()
			{
				base.DrawDefaultInspector();
				Path p = target as Path;
				//serializedObject.Update();
				//EditorGUILayout.PropertyField(PathObjs, true);
				//EditorGUILayout.PropertyField(LengthObj);
				//EditorGUILayout.PropertyField(InclusiveLengthObj);

				if (GUILayout.Button("Add Bezier"))
				{
					var new_obj = new GameObject("Bezier", typeof(BezierCurve));
					new_obj.transform.parent = p.transform;
					new_obj.transform.localPosition = Vector3.zero;
					Vector3 end_point = p.GetPositionAt(1f);
					//Debug.Log($"End Point World: {end_point}");
					end_point = new_obj.transform.InverseTransformPoint(end_point);
					//Debug.Log($"End Point Local: {end_point}");
					List<Vector3> points = new List<Vector3>(new Vector3[3]);
					points[0] = end_point;
					points[1] = points[0] + new_obj.transform.forward;
					points[2] = points[1] + new_obj.transform.forward;
					new_obj.GetComponent<BezierCurve>().LineSteps = 15;
					new_obj.GetComponent<BezierCurve>().Points = points;
					Undo.RecordObject(p, "Add Bezier");
					p.AddObject(new_obj.GetComponent<BezierCurve>());
					Undo.RegisterCreatedObjectUndo(new_obj, "Create Bezier");
				}
				if (GUILayout.Button("Add Line"))
				{
					var new_obj = new GameObject("Line", typeof(Line));
					new_obj.transform.parent = p.transform;
					new_obj.transform.localPosition = Vector3.zero;
					Vector3 end_point = p.GetPositionAt(1f);
					//Debug.Log($"End Point World: {end_point}");
					end_point = new_obj.transform.InverseTransformPoint(end_point);
					//Debug.Log($"End Point Local: {end_point}");
					new_obj.GetComponent<Line>().A = end_point;
					new_obj.GetComponent<Line>().B = end_point + new_obj.transform.forward;
					Undo.RecordObject(p, "Add Line");
					p.AddObject(new_obj.GetComponent<Line>());
					Undo.RegisterCreatedObjectUndo(new_obj, "Create Line");
				}
				if (GUILayout.Button("Recalculate Length"))
				{
					p.RecalculateLength();
				}
			}

			public void OnSceneGUI()
			{
				Path p = target as Path;
				if (p.PathSegments.Count < 1)
					return;

				m_DoubleTransformPoint = ScanForDouble(p);

				// Draw Transform Points
				int points = p.GetPointCount();
				Vector3[] transformed_points = new Vector3[points];
				for (int i = 0; i < points; ++i)
				{
					if (m_DoubleTransformPoint.ContainsKey(i))
					{
						var new_trans = ShowMultiPoint2(p, i, m_DoubleTransformPoint[i]);
						transformed_points[i] = new_trans;
						for (int j = 1; j <= m_DoubleTransformPoint[i]; ++j)
						{
							transformed_points[i + j] = new_trans;
						}
						i += m_DoubleTransformPoint[i];
					}
					else
					{
						transformed_points[i] = ShowPoint(p, i);
					}
				}

				int point_index_offset = 0;
				foreach (var segment in p.PathSegments)
				{
					if (segment is BezierCurve curve)
					{
						// Draw Lines between Points
						Handles.color = Color.gray;

						Vector3 lastPoint = transformed_points[point_index_offset];
						for (int i = 1; i < curve.Points.Count; ++i)
						{
							Handles.DrawLine(lastPoint, transformed_points[point_index_offset + i]);
							lastPoint = transformed_points[point_index_offset + i];
						}


						// Draw The Bezier and Tangents
						Handles.color = Color.white;
						lastPoint = curve.GetPositionAt(0f);
						int steps = curve.LineSteps;
						for (int i = 1; i <= steps; ++i)
						{
							Vector3 nextPoint = curve.GetPositionAt(i / (float)steps);
							// Draw Bezier Segment
							Handles.color = Color.white;
							Handles.DrawLine(lastPoint, nextPoint);

							// Draw Normal
							Vector3 gradient = curve.GetTangentAt(i / (float)steps).normalized;

							Handles.color = Color.Lerp(Color.magenta, Color.yellow, i / (float)steps);
							Handles.DrawLine(nextPoint, nextPoint - gradient);

							lastPoint = nextPoint;
						}

						point_index_offset += curve.Points.Count;
					}
					else if (segment is Line line)
					{
						Handles.color = Color.white;

						Handles.DrawLine(transformed_points[point_index_offset], transformed_points[point_index_offset + 1]);

						point_index_offset += 2;
					}
				}
			}

			private Dictionary<int, int> ScanForDouble(Path p)
			{
				Dictionary<int, int> doubles = new Dictionary<int, int>();

				for (int i = 0; i < p.GetPointCount(); ++i)
				{
					var first_segment = p.GetAffectedObjectAt(i);
					Vector3 point = first_segment.transform.TransformPoint(p.GetPoint(i));
					int j = 1;
					for (; i + j < p.GetPointCount();)
					{
						var other_segment = p.GetAffectedObjectAt(i + j);
						if (RoughlyTheSame(point, other_segment.transform.TransformPoint(p.GetPoint(i + j))))
							++j;
						else
							break;
					}
					if (j > 1)
					{
						doubles[i] = j - 1;
						i += j - 1;
					}
				}

				return doubles;
			}

			private bool RoughlyTheSame(Vector3 a, Vector3 b)
			{
				const float leniance = 0.015f;
				return a.x < b.x + leniance && a.x > b.x - leniance && a.y < b.y + leniance && a.y > b.y - leniance && a.z < b.z + leniance && a.z > b.z - leniance;
			}

			private bool DifferentEnough(Vector3 a, Vector3 b)
			{
				const float diff = 0.015f;
				return a.x < (b.x - diff) || a.x > (b.x + diff) || a.y < (b.y - diff) || a.y > (b.y + diff) || a.z < (b.z - diff) || a.z > (b.z + diff);
			}

			private Vector3 ShowPoint(Path path, int index)
			{
				var affected = path.GetAffectedObjectAt(index);
				Vector3 point = path.GetPoint(index);

				EditorGUI.BeginChangeCheck();
				point = Handles.DoPositionHandle(point, path.transform.rotation);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(affected, "Move Point");
					EditorUtility.SetDirty(affected);
					path.SetPoint(index, point);
					path.RecalculateLength();
				}
				return point;
			}

			private Vector3 ShowMultiPoint(Path path, int index, int count)
			{
				var affected = path.GetAffectedObjectAt(index);
				Vector3 point = path.GetPoint(index);

				EditorGUI.BeginChangeCheck();
				point = Handles.DoPositionHandle(point, path.transform.rotation);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(affected, "Move Point");
					EditorUtility.SetDirty(affected);
					path.SetPoint(index, point);
					for (int i = 1; i <= count; ++i)
					{
						var also_affected = path.GetAffectedObjectAt(index + i);
						if (also_affected != affected)
						{
							Undo.RecordObject(also_affected, "Move Point");
							EditorUtility.SetDirty(also_affected);
							affected = also_affected;
						}
						path.SetPoint(index + i, point);
					}
				}
				for (int i = 1; i <= count; ++i)
				{
					var also_affected = path.GetAffectedObjectAt(index + i);
					EditorGUI.BeginChangeCheck();
					Handles.DoPositionHandle(point, also_affected.transform.rotation);
					EditorGUI.EndChangeCheck();
				}
				return point;
			}
			private Vector3 ShowMultiPoint2(Path path, int index, int count)
			{
				Vector3 point = path.GetPoint(index);


				for (int i = 0; i <= count; ++i)
				{
					var affected = path.GetAffectedObjectAt(index + i);
					EditorGUI.BeginChangeCheck();
					point = Handles.DoPositionHandle(point, affected.transform.rotation);
					if (EditorGUI.EndChangeCheck())
					{
						for (int j = 0; j <= count; ++j)
						{
							var also_affected = path.GetAffectedObjectAt(index + j);
							if (j != i)
							{
								Undo.RecordObject(also_affected, "Move Point");
								EditorUtility.SetDirty(also_affected);
							}
							path.SetPoint(index + j, point);
						}
						path.RecalculateLength();
					}
				}
				return point;
			}

		}
#endif  
	}
}