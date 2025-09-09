// System
using System;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Monitor.Android
{
    public struct AndroidHashStatus : IAndroidStatus
    {
        /// <summary>
        /// Gets if the hash could not be retrieved or an exception occurred. And no valid value is returned.
        /// </summary>
        public bool FailedToRetrieveData { get; private set; }

        /// <summary>
        /// The algorithm used to hash the app.
        /// </summary>
        public String Algorithm { get; private set; }

        /// <summary>
        /// The hash of the whole app itself as hex string.
        /// </summary>
        public String Hash { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidHashStatus"/> struct.
        /// </summary>
        /// <param name="_FailedToRetrieveData">True if the data point could not be retrieved or exception occurred; otherwise, false.</param>
        /// <param name="Algorithm">The algorithm used to hash the app.</param>
        /// <param name="_Hash">The hash of the whole app itself.</param>
        public AndroidHashStatus(bool _FailedToRetrieveData, String _Algorithm, String _Hash) 
        {
            // Assign parameters.
            this.FailedToRetrieveData = _FailedToRetrieveData;
            this.Algorithm = _Algorithm;
            this.Hash = _Hash;
        }   
    }
}