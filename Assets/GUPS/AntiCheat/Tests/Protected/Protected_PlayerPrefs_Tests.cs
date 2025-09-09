// System
using System;

// Test
using NUnit.Framework;

// Unity
using UnityEngine;
using UnityEngine.TestTools;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Storage;

// GUPS - AntiCheat
using GUPS.AntiCheat.Protected;
using GUPS.AntiCheat.Protected.Storage.Prefs;

namespace GUPS.AntiCheat.Tests
{
    /// <summary>
    /// Test fixture for testing the protected player preferences.
    /// </summary>
    [TestFixture]
    public class Protected_PlayerPrefs_Tests
    {
        private const String CKEY_NAME = "gups_test_key";

#if UNITY_EDITOR

        [SetUp]
        public void Setup()
        {
            // Load or create the global settings asset.
            GUPS.AntiCheat.Settings.GlobalSettings.LoadOrCreateAsset();
        }

#endif

        [TearDown]
        public void TearDown()
        {
            // Delete the key.
            ProtectedPlayerPrefs.DeleteKey(CKEY_NAME);

            // Unload the global settings singleton instance.
            GUPS.AntiCheat.Settings.GlobalSettings.Unload();
        }

        [Test]
        public void HasKey_Exists_Test()
        {
            // Arrange
            ProtectedPlayerPrefs.SetInt(CKEY_NAME, 1);

            // Act
            bool var_Result = ProtectedPlayerPrefs.HasKey(CKEY_NAME);

            // Assert
            Assert.AreEqual(true, var_Result);
        }

        [Test]
        public void HasKey_Not_Exists_Test()
        {
            // Act
            bool var_Result = ProtectedPlayerPrefs.HasKey(CKEY_NAME);

            // Assert
            Assert.AreEqual(false, var_Result);
        }


        [Test]
        public void HasKey_Hashed_Exists_Test()
        {
            // Settings
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Hash_Key = true;

            // Arrange
            ProtectedPlayerPrefs.SetInt(CKEY_NAME, 1);

            // Act
            bool var_Result = ProtectedPlayerPrefs.HasKey(CKEY_NAME);

            // Assert
            Assert.AreEqual(true, var_Result);
        }

        [Test]
        public void Set_Get_Protected_Test()
        {
            // Arrange
            ProtectedPlayerPrefs.Set(CKEY_NAME, new ProtectedInt32(1234));

            // Act
            bool var_Result = ProtectedPlayerPrefs.HasKey(CKEY_NAME);
            Int32 var_Value = ProtectedPlayerPrefs.GetInt(CKEY_NAME);

            // Assert
            Assert.AreEqual(true, var_Result);
            Assert.AreEqual(1234, var_Value);
        }

        [Test]
        public void Set_Get_Object_Test()
        {
            // Arrange
            ProtectedPlayerPrefs.Set(CKEY_NAME, 1234);

            // Act
            bool var_Result = ProtectedPlayerPrefs.HasKey(CKEY_NAME);
            Int32 var_Value = ProtectedPlayerPrefs.GetInt(CKEY_NAME);

            // Assert
            Assert.AreEqual(true, var_Result);
            Assert.AreEqual(1234, var_Value);
        }

        [Test]
        public void Set_Get_Encrypted_Object_Test()
        {
            // Settings
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Value_Encryption_Key = "AwesomeEncryptionKey";

            // Arrange
            ProtectedPlayerPrefs.Set(CKEY_NAME, 1234);

            // Act
            bool var_Result = ProtectedPlayerPrefs.HasKey(CKEY_NAME);
            Int32 var_Value = ProtectedPlayerPrefs.GetInt(CKEY_NAME);

            // Assert
            Assert.AreEqual(true, var_Result);
            Assert.AreEqual(1234, var_Value);
        }

        [Test]
        public void Set_Get_InvalidOwner_Object_ShouldThrowException_Test()
        {
            // Settings
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Allow_Read_Any_Owner = false;

            // Arrange
            ProtectedPlayerPrefs.Set(CKEY_NAME, 1234);

            // Act
            PreferenceStorageItem var_Result = ProtectedPlayerPrefs.GetRaw(CKEY_NAME);

            // Assert
            Assert.AreEqual(var_Result.Owner, UnityEngine.SystemInfo.deviceUniqueIdentifier);

            // Act
            var_Result.Owner = "NewOwner";
            ProtectedPlayerPrefs.SetRaw(CKEY_NAME, var_Result);

            // Act & Assert
            Assert.Throws<Exception>(() => ProtectedPlayerPrefs.GetInt(CKEY_NAME), ProtectedPlayerPrefs.ERROR_OWNER);
        }

        [Test]
        public void Set_Get_AnyOwner_Object_Test()
        {
            // Settings
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Allow_Read_Any_Owner = true;

            // Arrange
            ProtectedPlayerPrefs.Set(CKEY_NAME, 1234);

            // Act
            PreferenceStorageItem var_Result = ProtectedPlayerPrefs.GetRaw(CKEY_NAME);

            // Assert
            Assert.AreEqual(var_Result.Owner, UnityEngine.SystemInfo.deviceUniqueIdentifier);

            // Act
            var_Result.Owner = "NewOwner";
            ProtectedPlayerPrefs.SetRaw(CKEY_NAME, var_Result);

            // Act & Assert
            Int32 var_Value = ProtectedPlayerPrefs.GetInt(CKEY_NAME);
        }

        [Test]
        public void Set_Get_InvalidSignature_Object_ShouldThrowException_Test()
        {
            // Settings
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Hash_Key = false;
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Verify_Integrity = true;

            // Arrange
            ProtectedPlayerPrefs.Set(CKEY_NAME, 1234);

            // Act
            String var_Result = PlayerPrefs.GetString(CKEY_NAME);

            byte[] var_ResultBytes = Convert.FromBase64String(var_Result);
            var_ResultBytes[5] = 0x01; // 4 Byte the size, 1 Byte the type, and then the value.
            var_ResultBytes[5 + 1] = 0x02;
            var_ResultBytes[5 + 2] = 0x03;

            var_Result = Convert.ToBase64String(var_ResultBytes);

            PlayerPrefs.SetString(CKEY_NAME, var_Result);

            // Act & Assert
            Assert.Throws<Exception>(() => ProtectedPlayerPrefs.GetInt(CKEY_NAME), ProtectedPlayerPrefs.ERROR_INTEGRITY);
        }


        [Test]
        public void Set_Get_InvalidType_Object_ShouldThrowException_Test()
        {
            // Settings
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Hash_Key = false;
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Verify_Integrity = true;

            // Arrange
            ProtectedPlayerPrefs.Set(CKEY_NAME, 1234);

            // Act
            String var_Result = PlayerPrefs.GetString(CKEY_NAME);

            byte[] var_ResultBytes = Convert.FromBase64String(var_Result);
            var_ResultBytes[5] = (byte)EStorageType.Int64; // 4 Byte the size, 1 Byte the type, and then the value.

            var_Result = Convert.ToBase64String(var_ResultBytes);

            PlayerPrefs.SetString(CKEY_NAME, var_Result);

            // Act & Assert
            Assert.Throws<Exception>(() => ProtectedPlayerPrefs.GetInt(CKEY_NAME), ProtectedPlayerPrefs.ERROR_TYPE);
        }
    }
}
