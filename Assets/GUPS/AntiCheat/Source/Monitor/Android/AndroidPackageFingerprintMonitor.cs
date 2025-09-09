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
    /// Represents a monitor designed for Android devices to calculate their fingerprints or signatures (of APK/AAB files) 
    /// and notifies observers about the calculated fingerprint. This fingerprint can be used to detect if the application was build
    /// and signed by the original developer or someone else.
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
    /// The monitor retrieves the algorithm to use for fingerprint calculation from the global settings. If the algorithm is not specified, 
    /// it defaults to "SHA-256". It then attempts to calculate the fingerprint using the provided algorithm and notifies observers with 
    /// the calculated fingerprint or null in case of failure and logs a warning message.
    /// </para>
    /// </remarks>
    public class AndroidPackageFingerprintMonitor : AMonitor
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the Monitor.
        /// </summary>
        public override String Name => "Android Package Fingerprint Monitor";

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On start read the fingerprint / signature and notify the observer.
        /// </summary>
        protected override void OnStart()
        {
            // Call the base method.
            base.OnStart();

#if UNITY_ANDROID && !UNITY_EDITOR

            // Get the algorithm.
            String var_Algorithm = HashHelper.GetName(this.GetAlgorithm());

            // Try to get the fingerprint.
            bool var_Success = this.TryGetFingerprint(var_Algorithm, out String var_Fingerprint);

            // Notify the observer.
            this.Notify(new AndroidFingerprintStatus(!var_Success, var_Algorithm, var_Fingerprint));
            
#else

            UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] {0} is only available on Android devices!", this.Name));

#endif
        }

        #endregion

        // Fingerprint
        #region Fingerprint

        /// <summary>
        /// Get the algorithm to use for the fingerprint.
        /// </summary>
        /// <returns>Returns the algorithm to use.</returns>
        private EHashAlgorithm GetAlgorithm()
        {
            return GlobalSettings.Instance?.Android_AppFingerprintAlgorithm ?? EHashAlgorithm.SHA256;
        }

        /// <summary>
        /// Tries to retrieve the fingerprint of the installed application on the device.
        /// </summary>
        /// <param name="_Algorithm">The algorithm to use for fingerprint calculation.</param>
        /// <param name="_Fingerprint">The fingerprint of the installed application.</param>
        /// <returns>True if the fingerprint could be read, false otherwise.</returns>
        private bool TryGetFingerprint(String _Algorithm, out String _Fingerprint)
        {
            try
            {
                // Access directly using a native Java class.
                using (AndroidJavaClass var_JavaClass = new AndroidJavaClass("com.gups.anticheat.android.signature.SignatureReader"))
                {
                    String var_Result = var_JavaClass.CallStatic<String>("getSigningSignature", _Algorithm);

                    if (var_Result == null)
                    {
                        // Retrun failure and null fingerprint.
                        _Fingerprint = null;

                        return false;
                    }

                    // Log the found signature as debug info.
                    UnityEngine.Debug.Log(String.Format("[GUPS][AntiCheat] Calculated app signature: '{0}'", var_Result));

                    // Return success and the fingerprint.
                    _Fingerprint = var_Result;

                    return true;
                }
            }
            catch (Exception var_Exception)
            {
                UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] Could not read android fingerprint / signature: {0}!", var_Exception));

                _Fingerprint = null;

                return false;
            }
        }

        #endregion
    }
}
