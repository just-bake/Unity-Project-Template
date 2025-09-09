// Unity
using UnityEngine;

// GUPS - AntiCheat
using GUPS.AntiCheat.Detector;
using GUPS.AntiCheat.Monitor.Time;

namespace GUPS.AntiCheat.Demo.Demo_Time
{
    /// <summary>
    /// Like the CheatAbleTime class, this class is used to demonstrate how common cheating / hacking tools modify the game speed. 
    /// Without accessing the operating system clock, how cheat tools commonly do it. To prevent the game from being manipulated use 
    /// the GUPS.AntiCheat.Speed.ProtectedTime class instead of the UnityEngine.Time class in your code.
    /// </summary>
    public class UiCheatSlider : MonoBehaviour
    {
        /// <summary>
        /// The detector for the game time cheating. You will not need to access it, just use the ProtectedTime class.
        /// </summary>
        private GameTimeCheatingDetector gameTimeCheatingDetector;

        /// <summary>
        /// The detected time deviation.
        /// </summary>
        private ETimeDeviation timeDeviation;

        /// <summary>
        /// Find the GameTimeCheatingDetector in the scene.
        /// </summary>
        private void Start()
        {
            // Get the detector from the AntiCheatMonitor.
            this.gameTimeCheatingDetector = AntiCheatMonitor.Instance.GetDetector<GameTimeCheatingDetector>();
        }

        /// <summary>
        /// When the slider value changes, update the cheat factor and notify the cheating detection.
        /// This is just for demonstration purposes. In a real game, the cheating detection will happen automatically in the background through the GameTimeMonitor.
        /// To protect you game time from cheating, use the GUPS.AntiCheat.Speed.ProtectedTime class instead of the UnityEngine.Time class.
        /// </summary>
        /// <param name="value">The new value.</param>
        public void OnSliderUpdate(float value)
        {
            // Apply the cheat factor to the CheatAbleTime.
            CheatAbleTime.CheatFactor = value;

            // Detect which time deviation is happening including some tolerance...
            if(value > 0.95f && value < 1.05f)
            {
                this.timeDeviation = ETimeDeviation.None;
                return;
            }

            // ... outside the tolerance, it's cheating.
            this.timeDeviation = value < 1 ? ETimeDeviation.SlowedDown : ETimeDeviation.SpeedUp;
        }

        /// <summary>
        /// For demonstration purposes, update the cheating detection and send the possible cheating event to the detector.
        /// </summary>
        private void Update()
        {
            // Send possible cheating detection event.
            // Note: Outside the demo, the cheating detection will happen automatically in the background through the GameTimeMonitor. This is just for demonstration purposes.
            if (this.gameTimeCheatingDetector.PossibleCheatingDetected)
            {
                // Do nothing, the cheating is already detected. And the protected time is already used.
            }
            else
            {
                // Nofify the detector about the possible cheating.
                this.gameTimeCheatingDetector.OnNext(new GameTimeStatus(this.timeDeviation));
            }
        }
    }
}