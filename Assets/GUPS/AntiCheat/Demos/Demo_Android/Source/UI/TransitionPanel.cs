// Unity
using UnityEngine;
using UnityEngine.UI;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
	public class TransitionPanel : MonoBehaviour 
	{
		public GameObject IntroContainer;
		public GameObject GameOverContainer;
		public Text LevelValue;
		public Text LevelNameValue;
		public Text LivesValue;

		public void DisplayIntro(bool enabled) {
			IntroContainer.SetActive (enabled);
		}

		public void DisplayGameOver(bool enabled) {
			GameOverContainer.SetActive (enabled);
		}

		public void SetIntro(int levelId, string levelName, int lives) {
			LevelValue.text = "Level " + levelId;
			LevelNameValue.text = "\"" + levelName + "\"";
			LivesValue.text = lives.ToString();
		}
	}
}
