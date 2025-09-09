// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
	public class VirtualInputPanel : MonoBehaviour 
	{
		public void LeftOnPress() {
			InputWrapper.Instance.VirtualLeft = true;
		}
		
		public void LeftOnRelease() {
			InputWrapper.Instance.VirtualLeft = false;
		}
		
		public void RightOnPress() {
			InputWrapper.Instance.VirtualRight = true;
		}
		
		public void RightOnRelease() {
			InputWrapper.Instance.VirtualRight = false;
		}
		
		public void UpOnPress() {
			InputWrapper.Instance.VirtualUp =  true;
		}

		public void UpOnRelease() {
			InputWrapper.Instance.VirtualUp =  false;
		}
	}
}