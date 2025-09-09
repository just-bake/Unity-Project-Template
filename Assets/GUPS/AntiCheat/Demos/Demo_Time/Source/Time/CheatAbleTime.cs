// GUPS - AntiCheat
using GUPS.AntiCheat.Detector;
using GUPS.AntiCheat.Protected.Time;

namespace GUPS.AntiCheat.Demo.Demo_Time
{
    /// <summary>
    /// This class is used to demonstrate how common cheating / hacking tools modify the game speed, without accessing the operating system clock, how cheat tools commonly do it.
    /// To prevent the game from being manipulated use the GUPS.AntiCheat.Speed.ProtectedTime class instead of the UnityEngine.Time class in your code.
    /// </summary>
    public static class CheatAbleTime
    {
        /// <summary>
        /// The detector for the game time cheating. You will not need to access it, just use the ProtectedTime class.
        /// </summary>
        private static GameTimeCheatingDetector gameTimeCheatingDetector;

        /// <summary>
        /// This is a simulated deltaTime to showcase how cheating would look like.
        /// </summary>
        public static float deltaTime
        {
            get
            {
                // If there is no GameTimeCheatingDetector yet, find it in the scene.
                if (gameTimeCheatingDetector == null)
                {
                    gameTimeCheatingDetector = AntiCheatMonitor.Instance.GetDetector<GameTimeCheatingDetector>();
                }

                // If there is no GameTimeCheatingDetector in the scene, return the normal deltaTime.
                if (gameTimeCheatingDetector == null)
                {
                    return UnityEngine.Time.deltaTime;
                }

                // If cheating is detected, return the protected deltaTime.
                if(gameTimeCheatingDetector.PossibleCheatingDetected)
                {
                    return ProtectedTime.deltaTime;
                }

                return UnityEngine.Time.deltaTime * CheatFactor;
            }
        }

        /// <summary>
        /// The cheating factor to simulate the cheating.
        /// </summary>
        public static float CheatFactor { get; set; } = 1.0f;
    }
}
