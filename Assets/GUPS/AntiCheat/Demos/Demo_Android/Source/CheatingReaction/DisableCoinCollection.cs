// System
using System;

// Unity
using UnityEngine;

// GUPS - AntiCheat
using GUPS.AntiCheat.Detector.Android;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
    /// <summary>
    /// This class is used to disable the player coin collection in the game when an unallowed app is installed on the device.
    /// </summary>
    public class DisableCoinCollection : MonoBehaviour
    {
        /// <summary>
        /// React to the detection of unallowed apps.
        /// </summary>
        public EAndroidCheatingType ReactTo = EAndroidCheatingType.DEVICE_INSTALLED_APPS;

        /// <summary>
        /// The image component to display the checkmark / cross.
        /// </summary>
        public UnityEngine.UI.Image Image;

        /// <summary>
        /// The cross texture to display on cheating.
        /// </summary>
        public Sprite CrossTexture;

        /// <summary>
        /// The text component to display the message.
        /// </summary>
        public UnityEngine.UI.Text Text;

        /// <summary>
        /// The message to display when an unallowed app is detected.
        /// </summary>
        public String Message = "Unallowed Apps: Deactivated Coin collection";

        /// <summary>
        /// This method is assigned in the Inspector to the AndroidPackageTamperingDetector's OnCheatingDetected event.
        /// </summary>
        /// <param name="detectionStatus">The detection status of the AndroidPackageTamperingDetector.</param>
        public void ReactOnCheating(AndroidCheatingDetectionStatus detectionStatus)
        {
            // Only reacts to detection of unallowed apps! Also react if the monitor failed to retrieve the installed apps.
            if (detectionStatus.AndroidCheatingType == this.ReactTo)
            {
                // Disables the coin collection in the game, by deactivating the interaction with the coins.
                InteractiveCoinController.IsInteractAble = false;

                // Display the cross texture.
                this.Image.sprite = this.CrossTexture;

                // Display the message.
                this.Text.text = this.Message;
            }
        }
    }
}