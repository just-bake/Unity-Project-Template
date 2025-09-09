# Demo - Protect Android

Mobile apps become frequently targets for unauthorized redistribution, whether by disabling Digital Rights Management (DRM), modifying the app, bypassing payment methods, or rebranding it under a different name. This is a major problem for developers. That's why AntiCheat introduces features that secure your mobile apps (Android and iOS) and detect if they have been compromised or altered in any way.

# Scene

In this demo you will find several scenes, but the starting scene is the 'Title' scene, which you can find under 'Scenes/Title'.

This is a simple jump-and-run mobile ready demo featuring multiple levels where the player collects coins to earn points. Each coin awards 100 points. If the player touches a spike or a yellow enemy, they lose a life and must restart the level. Reaching the goal flag allows them to progress to the next level.

The demo contains the 'AntiCheat-Monitor' with built-in Android package and device monitoring and detectoring. Both can be located at the corresponding child GameObjects 'Android Package Cheating Detector' and 'Android Device Cheating Detector'. This setup show how to use the current Android security features offered by AntiCheat:
- Validate Installation Source: Validate the installation source, to check whether your app was installed by an official app stores and not by third parties.

- Validate Hash: Validate the entire app hash to determine whether the app has been modified in any way. Be it a different package name or changed code or other resources. 

- Validate Certificate Fingerprint: Validate your apps certificate fingerprint to make sure the app is shipped by you and no one else.

- Validate Libraries: A common cheat method in Unity Android apps is to insert custom libraries into your app instead of modifying the existing code. Validate against whitelisted and blacklisted libraries.

- Validate Installed Apps: Not only can a user modify or manipulate your game or app, but they can also try to gain an advantage by making changes to their device. Validate the installed apps!

To demonstrate how to recognize and react to mobile tampering, the demo itself reacts to 3 different types of Android cheating:
1. If the player has any type of unauthorized apps installed on their device, the coin and therefore points collection will be deactivated.

2. If the app does not have the expected hash value, the winning will be disabled for the player. This validation will fail every time unless you set up a server that returns the correct hash code for the running app. The hash validation will attempt to reach the server within a timeout window of 30 seconds. If this is not successful or an incorrect hash is returned, the winning is deactivated for the player.

3. If the app certificate fingerprint is not valid and does not match the expected fingerprint, the player's ability to double jump is deactivated. This makes some levels more difficult or an event cannot be completed.

# Legal

The demo uses parts of the github repository: https://github.com/angelotadres/RunAndJump

The repository is under the following license:

The MIT License (MIT)

Copyright (c) 2015 Angelo Tadres

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Also the repository credits the following artists:

Part of the art and sounds used on this demo are from Bevouliin (https://opengameart.org/users/bevouliin) and Kenney (https://opengameart.org/users/kenney).