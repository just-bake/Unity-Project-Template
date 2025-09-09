// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Demo
{
    /// <summary>
    /// A simple ui component that displays the current threat level of the anti cheat monitor.
    /// </summary>
    public class UiThreatLevel : MonoBehaviour
    {
        /// <summary>
        /// The anti cheat monitor.
        /// </summary>
        public AntiCheatMonitor antiCheatMonitor;

        /// <summary>
        /// The debug text for the threat level.
        /// </summary>
        public UnityEngine.UI.Text threatLevelText;

        /// <summary>
        /// The debug text for the sensitivity level.
        /// </summary>
        public UnityEngine.UI.Text sensitivityLevelText;

        /// <summary>
        /// Update the threat level text.
        /// </summary>
        private void Update()
        {
            // Update the threat level text.
            if (this.antiCheatMonitor != null)
            {
                this.threatLevelText.text = this.antiCheatMonitor.ThreatLevel.ToString("0.00");

                this.sensitivityLevelText.text = this.antiCheatMonitor.SensitiveLevel.ToString();
            }
        }
    }
}