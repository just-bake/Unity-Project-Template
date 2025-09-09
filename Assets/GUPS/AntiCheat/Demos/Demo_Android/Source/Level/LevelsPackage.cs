// System
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
	public class LevelsPackage : ScriptableObject 
	{
		public const string Suffix = "_level";
		public const string ResourcePath = "Levels/LevelsPackage";
		public List<LevelMetadata> metadataList;
		public bool hasChanges = false;
	}	
}
