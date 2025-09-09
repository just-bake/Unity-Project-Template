// System
using System;

// Unity
using UnityEngine;

// GUPS - AntiCheat
using GUPS.AntiCheat.Detector.Android;

namespace GUPS.AntiCheat.Demo.Demo_Android
{
    /// <summary>
    /// This class is used to disable the player double jumping in the game when the package certificate fingerprint of the app is not as expected and so the app was repacked by a third party.
    /// </summary>
    public class DisableDoubleJumping : MonoBehaviour
    {
        /// <summary>
        /// React to the detection of unexpected package certificate fingerprint.
        /// </summary>
        public EAndroidCheatingType ReactTo = EAndroidCheatingType.PACKAGE_FINGERPRINT;

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
        /// The message to display when the fingerprint is not as expected.
        /// </summary>
        public String Message = "Invalid Fingerprint: Deactivated Double Jumping";

        /// <summary>
        /// This method is assigned in the Inspector to the AndroidPackageTamperingDetector's OnCheatingDetected event.
        /// </summary>
        /// <param name="detectionStatus">The detection status of the AndroidPackageTamperingDetector.</param>
        public void ReactOnCheating(AndroidCheatingDetectionStatus detectionStatus)
        {
            // Only react to detection of unexpected package certificate fingerprint! Also react if the monitor failed to retrieve the package certificate fingerprint.
            if (detectionStatus.AndroidCheatingType == this.ReactTo)
            {
                // Disable the double jumping.
                PlayerController.CanDoubleJump = false;

                // Display the cross texture.
                this.Image.sprite = this.CrossTexture;

                // Display the message.
                this.Text.text = this.Message;
            }
        }
    }
}