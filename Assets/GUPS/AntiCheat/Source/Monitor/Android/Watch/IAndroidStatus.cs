// System
using System;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Monitor.Android
{
    public interface IAndroidStatus : IWatchedSubject
    {
        /// <summary>
        /// Gets if the specific data point could not be retrieved or an exception occurred. And no valid value is returned.
        /// </summary>
        bool FailedToRetrieveData { get; }
    }
}