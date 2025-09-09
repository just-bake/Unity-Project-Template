// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo.Demo_Time
{
    /// <summary>
    /// A simple ui component that displays the current value of a slider.
    /// </summary>
    public class UiSliderText : MonoBehaviour
    {
        /// <summary>
        /// The text of the slider.
        /// </summary>
        public UnityEngine.UI.Text SliderText;

        /// <summary>
        /// Update the slider text.
        /// </summary>
        /// <param name="value">The new value of the slider.</param>
        public void OnSliderUpdate(float value)
        {
            this.SliderText.text = value.ToString("0.00");
        }
    }
}