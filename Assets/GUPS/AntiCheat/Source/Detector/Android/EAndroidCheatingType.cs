namespace GUPS.AntiCheat.Detector.Android
{
    /// <summary>
    /// Enum representing various types of cheating that can occur on an Android device.
    /// </summary>
    public enum EAndroidCheatingType : byte
    {
        /// <summary>
        /// A default value representing an unknown cheating type.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// The app package installation source is not from the allowed stores.
        /// </summary>
        PACKAGE_SOURCE = 1,

        /// <summary>
        /// The app package hash is not the expected hash.
        /// </summary>
        PACKAGE_HASH = 2,

        /// <summary>
        /// The app package certificates fingerprint is not the expected fingerprint.
        /// </summary>
        PACKAGE_FINGERPRINT = 3,

        /// <summary>
        /// The app package libraries contain a library that is not allowed.
        /// </summary>
        PACKAGE_LIBRARY = 4,

        /// <summary>
        /// The device has installed apps that are not allowed.
        /// </summary>
        DEVICE_INSTALLED_APPS = 10,
    }
}
