using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Common
{

	public class SelectionMover : EditorWindow
	{
		private Vector3 _move;

		private Vector3 _rotation;

		private int _rotationItem = -1;

		private bool rotationItemIsValid { get => _rotationItem > -1 && _rotationItem < Selection.transforms.Length; }

		private Vector3 _rotationCenter;

		private Vector3 _flip;
		private bool _flipX, _flipY, _flipZ;

		[MenuItem("Window/Common/Selection Mover")]
		private static void Init()
		{
			EditorWindow.GetWindow(typeof(SelectionMover));
		}

		private void OnGUI()
		{
			GUILayout.Label("Selection Mover");

			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Label("X Delta: ", GUILayout.MaxWidth(60));
				_move.x = EditorGUILayout.FloatField(_move.x, GUILayout.MaxWidth(100));
				GUILayout.Label("Y Delta: ", GUILayout.MaxWidth(60));
				_move.y = EditorGUILayout.FloatField(_move.y, GUILayout.MaxWidth(100));
				GUILayout.Label("Z Delta: ", GUILayout.MaxWidth(60));
				_move.z = EditorGUILayout.FloatField(_move.z, GUILayout.MaxWidth(100));
				if (GUILayout.Button("Move Selection"))
					DoMove();
			}
			using (new GUILayout.HorizontalScope())
			{
				_rotation = EditorGUILayout.Vector3Field("Rotation: ", _rotation);
			}
			using (new GUILayout.HorizontalScope())
			{
				string itemString;
				if (rotationItemIsValid)
					itemString = $"item '{Selection.transforms[_rotationItem].gameObject.name}'";
				else
					itemString = "given center";
				GUILayout.Label("Will rotate around " + itemString);
			}
			using (new GUILayout.HorizontalScope())
			{
				_rotationItem = EditorGUILayout.IntField("Rotation item: ", _rotationItem);
			}
			using (new GUILayout.HorizontalScope())
			{
				_rotationCenter = EditorGUILayout.Vector3Field("Rotation Center: ", _rotationCenter);
			}
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Rotate"))
				{
					DoRotate();
				}
			}
			using (new GUILayout.HorizontalScope())
			{
				_flip = EditorGUILayout.Vector3Field("Flip Center", _flip);
			}
			using (new GUILayout.HorizontalScope())
			{
				_flipX = GUILayout.Toggle(_flipX, "Flip X:");
				_flipY = GUILayout.Toggle(_flipY, "Flip Y:");
				_flipZ = GUILayout.Toggle(_flipZ, "Flip Z:");
				if (GUILayout.Button("Flip"))
					DoFlip();
			}
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Label($"Currently Selected '{Selection.transforms.Length}' transforms");
			}
		}

		void DoMove()
		{
			Undo.RecordObjects(Selection.transforms, "Move Selected Objects");
			for (int i = 0; i < Selection.transforms.Length; ++i)
			{
				var copy = Selection.transforms[i].position;
				copy += _move;
				Selection.transforms[i].position = copy;
				EditorUtility.SetDirty(Selection.transforms[i]);
			}
		}

		void DoRotate()
		{
			Vector3 rotationCenter;
			if (rotationItemIsValid)
			{
				rotationCenter = Selection.transforms[_rotationItem].position;
			}
			else
				rotationCenter = _rotationCenter;

			Undo.RecordObjects(Selection.transforms, "Rotate Selected Objects");
			for (int i = 0; i < Selection.transforms.Length; ++i)
			{
				var item = Selection.transforms[i];

				item.RotateAround(rotationCenter, Vector3.right, _rotation.x);
				item.RotateAround(rotationCenter, Vector3.up, _rotation.y);
				item.RotateAround(rotationCenter, Vector3.forward, _rotation.z);
			}
		}

		void DoFlip()
		{
			if (!_flipX && !_flipY && !_flipZ)
				return;
			Undo.RecordObjects(Selection.transforms, "Flip Selected Objects");
			for (int i = 0; i < Selection.transforms.Length; ++i)
			{
				var copy = Selection.transforms[i].position;
				copy -= _flip;
				copy.x *= (_flipX ? -1 : 1);
				copy.y *= (_flipY ? -1 : 1);
				copy.z *= (_flipZ ? -1 : 1);
				copy += _flip;
				Selection.transforms[i].position = copy;
				copy = Selection.transforms[i].rotation.eulerAngles;
				copy *= -1;
				Selection.transforms[i].rotation = Quaternion.Euler(copy);
				EditorUtility.SetDirty(Selection.transforms[i]);
			}
		}
	}

}