// System
using System;

// Unity
using UnityEngine;

// GUPS - AntiCheat
using GUPS.AntiCheat.Detector.Android;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
    /// <summary>
    /// This class is used to disable the player winning in the game when the package hash of the app is not as expected and so the app was repacked by a third party.
    /// </summary>
    public class DisableWinning : MonoBehaviour
    {
        /// <summary>
        /// React to the detection of unexpected package hash.
        /// </summary>
        public EAndroidCheatingType ReactTo = EAndroidCheatingType.PACKAGE_HASH;

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
        /// The message to display when the package hash is not as expected.
        /// </summary>
        public String Message = "Invalid Hash: Deactivated Winning";

        /// <summary>
        /// This method is assigned in the Inspector to the AndroidPackageTamperingDetector's OnCheatingDetected event.
        /// </summary>
        /// <param name="detectionStatus">The detection status of the AndroidPackageTamperingDetector.</param>
        public void ReactOnCheating(AndroidCheatingDetectionStatus detectionStatus)
        {
            // Only react to the detection of an unexpected calculated package hash! Also react if the monitor failed to retrieve the package hash.
            if (detectionStatus.AndroidCheatingType == this.ReactTo)
            {
                // Disables the player winning in the game, by deactivating the interaction with the goal flag.
                InteractiveGoalFlagController.IsInteractAble = false;

                // Display the cross texture.
                this.Image.sprite = this.CrossTexture;

                // Display the message.
                this.Text.text = this.Message;
            }
        }
    }
}