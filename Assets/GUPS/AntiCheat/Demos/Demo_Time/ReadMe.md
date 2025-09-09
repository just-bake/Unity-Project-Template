# Demo - Protected Time

In Unity, frame-based calculations rely heavily on UnityEngine.Time.deltaTime. Unfortunately, this time calculation is vulnerable to manipulation by cheat or hacking tools that allow unauthorised acceleration, deceleration or interruption of your application. To minimise such risks, the implementation of AntiCheat ProtectedTime is essential.

# Scene

In this demo you will find a scene. In this scene there is a cube that rotates around its center using the deltaTime. The deltaTime is a frequent target of attack, calculated using the system ticks, to speed up or slow down the game. To protect against such cheating, the demo showcases the use of ProtectedTime, which detects cheating attempts and calculates the correct deltaTime and everything UnityEngine.Time offers.