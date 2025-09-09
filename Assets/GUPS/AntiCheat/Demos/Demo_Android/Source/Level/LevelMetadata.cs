// System
using System;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
    [Serializable]
    public class LevelMetadata
    {
        [SerializeField]
        private string sceneName;

        [SerializeField]
        private string levelName;

        public string LevelName
        {
            get { return (levelName == null || levelName == "") ? "Untitled" : levelName; }
            set { levelName = value; }
        }

        public string SceneName
        {
            get
            {
                Debug.Log("Scene Name : " + this.sceneName);
                return this.sceneName;
            }
        }
    }
}