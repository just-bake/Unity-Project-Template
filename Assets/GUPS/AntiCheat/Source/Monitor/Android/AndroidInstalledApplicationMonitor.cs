// System
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;

// GUPS - AntiCheat
using GUPS.AntiCheat.Settings;

namespace GUPS.AntiCheat.Monitor.Android
{
    /// <summary>
    /// Represents a monitor designed for Android devices to read and provide information about installed applications on the 
    /// device (ignoring system apps). This monitor notifies observers about the installed applications by passing a list of 
    /// package names.
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
    /// The monitor retrieves the list of installed applications when it starts and notifies observers with the status of the installed 
    /// applications. If any errors occur during the process of retrieving the installed applications a warning message will be logged.
    /// Also an empty list, with the notification that something failed, is passed to the observers.
    /// </para>
    /// </remarks>
    public class AndroidInstalledApplicationMonitor : AMonitor
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the Monitor.
        /// </summary>
        public override String Name => "Android Installed Applications Monitor";

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On start read the installed applications and notify the observer.
        /// </summary>
        protected override void OnStart()
        {
            // Call the base method.
            base.OnStart();

#if UNITY_ANDROID && !UNITY_EDITOR

            // Try to get the installed applications.
            bool var_Success = this.TryGetInstalledApplications(out List<String> var_Applications, this.GetAppPackagesToFind());

            // Notify the observer.
            this.Notify(new AndroidInstalledApplicationStatus(!var_Success, var_Applications));

#else

            UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] {0} is only available on Android devices!", this.Name));

#endif
        }

        #endregion

        // Application
        #region Application

        /// <summary>
        /// Gets the list of applications to find from the global settings.
        /// </summary>
        /// <returns>The list of applications to find or an empty list if the global settings are not available.</returns>
        private List<String> GetAppPackagesToFind()
        {
            return GlobalSettings.Instance?.Android_BlacklistedApplications ?? new List<String>();
        }

        /// <summary>
        /// Tries to retrieve the installed applications on the device.
        /// </summary>
        /// <param name="_FoundApplications">A list of installed applications.</param>
        /// <returns>True if the applications could be read, otherwise false.</returns>
        private bool TryGetInstalledApplications(out List<String> _FoundApplications, List<String> _SearchApplications)
        {
            try
            {
                // Access directly using a native Java class.
                using (AndroidJavaClass var_JavaClass = new AndroidJavaClass("com.gups.anticheat.android.app.ApplicationReader"))
                {
                    // Note: Make sure to cast the array as an object.
                    String[] var_Result = var_JavaClass.CallStatic<String[]>("getSpecificInstalledAppPackages", (object)_SearchApplications.ToArray());

                    if (var_Result == null)
                    {
                        // Return failure and an empty list of applications.
                        _FoundApplications = new List<String>();

                        return false;
                    }

                    // Log the found applications as debug info.
                    UnityEngine.Debug.Log(String.Format("[GUPS][AntiCheat] Found installed applications: '{0}'", String.Join(", ", var_Result)));

                    // Return success and the list of applications.
                    _FoundApplications = new List<String>(var_Result);

                    return true;
                }
            }
            catch (Exception var_Exception)
            {
                UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] Could not read android applications: {0}!", var_Exception));

                _FoundApplications = new List<String>();

                return false;
            }
        }

        #endregion
    }
}
