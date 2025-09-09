// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
	public class InteractiveCoinController : LevelPiece 
	{
		public delegate void StartInteractionDelegate();
		public static event StartInteractionDelegate StartInteractionEvent;
		public static bool IsInteractAble = true;

		private void OnTriggerEnter2D(Collider2D col) {
			if(StartInteractionEvent != null && IsInteractAble) {
				StartInteractionEvent();
			}
			Destroy (gameObject);
		}
	}
}
