// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
	public partial class Level : MonoBehaviour 
	{
		[SerializeField]
		public int _totalTime = 60;
		[SerializeField]
		private float _gravity = -30;
		[SerializeField]
		private Sprite _background;

		public int TotalTime 
		{
			get { return _totalTime; }
			set { _totalTime = value; }
		}

		public float Gravity 
		{
			get { return _gravity; }
			set { _gravity = value; }
		}

		public Sprite Background 
		{
			get { return _background; }
			set { _background = value; }
		}
	}
}