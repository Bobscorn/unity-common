using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Common
{
	namespace Behaviour
	{
		public class CameraTextureSaver : MonoBehaviour
		{

			public Camera Camera;

			public string fileName;

			public int ResolutionScale;

			public KeyCode RegularCode;
			public KeyCode CaptureCode;

			private static int _counter = 0;

			private void Update()
			{
				if (Input.GetKeyDown(RegularCode))
				{
					SavePNG();
				}
				if (Input.GetKeyDown(CaptureCode))
				{
					SavePNGCapture();
				}
			}

			public void SavePNG()
			{
				RenderTexture currentRT = RenderTexture.active;
				RenderTexture.active = Camera.targetTexture;


				Camera.Render();

				Texture2D img = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height);
				img.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
				img.Apply();
				RenderTexture.active = currentRT;

				var bytes = img.EncodeToPNG();

				Destroy(img);

				File.WriteAllBytes(System.IO.Path.Combine(Application.persistentDataPath, "Backgrounds", fileName + $" {++_counter:00}.png"), bytes);
			}

			public void SavePNGCapture()
			{
				ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(Application.persistentDataPath, "Backgrounds", fileName + $" {++_counter:00}.png"), ResolutionScale);
			}
		}

#if UNITY_EDITOR
		[CustomEditor(typeof(CameraTextureSaver))]
		public class CameraTextureSaverEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				if (GUILayout.Button("Save Image"))
				{
					var pp = target as CameraTextureSaver;

					pp.SavePNG();
				}

				if (GUILayout.Button("'Capture' Image"))
				{
					var pp = target as CameraTextureSaver;

					pp.SavePNGCapture();
				}
			}
		}
#endif  
	}
}