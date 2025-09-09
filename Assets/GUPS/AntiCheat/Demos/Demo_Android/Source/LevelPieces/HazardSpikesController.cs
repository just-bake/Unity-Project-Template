// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
	public class HazardSpikesController : LevelPiece 
	{
		private void OnCollisionEnter2D(Collision2D coll) 
		{
			if (coll.gameObject.name == "Player") 
			{
				PlayerController player = coll.gameObject.GetComponent<PlayerController>();
				player.StartPlayerDeath();
			}
		}
	}
}
