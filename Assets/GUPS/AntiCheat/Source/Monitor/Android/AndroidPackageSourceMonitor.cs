// System
using System;

// Unity
using UnityEngine;

namespace GUPS.AntiCheat.Monitor.Android
{
    /// <summary>
    /// Represents a monitor designed for Android devices to determine the installation source (app store) of the app (APK/AAB) and notifies 
    /// observers about the app store from which the app was installed. If the installation source cannot be determined or is unknown, 
    /// <see cref="EAppStore.Unknown"/> is passed to the observers along with a notification of failure.
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
    /// The monitor attempts to determine the installation source of the application when it starts. If successful, it notifies observers 
    /// with the identified app store. If unsuccessful, it notifies observers with <see cref="EAppStore.Unknown"/> and logs a warning message.
    /// </para>
    /// </remarks>
    public class AndroidPackageSourceMonitor : AMonitor
    {
        // Name
        #region Name

        /// <summary>
        /// The name of the Monitor.
        /// </summary>
        public override String Name => "Android Package Source Monitor";

        #endregion

        // Lifecycle
        #region Lifecycle

        /// <summary>
        /// On start read the installation source of the app (apk / aab) and notify the observer.
        /// </summary>
        protected override void OnStart()
        {
            // Call the base method.
            base.OnStart();

#if UNITY_ANDROID && !UNITY_EDITOR

            // Try to get the installation source.
            bool var_Success = this.TryGetAppStoreSource(out EAppStore var_AppStore, out String _AppStorePackage);

            // Notify the observer.
            this.Notify(new AndroidSourceStatus(!var_Success, var_AppStore, _AppStorePackage));
            
#else

            UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] {0} is only available on Android devices!", this.Name));

#endif
        }

        #endregion

        // Source
        #region Source

        /// <summary>
        /// Try to get the installation source of the installed application on the device.
        /// </summary>
        /// <param name="_AppStore">The app store source of the application.</param>
        /// <param name="_AppStorePackage">The app store source of the application as string package name.</param>
        /// <returns>True if the app store source could be read, false otherwise.</returns>
        private bool TryGetAppStoreSource(out EAppStore _AppStore, out String _AppStorePackage)
        {
            try
            {
                // Access directly using a native Java class.
                using (AndroidJavaClass var_JavaClass = new AndroidJavaClass("com.gups.anticheat.android.store.PackageInstallerReader"))
                {
                    String var_Result = var_JavaClass.CallStatic<String>("getAppStore");

                    if (var_Result == null)
                    {
                        // Return failure and unknown app store.
                        _AppStore = EAppStore.Unknown;
                        _AppStorePackage = null;

                        return false;
                    }

                    // Log the found installation source as debug info.
                    UnityEngine.Debug.Log(String.Format("[GUPS][AntiCheat] App installation source: '{0}'", var_Result));

                    // Return success and the app store source.
                    _AppStore = AppStoreHelper.GetStore(var_Result);
                    _AppStorePackage = var_Result;

                    return true;
                }
            }
            catch (Exception var_Exception)
            {
                UnityEngine.Debug.LogWarning(String.Format("[GUPS][AntiCheat] Could not read android app store source: {0}!", var_Exception));

                _AppStore = EAppStore.Unknown;
                _AppStorePackage = null;

                return false;
            }
        }

        #endregion
    }
}
