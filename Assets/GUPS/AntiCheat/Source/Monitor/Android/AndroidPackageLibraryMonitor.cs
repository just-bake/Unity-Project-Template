// System
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Monitor.Android
{
    /// <summary>
    /// Represents a monitor designed for Android devices to read the libraries contained within the app (APK/AAB) itself, such as 
    /// those found in '[APK]\lib\armeabi-v7a\', and notifies observers about the libraries present in the application. Typically,
    /// a cheater or hacker may add additional libraries to the app to manipulate the game and gain an unfair advantage.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class extends the functionality of the abstract base class <see cref="AMonitor"/>, which defines common functionality for 
    /// a monitor in a Unity environment.
    /// </para>
    /// <para>
    /// The monitor controls its lifecycle, including starting, pausing, resuming, and stopping, through methods provided by the 
    /// <see cref="AMonitor"/> class. It also supports the observer pattern, allowing observers to subscribe and receive notifications 
    /// about relevant events during the monitor's lifecycle.
    /// </para>
    /// <para>
    /// The monitor attempts to read the libraries stored in the application when it starts. If successful, it notifies observers with the 
    /// list of libraries. If unsuccessful, it notifies observers with an empty list and logs a warning message.
    /// </para>
    /// </remarks>
    public class AndroidPackageLibraryMonitor : AMonitor
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the Monitor.
        /// </summary>
        public override String Name => "Android Package Library Monitor";

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On start read the libraries stores in the app (apk / aab) and notify the observer.
        /// </summary>
        protected override void OnStart()
        {
            // Call the base method.
            base.OnStart();

#if UNITY_ANDROID && !UNITY_EDITOR

            // Try to get the libraries.
            bool var_Success = this.TryGetLibraries(out List<String> var_Libraries);

            // Notify the observer.
            this.Notify(new AndroidLibraryStatus(!var_Success, var_Libraries));
            
#else

            UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] {0} is only available on Android devices!", this.Name));

#endif
        }

        #endregion

        // Library
        #region Library

        /// <summary>
        /// Tries to retrieve the libraries of the installed application on the device.
        /// </summary>
        /// <param name="_Libraries">The list of libraries in the application.</param>
        /// <returns>True if the libraries could be read, false otherwise.</returns>
        private bool TryGetLibraries(out List<String> _Libraries)
        {
            try
            {
                // Access directly using a native Java class.
                using (AndroidJavaClass var_JavaClass = new AndroidJavaClass("com.gups.anticheat.android.library.LibraryReader"))
                {
                    String[] var_Result = var_JavaClass.CallStatic<String[]>("getLibraryNames");

                    if (var_Result == null)
                    {
                        // Return failure and an empty list of libraries.
                        _Libraries = new List<String>();

                        return false;
                    }

                    // Log the found libraries as debug info.
                    UnityEngine.Debug.Log(String.Format("[GUPS][AntiCheat] Found package libraries in app: '{0}'", String.Join(", ", var_Result)));

                    // Return success and the list of found libraries.
                    _Libraries = new List<String>(var_Result);

                    return true;
                }
            }
            catch (Exception var_Exception)
            {
                UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] Could not read android app libraries: {0}!", var_Exception));

                _Libraries = new List<String>();

                return false;
            }
        }

        #endregion
    }
}
