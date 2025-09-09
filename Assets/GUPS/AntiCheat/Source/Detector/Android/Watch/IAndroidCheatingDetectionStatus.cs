// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Detector;

namespace GUPS.AntiCheat.Detector.Android
{
    /// <summary>
    /// Represents an extension of the <see cref="IDetectorStatus"/> interface, providing next to a possibility of false positive and threat rating,
    /// information about the type of cheating detected on the Android device.
    /// </summary>
    public interface IAndroidCheatingDetectionStatus : IDetectorStatus
    {
        /// <summary>
        /// The type of cheating detected on the Android device.
        /// </summary>
        EAndroidCheatingType AndroidCheatingType { get; }
    }
}
