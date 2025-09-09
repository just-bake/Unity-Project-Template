// System
using System;
using System.Collections.Generic;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Monitor.Android
{
    /// <summary>
    /// This status is used to communicate between the <see cref="GUPS.AntiCheat.Monitor.Android.AndroidInstalledApplicationMonitor"/> 
    /// and the observers, by default this is the <see cref="GUPS.AntiCheat.Detector.Android.AndroidDeviceCheatingDetector"/>.
    /// It updates the observers about the installed applications on the device.
    /// </summary>
    public struct AndroidInstalledApplicationStatus : IAndroidStatus
    {
        /// <summary>
        /// Gets if the apps on the device could not be retrieved or an exception occurred. And no valid value is returned.
        /// </summary>
        public bool FailedToRetrieveData { get; private set; }

        /// <summary>
        /// The list of found installed applications on the device.
        /// </summary>
        public List<String> Applications { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidInstalledApplicationStatus"/> struct.
        /// </summary>
        /// <param name="_FailedToRetrieveData">True if the data point could not be retrieved or exception occurred; otherwise, false.</param>
        /// <param name="_Applications">The list of installed applications on the device.</param>
        public AndroidInstalledApplicationStatus(bool _FailedToRetrieveData, List<String> _Applications)
        {
            // Assign parameters.
            this.FailedToRetrieveData = _FailedToRetrieveData;
            this.Applications = _Applications;
        }
    }
}