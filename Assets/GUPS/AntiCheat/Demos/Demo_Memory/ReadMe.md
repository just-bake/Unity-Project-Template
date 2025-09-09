# Demo - Protected Memory

Important data such as positions or health status are usually stored in the runtime memory, which can be vulnerable to manipulation by cheat tools or data sniffers. Encryption is essential to protect against unauthorised changes. The use of AntiCheats protected fields provides a straightforward solution that allows developers to protect their application's runtime data and prevent cheat attempts.

# Scene

In this demo you will find one scene. It's a simple arcade space shooter where you move from left to right shooting asteroids. Once an asteroid has been destroyed, you receive 10 points. If you are hit by an asteroid, you lose 10 points. 

This demo shows the use of protected memory by using a ProtectedInt32 field to store the player's score. This is because the score sounds like a target that a cheater would like to change to gain an advantage. Due to the protection, the score is encrypted and not visible to cheating/hacking tools. In addition, a honeypot is placed to attract and encrypt cheaters.