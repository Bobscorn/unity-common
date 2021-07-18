using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Common
{
	namespace Behaviour
	{
#if UNITY_EDITOR
		using UnityEditor;
		[CustomEditor(typeof(Touchable))]
		public class TouchableEditor : Editor
		{
			public override void OnInspectorGUI() { }
		}
#endif


		public class Touchable : Text
		{
			protected override void Awake()
			{
				base.Awake();
			}
		}  
	}
}