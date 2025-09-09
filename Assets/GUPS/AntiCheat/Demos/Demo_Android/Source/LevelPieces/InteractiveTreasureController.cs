// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Android
{	
	public class InteractiveTreasureController : LevelPiece 
	{
		public delegate void StartInteractionDelegate();
		public static event StartInteractionDelegate StartInteractionEvent;
		
		private void OnTriggerEnter2D(Collider2D col) {
			if(StartInteractionEvent != null) {
				StartInteractionEvent();
			}
			Destroy (gameObject);
		}
	}
}