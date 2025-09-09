# Demo - Protected DataChain

A datachain is similar to a linked list consisting of a sequence of elements arranged in a specific order. It is used to maintain the order of these elements while keeping its integrity. And could be useful for example for the following use cases:

- Digital assets: Store in-game assets such as weapons, skins or cosmetics so that players have real ownership rights and secure trading opportunities.
- Achievements: Ensure the immutability and transparency of achievements, increasing trust and credibility within the community.
- Virtual currency: Use a datachain to manage game currencies to ensure transparency and protect against cheating or manipulation.

# Scene

In this demo you will find one scene. It is a simple jumping game where you have to catch coins to get a score. When you catch a coin, you get 10 points.

The demo shows the use of a datachain to save the player's score. Each time a coin is caught by the player, the points are appended as a new element in the chain. Why not just use an integer value and add them up? Because a data chain offers the possibility to store the order of the inserted data. Maybe you don't just want to add simple int values, but whole objects with different coin types, which can later be displayed in a nice graph showing when the player has caught which coin.

The data chain checks its integrity every time a new element is added or read. If an integrity problem is detected, the primitive cheating detector is notified. It should also be noted the datachain only allow data that is stored as a value (structs) and not as a reference (classes). This ensures that the list elements cannot be indirectly manipulated by you, which would lead to an accidental integrity problem, even if it is meant to be. However, you can of course also allow data by reference if you wish.

If a cheater tries to cheat in the demo, the manipulation is detected and the chain is reset. Also as punishment fake coins will be spawned reducing his score by 10 points.