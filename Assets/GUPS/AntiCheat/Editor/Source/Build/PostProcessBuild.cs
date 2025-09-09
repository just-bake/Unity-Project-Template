// Microsoft
using System;
using System.IO;

// Unity
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Hash;

// GUPS - AntiCheat
using GUPS.AntiCheat.Settings;

namespace GUPS.AntiCheat.Editor.Build
{
    /// <summary>
    /// Represents a postprocessor that is executed after the Unity build process is complete.
    /// This postprocessor calculates the hash of the generated Android APK/AAB file and displays it in the console.
    /// The calculated hash can be used to verify the integrity of the app by comparing it with the hash of the app running on a device.
    /// </summary>
    /// <remarks>
    /// This class is specifically designed for Android builds and relies on the hash algorithm specified in the global settings.
    /// If the hash algorithm is set to `NONE`, the hash calculation will be skipped, and a warning will be logged.
    /// </remarks>
    internal class PostProcessBuild : IPostprocessBuildWithReport
    {
        /// <summary>
        /// Gets the callback order of this postprocessor.
        /// </summary>
        /// <remarks>
        /// The callback order is set to a high value (<see cref="Int32.MaxValue"/> - 1) to ensure
        /// that this postprocessor is executed near the end of the post-processing pipeline.
        /// This ensures that the build process is fully completed before the hash is calculated.
        /// </remarks>
        public int callbackOrder => Int32.MaxValue - 1;

        /// <summary>
        /// Executes after the build process finishes.
        /// Calculates the hash of the generated Android APK/AAB file and logs it to the console.
        /// </summary>
        /// <param name="_Report">The build report containing information about the completed build.</param>
        /// <remarks>
        /// This method is only executed for Android builds. It performs the following steps:
        /// <list type="number">
        /// <item>Retrieves the output path of the build.</item>
        /// <item>Determines the hash algorithm to use from the global settings.</item>
        /// <item>If the hash algorithm is set to `NONE`, logs a warning and skips the hash calculation.</item>
        /// <item>Calculates the hash of the APK/AAB file using the specified algorithm.</item>
        /// <item>Logs the calculated hash, the algorithm used, the app version, and the file path to the console.</item>
        /// </list>
        /// </remarks>
        public void OnPostprocessBuild(BuildReport _Report)
        {
#if UNITY_ANDROID

            // Get the output path of the build.
            String var_OutputPath = _Report.summary.outputPath;

            // Get the hash algorithm to calculate the hash of the build APK/AAB file.
            EHashAlgorithm var_HashAlgorithm = GlobalSettings.Instance?.Android_AppHashAlgorithm ?? EHashAlgorithm.SHA256;

            // Check if the hash algorithm is set to NONE; skip the hash calculation if so.
            if (var_HashAlgorithm == EHashAlgorithm.NONE)
            {
                UnityEngine.Debug.LogWarning("[GUPS][AntiCheat] The hash algorithm is set to NONE. The hash of the build APK/AAB file will not be calculated.");
                return;
            }

            // Calculate the hash of the build APK/AAB file.
            String var_Hash = CalculateHexedHash(var_OutputPath, var_HashAlgorithm);

            // Log the calculated hash to the console.
            UnityEngine.Debug.Log(String.Format(
                "[GUPS][AntiCheat] App hash: {0} with algorithm: {1} for version: {2} at path: {3}.",
                var_Hash,
                HashHelper.GetName(var_HashAlgorithm),
                UnityEngine.Application.version,
                var_OutputPath));

#endif
        }

        /// <summary>
        /// Calculates the hash of a file at the specified path and returns it as a hexadecimal string.
        /// </summary>
        /// <param name="_Path">The file path of the APK/AAB to hash.</param>
        /// <param name="_HashAlgorithm">The hash algorithm to use (e.g., SHA256, MD5).</param>
        /// <returns>The hash of the file as a hex-encoded string.</returns>
        /// <remarks>
        /// This method reads the file as a stream and calculates its hash using the specified hash algorithm.
        /// The hash is then converted to a hexadecimal string for display or comparison purposes.
        /// </remarks>
        private String CalculateHexedHash(String _Path, EHashAlgorithm _HashAlgorithm)
        {
            // Open the file as a stream.
            using (FileStream var_FileStream = new FileStream(_Path, FileMode.Open, FileAccess.Read))
            {
                // Compute the hash of the file.
                byte[] var_HashedBytes = HashHelper.ComputeHash(_HashAlgorithm, var_FileStream);

                // Convert the hash to a hexadecimal string and return it.
                return HashHelper.ToHex(var_HashedBytes, true, true);
            }
        }
    }
}
