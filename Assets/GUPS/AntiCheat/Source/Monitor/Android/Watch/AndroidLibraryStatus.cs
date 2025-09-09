// System
using System;
using System.Collections.Generic;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Monitor.Android
{
    public struct AndroidLibraryStatus : IAndroidStatus
    {
        /// <summary>
        /// Gets if the libraries could not be retrieved or an exception occurred. And no valid value is returned.
        /// </summary>
        public bool FailedToRetrieveData { get; private set; }

        /// <summary>
        /// The list of libraries used by the app on the device.
        /// </summary>
        public List<String> Libraries { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidLibraryStatus"/> struct.
        /// </summary>
        /// <param name="_FailedToRetrieveData">True if the data point could not be retrieved or exception occurred; otherwise, false.</param>
        /// <param name="_Libraries">The list of libraries used by the app on the device.</param>
        public AndroidLibraryStatus(bool _FailedToRetrieveData, List<String> _Libraries) 
        {
            // Assign parameters.
            this.FailedToRetrieveData = _FailedToRetrieveData;
            this.Libraries = _Libraries;
        }   
    }
}