// System
using System;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Hash;

// GUPS - AntiCheat
using GUPS.AntiCheat.Settings;

namespace GUPS.AntiCheat.Monitor.Android
{
    /// <summary>
    /// Represents a monitor designed for Android devices to calculate the hash of the entire app (APK/AAB) itself and notifies observers about 
    /// the calculated hash. This hash can be compared with a remote source to detect if the app is in it original state or was modified or 
    /// tampered with.
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
    /// The monitor retrieves the algorithm to use for hash calculation from the global settings. If the algorithm is not specified, it defaults 
    /// to "SHA-256". It then attempts to calculate the hash using the provided algorithm and notifies observers with the calculated hash or 
    /// null in case of failure and logs a warning message.
    /// </para>
    /// </remarks>
    public class AndroidPackageHashMonitor : AMonitor
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the Monitor.
        /// </summary>
        public override String Name => "Android Package Hash Monitor";

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On start calculate the hash of the whole app (apk / aab) and notify the observer.
        /// </summary>
        protected override void OnStart()
        {
            // Call the base method.
            base.OnStart();

#if UNITY_ANDROID && !UNITY_EDITOR

            // Get the algorithm.
            String var_Algorithm = HashHelper.GetName(this.GetAlgorithm());

            // Try to get the hash.
            bool var_Success = this.TryGetHash(var_Algorithm, out String var_Hash);

            // Notify the observer.
            this.Notify(new AndroidHashStatus(!var_Success, var_Algorithm, var_Hash));
            
#else

            UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] {0} is only available on Android devices!", this.Name));

#endif
        }

        #endregion

        // Hash
        #region Hash

        /// <summary>
        /// Get the algorithm to use for the hash.
        /// </summary>
        /// <returns>Returns the algorithm to use.</returns>
        private EHashAlgorithm GetAlgorithm()
        {
            return GlobalSettings.Instance?.Android_AppFingerprintAlgorithm ?? EHashAlgorithm.SHA256;
        }

        /// <summary>
        /// Tries to retrieve the hash of the installed application on the device.
        /// </summary>
        /// <param name="_Algorithm">The algorithm to use for hash calculation.</param>
        /// <param name="_Hash">The hash of the installed application.</param>
        /// <returns>True if the hash could be read, false otherwise.</returns>
        private bool TryGetHash(String _Algorithm, out String _Hash)
        {
            try
            {
                // Access directly using a native Java class.
                using (AndroidJavaClass var_JavaClass = new AndroidJavaClass("com.gups.anticheat.android.hash.HashReader"))
                {
                    String var_Result = var_JavaClass.CallStatic<String>("getAppHash", _Algorithm);

                    if (var_Result == null)
                    {
                        // Return failure and a null hash.
                        _Hash = null;

                        return false;
                    }

                    // Log the found hash as debug info.
                    UnityEngine.Debug.Log(String.Format("[GUPS][AntiCheat] Calculated app hash: '{0}'", var_Result));

                    // Return success and the hash.
                    _Hash = var_Result;

                    return true;
                }
            }
            catch (Exception var_Exception)
            {
                UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] Could not read android app hash: {0}!", var_Exception));

                _Hash = null;

                return false;
            }
        }

        #endregion
    }
}
