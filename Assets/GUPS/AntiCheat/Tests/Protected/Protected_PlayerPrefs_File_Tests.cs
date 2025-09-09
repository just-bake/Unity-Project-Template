// System
using System;

// Test
using NUnit.Framework;

// Unity
using UnityEngine;
using UnityEngine.TestTools;

// GUPS - AntiCheat
using GUPS.AntiCheat.Protected;
using GUPS.AntiCheat.Protected.Storage.Prefs;

namespace GUPS.AntiCheat.Tests
{
    /// <summary>
    /// Test fixture for testing the protected player file preferences.
    /// </summary>
    [TestFixture]
    public class Protected_PlayerPrefs_File_Tests
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
            ProtectedFileBasedPlayerPrefs.DeleteKey(CKEY_NAME);

            // Unload the global settings singleton instance.
            GUPS.AntiCheat.Settings.GlobalSettings.Unload();
        }

        [Test]
        public void HasKey_Exists_Test()
        {
            // Arrange
            ProtectedFileBasedPlayerPrefs.SetInt(CKEY_NAME, 1);

            // Act
            bool var_Result = ProtectedFileBasedPlayerPrefs.HasKey(CKEY_NAME);

            // Assert
            Assert.AreEqual(true, var_Result);
        }

        [Test]
        public void HasKey_Not_Exists_Test()
        {
            // Act
            bool var_Result = ProtectedFileBasedPlayerPrefs.HasKey(CKEY_NAME);

            // Assert
            Assert.AreEqual(false, var_Result);
        }


        [Test]
        public void HasKey_Hashed_Exists_Test()
        {
            // Settings
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Hash_Key = true;

            // Arrange
            ProtectedFileBasedPlayerPrefs.SetInt(CKEY_NAME, 1);

            // Act
            bool var_Result = ProtectedFileBasedPlayerPrefs.HasKey(CKEY_NAME);

            // Assert
            Assert.AreEqual(true, var_Result);
        }

        [Test]
        public void Set_Get_Protected_Test()
        {
            // Arrange
            ProtectedFileBasedPlayerPrefs.Set(CKEY_NAME, new ProtectedInt32(1234));

            // Act
            bool var_Result = ProtectedFileBasedPlayerPrefs.HasKey(CKEY_NAME);
            Int32 var_Value = ProtectedFileBasedPlayerPrefs.GetInt(CKEY_NAME);

            // Assert
            Assert.AreEqual(true, var_Result);
            Assert.AreEqual(1234, var_Value);
        }

        [Test]
        public void Set_Get_Object_Test()
        {
            // Arrange
            ProtectedFileBasedPlayerPrefs.Set(CKEY_NAME, 1234);

            // Act
            bool var_Result = ProtectedFileBasedPlayerPrefs.HasKey(CKEY_NAME);
            Int32 var_Value = ProtectedFileBasedPlayerPrefs.GetInt(CKEY_NAME);

            // Assert
            Assert.AreEqual(true, var_Result);
            Assert.AreEqual(1234, var_Value);
        }

        [Test]
        public void Save_Test()
        {
            // Arrange
            ProtectedFileBasedPlayerPrefs.SetInt(CKEY_NAME, 1234);

            // Act
            ProtectedFileBasedPlayerPrefs.Save();

            // Assert
            Assert.AreEqual(true, ProtectedFileBasedPlayerPrefs.HasKey(CKEY_NAME));
        }

        [Test]
        public void Save_Encrypted_Test()
        {
            // Settings
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Value_Encryption_Key = "AwesomeEncryptionKey";

            // Arrange
            ProtectedFileBasedPlayerPrefs.SetInt(CKEY_NAME, 1234);

            // Act
            ProtectedFileBasedPlayerPrefs.Save();

            // Assert
            Assert.AreEqual(true, ProtectedFileBasedPlayerPrefs.HasKey(CKEY_NAME));
        }

        [Test]
        public void Load_Test()
        {
            // Act
            ProtectedFileBasedPlayerPrefs.Load();
        }

        [Test]
        public void Load_Encrypted_Test()
        {
            // Settings
            GUPS.AntiCheat.Settings.GlobalSettings.Instance.PlayerPreferences_Value_Encryption_Key = "AwesomeEncryptionKey";

            // Act
            ProtectedFileBasedPlayerPrefs.Load();
        }
    }
}
