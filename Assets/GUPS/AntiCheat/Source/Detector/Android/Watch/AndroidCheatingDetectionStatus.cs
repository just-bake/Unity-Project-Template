namespace GUPS.AntiCheat.Detector.Android
{
    /// <summary>
    /// Represents a default implementation of the <see cref="IAndroidCheatingDetectionStatus"/> interface, providing next to a possibility of false 
    /// positive and threat rating, information about the type of cheating detected on the Android device.
    /// </summary>
    public struct AndroidCheatingDetectionStatus : IAndroidCheatingDetectionStatus
    {
        /// <summary>
         /// Gets a value indicating the possibility of a false positive when assessing threats for the implementing subject from 0.0 to 1.0.
         /// </summary>
         /// <remarks>
         /// The value is represented as a positive float value ranging from 0.0 to 1.0, where 0.0 indicates no possibility of a false positive,
         /// and 1.0 denotes a 100% possibility of a false positive.
         /// </remarks>
        public float PossibilityOfFalsePositive { get; private set; }

        /// <summary>
        /// Gets the threat rating associated with the implementing class, indicating the assessed level of potential threat.
        /// </summary>
        /// <remarks>
        /// The threat rating is represented as a positive 32-bit integer (UInt32), where higher values denote greater perceived threats.
        /// </remarks>
        public uint ThreatRating { get; private set; }

        /// <summary>
        /// The type of cheating detected on the Android device.
        /// </summary>
        /// <remarks>
        /// Represents the type of cheating detected on the Android device, as an instance of the <see cref="EAndroidCheatingType"/> enumeration.
        /// If no type of cheating could be classified, the value is set to <see cref="EAndroidCheatingType.UNKNOWN"/>.
        /// </remarks>
        public EAndroidCheatingType AndroidCheatingType { get; private set; }

        /// <summary>
        /// Gets if the monitor notifing the detector failed to retrieve its data or an exception occurred while retrieving the data over the native interface.
        /// So no valid value was returned and no validation could be performed by the detector.
        /// </summary>
        /// <remarks>
        /// If the monitor notifing the detector failed to retrieve its data or an exception occurred while retrieving the data over the native interface this 
        /// property will be set to false. Meaning no valid value was returned and no validation could be performed by the detector. So it is not certain if this 
        /// was caused by cheating or not. Mostly the cause is that parts of the native implementation are not available on the current Android device with the 
        /// build sdk version (requires at least sdk v19).
        /// </remarks>
        public bool MonitorFailedToRetrieveData { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="CheatingDetectionStatus"/> struct with the specified possibility of false positive and threat rating.
        /// </summary>
        /// <param name="_PossibilityOfFalsePositive">The possibility of a false positive ranging from 0.0 to 1.0.</param>
        /// <param name="_ThreatRating">The threat rating.</param>
        /// <param name="_AndroidCheatingType">The type of cheating detected on the Android device.</param>
        /// <param name="_MonitorFailedToRetrieveData">The monitor notifing the detector failed to retrieve its data or an exception occurred.</param>
        public AndroidCheatingDetectionStatus(float _PossibilityOfFalsePositive, uint _ThreatRating, EAndroidCheatingType _AndroidCheatingType, bool _MonitorFailedToRetrieveData)
        {
            this.PossibilityOfFalsePositive = _PossibilityOfFalsePositive;
            this.ThreatRating = _ThreatRating;
            this.AndroidCheatingType = _AndroidCheatingType;
            this.MonitorFailedToRetrieveData = _MonitorFailedToRetrieveData;
        }
    }
}
