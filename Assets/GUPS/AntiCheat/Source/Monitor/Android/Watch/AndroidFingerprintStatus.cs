// System
using System;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Monitor.Android
{
    public struct AndroidFingerprintStatus : IAndroidStatus
    {
        /// <summary>
        /// Gets if the certificates fingerprint could not be retrieved or an exception occurred. And no valid value is returned.
        /// </summary>
        public bool FailedToRetrieveData { get; private set; }

        /// <summary>
        /// The algorithm used to determine the fingerprint / signature.
        /// </summary>
        public String Algorithm { get; private set; }

        /// <summary>
        /// The public fingerprint / signature of the app which it was signed with.
        /// </summary>
        public String Fingerprint { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidFingerprintStatus"/> struct.
        /// </summary>
        /// <param name="_FailedToRetrieveData">True if the data point could not be retrieved or exception occurred; otherwise, false.</param>
        /// <param name="_Algorithm">The algorithm used to determine the fingerprint / signature.</param>
        /// <param name="_Fingerprint">The public fingerprint / signature of the app which it was signed with.</param>
        public AndroidFingerprintStatus(bool _FailedToRetrieveData, String _Algorithm, String _Fingerprint) 
        {
            // Assign parameters.
            this.FailedToRetrieveData = _FailedToRetrieveData;
            this.Algorithm = _Algorithm;
            this.Fingerprint = _Fingerprint;
        }   
    }
}