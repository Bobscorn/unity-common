using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Common
{
	public class GameObjectReplacer : EditorWindow
	{
		[MenuItem("Window/Common/GameObject Replacer")]
		private static void Init()
		{
			GetWindow<GameObjectReplacer>();
		}



		private GameObject _replacePrefab;

		private Vector3 _positionOffset;
		private Vector3 _rotationOffset;

		private bool _replacePositions;
		private bool _replaceRotations;

		private bool _allowNonScene = false;
		private bool _doRename = false;

		private void OnGUI()
		{
			using (new GUILayout.HorizontalScope())
			{
				GUILayout.Label("Game Object Replacer");
			}

			using (new GUILayout.HorizontalScope())
			{
				_replacePrefab = EditorGUILayout.ObjectField(_replacePrefab, typeof(GameObject), false) as GameObject;
			}

			using (new GUILayout.HorizontalScope())
			{
				_replacePositions = GUILayout.Toggle(_replacePositions, "Replace Positions");
				_replaceRotations = GUILayout.Toggle(_replaceRotations, "Replace Rotations");
			}

			using (new GUILayout.HorizontalScope())
			{
				_positionOffset = EditorGUILayout.Vector3Field("Position Offset", _positionOffset);
				_rotationOffset = EditorGUILayout.Vector3Field("Rotation Offset", _rotationOffset);
			}

			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Replace"))
				{
					if (_allowNonScene)
						ReplaceObjects(Selection.gameObjects);
					else
						ReplaceObjects(Selection.transforms);
				}
			}

			using (new GUILayout.HorizontalScope())
			{
				_allowNonScene = GUILayout.Toggle(_allowNonScene, "Allow Non-Scene Objects");
				_doRename = GUILayout.Toggle(_doRename, "Rename Pieces");
			}
		}

		void ReplaceObjects(GameObject[] gameObjects)
		{
			for (int i = 0; i < gameObjects.Length; ++i)
				ReplaceObject(gameObjects[i].transform);
		}

		void ReplaceObjects(Transform[] transforms)
		{
			for (int i = 0; i < transforms.Length; ++i)
				ReplaceObject(transforms[i]);
		}

		private void ReplaceObject(Transform transform)
		{
			var replacedObj = PrefabUtility.InstantiatePrefab(_replacePrefab, transform.parent) as GameObject;

			if (_doRename)
				replacedObj.name = transform.name;

			var replacedTrans = replacedObj.transform;

			if (_replacePositions)
				replacedTrans.localPosition = _positionOffset + transform.localPosition;

			if (_replaceRotations)
				replacedTrans.localRotation = Quaternion.Euler(_rotationOffset + transform.localRotation.eulerAngles);
			
			Undo.RegisterCreatedObjectUndo(replacedObj, "Replace Objects");
			Undo.DestroyObjectImmediate(transform.gameObject);
		}
	} 
}
