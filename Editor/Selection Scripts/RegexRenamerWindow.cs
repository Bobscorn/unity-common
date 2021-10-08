using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace Common
{
	public class RegexRenamerWindow : EditorWindow
	{
		private string _findString;
		private string _replaceString;
		private bool _allowNonSceneObjects;

		[MenuItem("Window/Common/RegexRenamer")]
		static void Init()
		{
			GetWindow(typeof(RegexRenamerWindow));
		}

		private void OnGUI()
		{
			GUILayout.Label("Regex Renamer Window");

			GUILayout.BeginHorizontal();

			_findString = EditorGUILayout.TextField("Find: ", _findString);

			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();

			_replaceString = EditorGUILayout.TextField("Replace with: ", _replaceString);

			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();

			_allowNonSceneObjects = EditorGUILayout.Toggle("Prefabs:", _allowNonSceneObjects);

			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Replace"))
			{
				DoReplace();
			}

			GUILayout.EndHorizontal();
		}

		void DoReplace()
		{
			Undo.RecordObjects(Selection.gameObjects, "Rename Objects");
			for (int i = 0; i < Selection.gameObjects.Length; ++i)
			{
				var obj = Selection.gameObjects[i];
				if (_allowNonSceneObjects || obj.scene.IsValid())
				{
					obj.name = Regex.Replace(obj.name, _findString, _replaceString);
					EditorUtility.SetDirty(obj);
				}
			}
		}
	} 
}
