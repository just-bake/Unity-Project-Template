namespace GUPS.AntiCheat.Monitor.Time
{
    /// <summary>
    /// Enumeration representing different types of time deviations that can occur.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ETimeDeviation"/> enum defines the possible types of time deviations that may occur during time comparisons for cheat detection. 
    /// Each member represents a specific kind of deviation, including no deviation (<see cref="None"/>), time stopping (<see cref="Stopped"/>), 
    /// time slowing down (<see cref="SlowedDown"/>), and time speeding up (<see cref="SpeedUp"/>).
    /// </para>
    /// </remarks>
    public enum ETimeDeviation : byte
    {
        /// <summary>
        /// No time deviation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that time has stopped.
        /// </summary>
        Stopped = 1,

        /// <summary>
        /// Indicates that time is slowing down.
        /// </summary>
        SlowedDown = 2,

        /// <summary>
        /// Indicates that time is speeding up.
        /// </summary>
        SpeedUp = 3
    }
}