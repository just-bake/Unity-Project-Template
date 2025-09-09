// System
using System;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Monitor.Android
{
    public struct AndroidSourceStatus : IAndroidStatus
    {
        /// <summary>
        /// Gets if the installation source could not be retrieved or an exception occurred. And no valid value is returned.
        /// </summary>
        public bool FailedToRetrieveData { get; private set; }

        /// <summary>
        /// Gets the app store source, this app was downloaded and installed from.
        /// </summary>
        public EAppStore AppStoreSource { get; private set; }

        /// <summary>
        /// Gets the app store source package name, this app was downloaded and installed from.
        /// </summary>
        public String AppStoreSourcePackage { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidSourceStatus"/> struct.
        /// </summary>
        /// <param name="_FailedToRetrieveData">True if the data point could not be retrieved or exception occurred; otherwise, false.</param>
        /// <param name="_Source">The app store source, this app was downloaded and installed from.</param>
        /// <param name="_AppStoreSourcePackage">The app store source package name, this app was downloaded and installed from.</param>
        public AndroidSourceStatus(bool _FailedToRetrieveData, EAppStore _Source, String _AppStoreSourcePackage) 
        {
            // Assign parameters.
            this.FailedToRetrieveData = _FailedToRetrieveData;
            this.AppStoreSource = _Source;
            this.AppStoreSourcePackage = _AppStoreSourcePackage;
        }   
    }
}