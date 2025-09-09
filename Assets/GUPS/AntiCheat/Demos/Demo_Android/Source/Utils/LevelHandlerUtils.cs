// System
using System;
using System.Collections;

// Unity
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
	public class LevelHandlerUtils : MonoBehaviour 
	{
		private static Level _level;

		public static String LevelRootPath = "Assets/GUPS/AntiCheat/Demos/Demo_Android/Scenes/Levels/";

		public static IEnumerator LoadLevel(string sceneName) 
		{
			String levelPath = LevelRootPath + sceneName + ".unity";

#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(levelPath, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.None));
#else
            SceneManager.LoadScene(System.IO.Path.GetFileNameWithoutExtension(levelPath), LoadSceneMode.Additive);
#endif
            yield return 0;

#if UNITY_2023_1_OR_NEWER
            _level = FindFirstObjectByType<Level>(FindObjectsInactive.Include);
#else
            _level = GameObject.FindObjectOfType<Level>();
#endif
        }

        public static void DestroyLevel() {
			if(_level != null) {
				Destroy(_level.gameObject);
			}
		}
	}
}
