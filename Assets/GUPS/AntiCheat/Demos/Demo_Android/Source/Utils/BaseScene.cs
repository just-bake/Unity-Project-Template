// System
using System;

// Unity
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
	public class BaseScene : MonoBehaviour 
	{
		public enum Scene 
		{
			Title,
			LevelSelect,
			LevelHandler
		}

		public String TitleScene = "Assets/GUPS/AntiCheat/Demos/Demo_Android/Scenes/Title/Title.unity";

        public String LevelSelectScene = "Assets/GUPS/AntiCheat/Demos/Demo_Android/Scenes/LevelSelect/LevelSelect.unity";

        public String LevelHandlerScene = "Assets/GUPS/AntiCheat/Demos/Demo_Android/Scenes/LevelHandler/LevelHandler.unity";

        protected virtual void Awake() 
		{
			Session.Instantiate();
		}

		protected void GoToScene(Scene scene) 
		{
			String scenePath = null;

			switch(scene)
			{
                case Scene.Title:
                    scenePath = TitleScene;
                    break;
                case Scene.LevelSelect:
                    scenePath = LevelSelectScene;
                    break;
                case Scene.LevelHandler:
                    scenePath = LevelHandlerScene;
                    break;
            }

#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
#else
            SceneManager.LoadScene(System.IO.Path.GetFileNameWithoutExtension(scenePath));
#endif
        }
    }
}