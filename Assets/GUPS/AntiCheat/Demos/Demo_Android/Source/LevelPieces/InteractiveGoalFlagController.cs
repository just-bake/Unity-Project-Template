// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Android
{	
	public class InteractiveGoalFlagController : LevelPiece 
	{
		public delegate void StartInteractionDelegate();
		public static event StartInteractionDelegate StartInteractionEvent;
        public static bool IsInteractAble = true;

        private void OnTriggerEnter2D(Collider2D col) {
			if(StartInteractionEvent != null && col.gameObject.name == "Player" && IsInteractAble) {
				StartInteractionEvent();
			}
		}
	}
}
