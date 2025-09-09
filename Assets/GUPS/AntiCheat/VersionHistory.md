# Version History:

## 2025.1.2: Minor Fixes
Bug Fixes:
- [Bug-2025-2] Primitives such as int, float, double, etc. return with false for the HasIntegrity property if no value has yet been set. [thanks David]
- [Bug-2025-3] The Android monitors have not disposed the Java callbacks. This could lead to memory leaks. [thanks David]

## 2025.1.1: Release Day - Patch
Bug Fixes:
- [Bug-2025-1] For the default ProtectedFileBasedPlayerPrefs path, the "PathSeparator" got used instead of "DirectorySeparatorChar". Resulting in a "wrong" file path.

Internal:
- [Intern-2025-1] Corrected and optimized some tests.

## 2025.1: Official release of AntiCheat 2025

We are excited to announce the official release of **AntiCheat 2025.1**, packed with new features and improvements to enhance your security and development experience!

### Key Highlights:

- **Unity 6 (6000) Support:** AntiCheat 2025 is now fully compatible with Unity 6, ensuring smooth integration with the latest Unity version.

- **Complete Rewrite of Protected PlayerPrefs:** We've completely rewritten the protected PlayerPrefs system, significantly boosting security and making it even more user-friendly. The new protected PlayerPrefs code now resides in the GUPS.AntiCheat.Protected.Storage.Prefs namespace.

*Important: This update is not compatible with the approach used in AntiCheat 2024 and earlier versions. We recommend using AntiCheat 2025+ for new projects or projects that have not yet been released. Else continue with AntiCheat 2024.*

### What's New in Protected PlayerPrefs:

1. Expanded Data Type Support:

AntiCheat 2025 now supports a broader range of data types, including:
- Byte, Byte Array, Boolean, Int16, Int32, Int64, UInt16, UInt32, UInt64
- Single, Double, Decimal, Char, String
- Color, Color32, Vector2, Vector2Int, Vector3, Vector3Int, Vector4
- Quaternion, Rect, Plane, Ray, Matrix4x4

2. New PlayerPrefs Settings:

Customize your PlayerPrefs protection with several new settings available in the Player Settings (Project Settings → GuardingPearSoftware → AntiCheat).

- Hash Key:
Option to hash the PlayerPrefs key (default or file-based). When enabled, the key is stored as a hashed version instead of the original name.
Recommended: True.

- Value Encryption Key:
Set a key to encrypt the PlayerPrefs value (default or file-based). If no key is set, the value remains unencrypted.
Note: Changing this key will make previously stored values unreadable.
Recommended: Yes.

- Allow Read Any Owner:
Enable this option to allow any user to read the stored PlayerPrefs (default or file-based), facilitating sharing between users. Disable it to restrict access to the device that originally created the PlayerPrefs, using the device's unique identifier (UnityEngine.SystemInfo.deviceUniqueIdentifier).
Recommended: Optional.

- Verify Integrity:
Enable verification of PlayerPrefs integrity to ensure data consistency. The integrity is checked using a hash calculated from the data type, value, and owner, and stored as a signature alongside the data.
Recommended: Optional.

This release brings enhanced security, flexibility, and broader compatibility to your projects. Make sure to explore the new features and settings to safeguard your game data with ease!

For optimal compatibility and security, we recommend using AntiCheat 2025 for new or unreleased projects.

## 2024.3.1: Protected Primitives - Fix
- Fix: Protected Primitives without any assigned values, threw a detection event on access (like .ToString()).

## 2024.3: Android Security Update
After two months of development, it's finally here: the Android Security update! Packed with numerous security features related to Android apps, it ensures your mobile gaming experience is safer than ever before.

Frequently mobile apps become targets for unauthorized redistribution, whether by disabling Digital Rights Management (DRM), modifying the app, bypassing payment methods, or rebranding it under a different name. This is a major problem for developers. That's why AntiCheat introduces new features that secure your mobile Android apps!

Android:
- Feature (Pro): Validate Installation Source - Validate the installation source, to check whether your app was installed by an official app stores and not by third parties.
- Feature (Pro): Validate Hash - Validate the entire app hash to determine whether the app has been modified in any way. Be it a different package name or changed code or other resources. 
- Feature (Pro): Validate Certificate Fingerprint - Validate your apps certificate fingerprint to make sure the app is shipped by you and no one else.
- Feature (Pro): Validate Libraries - A common cheat method in Unity Android apps is to insert custom libraries into your app instead of modifying the existing code. Validate against whitelisted and blacklisted libraries.
- Feature (Pro): Validate Installed Apps - Not only can a user modify or manipulate your game or app, but they can also try to gain an advantage by making changes to their device. Validate the installed apps!

iOS + Android:
- Feature: Validate Package Name - Validate the package name of the shipped app and make sure it is your app and not a rebranding.

## 2024.2.2: Protected Player Prefs - Fix
- Fix: Protected Player Prefs had an issue throwing detected cheating when reading from Protected Player Prefs.

## 2024.2.1: Support of Unity 2019 & 2020
- Support: Supports now Unity 2019 and 2020.
- Feature (Pro): Blockchain: A blockchain class has been introduced that inherits from a datachain that can be synchronized with a remote source.

## 2024.2: Datachain & Collection - Update
- QoL: Unity events can now also be attached to detectors via the inspector in order to react to cheating events.
- Feature (Pro): ProtectedList - A protected list is similar to the normal generic list you would use, with the special feature that its integrity is checked.
- Feature (Pro): ProtectedQueue - A protected queue is similar to the normal generic queue you would use, with the special feature that its integrity is checked.
- Feature (Pro): ProtectedStack - A protected stack is similar to the normal generic stack you would use, with the special feature that its integrity is checked.
- Feature (Pro): Datachain - A datachain is similar to a linked list consisting of a sequence of elements arranged in a specific order. It is used to maintain the order of these elements while keeping its integrity.

## 2024.1: Official release of AntiCheat 2024
- Refactored the code base to tackle the security issues in gaming 2024!
