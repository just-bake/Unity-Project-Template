// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Watch;

namespace GUPS.AntiCheat.Monitor.Time
{
    /// <summary>
    /// Represents a structure for monitoring and conveying the status of game time deviation, implementing the <see cref="IWatchedSubject"/> interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="GameTimeStatus"/> struct encapsulates information about the deviation of game time, allowing observers to stay informed about any time-related manipulations.
    /// </para>
    /// </remarks>
    public struct GameTimeStatus : IWatchedSubject
    {
        /// <summary>
        /// Gets the deviation of game time, indicating any time-related manipulations.
        /// </summary>
        public ETimeDeviation Deviation { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameTimeStatus"/> struct with the specified time deviation.
        /// </summary>
        /// <param name="_Deviation">The time deviation to be associated with the game time status.</param>
        public GameTimeStatus(ETimeDeviation _Deviation)
        {
            this.Deviation = _Deviation;
        }
    }
}
