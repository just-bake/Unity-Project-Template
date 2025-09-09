// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Monitor.Time
{
    /// <summary>
    /// Represents the status of a device's time, providing information about its deviation from expected time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="DeviceTimeStatus"/> struct is used to encapsulate the current time deviation status of a device, serving as a subject that can be watched or monitored.
    /// </para>
    /// </remarks>
    public struct DeviceTimeStatus : IWatchedSubject
    {
        /// <summary>
        /// Gets the deviation of the device's time from the expected time, utilizing the <see cref="ETimeDeviation"/> enumeration.
        /// </summary>
        public ETimeDeviation Deviation { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceTimeStatus"/> struct with the specified parameters.
        /// </summary>
        /// <param name="_Deviation">The deviation of the device's time from the expected time.</param>
        public DeviceTimeStatus(ETimeDeviation _Deviation)
        {
            this.Deviation = _Deviation;
        }
    }
}
