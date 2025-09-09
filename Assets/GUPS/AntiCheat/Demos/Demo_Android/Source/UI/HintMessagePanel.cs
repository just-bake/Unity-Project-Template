// Unity
using UnityEngine;
using UnityEngine.UI;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
	public class HintMessagePanel : MonoBehaviour 
	{
		public GameObject Container;
		public Text MessageValue;

		private void Awake() {
			Container.SetActive(false);
		}

		public void SetMessage(string message) {
			MessageValue.text = message;
		}

		public void Show() {
			Container.SetActive(true);
		}

		public void Hide() {
			Container.SetActive(false);
		}
	}
}
