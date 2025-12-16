# Reflection Questions – Lecture 3

## 10. Reflection Questions
Answer these with a few sentences each:

### 1. In your own words, what is the difference between:  
- A global path / waypoint list, and  
- A local steering behaviour like Seek or Arrive?

**Answer:**  
---
- A global path/waypoint list could be described as graph of nodes connected through edeges, used to determine the optimal path from point A to point B. There are several suitable algorithms for this, including A* (most commonly used in game dev), Dijkstra's algorithm and Greedy best-first search. Each algorithm has it's benefits and drawbacks, and individual analysis is required in order to determine the most efficient one for your specific use case. A summary for what a path is could be summed up as the definition of "where to go".
- Local steering behaviours like Seek or Arrive is the "how to move" part. It allows for emergent behaviour such as flocking or fleeing. Given obstacle along a defined path, a steering behaviour defines how the agent chooses to handle it's current path forward.

### 2. What visual difference did you notice between:  
- An agent using Seek, and  
- An agent using Arrive?

**Answer:**  
---
- Arrive has a smoother trajectory, featuring acceleration and deceleration. One big difference is the tendency of Seek to overshoot it's destination, however I feel like Arrive had similar behaviour without additional tweaking to handle stop distances and deceleration.

### 3. How did Separation change the behaviour of your group?  
- What happens if you set separationStrength very low? Very high?

**Answer:**  
---
- Separation enabled a larger spatial spread between the active agents. Separation enabled the feeling of actually getting swarmed as part of the emergent behaviour based on Reynold Craig's mathematical formula for separation.
- Based on the value of separationStrength you get a larger or smaller force multipliere, directly impacting how far the agents are pushed away from each other relative to how close they are at the time of separation calculation.

### 4. (If you tried flocking or path integration)  
What new behaviours did you see when you:  
- Added Cohesion / Alignment, or  
- Combined steering with NavMesh / A* paths?

**Answer:**  
---
- Based on my development scene, the nuances were subtle, but they *did* give make it feel as if the agents shared a common purpose without behaving robotic. As the agents pursued a target, they would somewhat group up as they were approaching the target, which gave the impression of the agents acting on a subconcious "safety in numbers" instinct, while still allowing for "individual" behaviour if the individual was outside of a local flocking neighbourhood.
- Combining steering with NavMesh's A* implementation did provide a more natural feeling when it came to path traversal. Whereas my previous "static" movement logic provided a more mechanical movement pattern, the mentioned combination felt more human, or rather, more reactive in relation to general surroundings and behaviours.

### 5. Looking ahead to your final project:  
- Name at least one NPC, enemy, or unit that could use this SteeringAgent.  
- How might you combine steering with your FSM or pathfinding in that project?

**Answer:**  
---
- Both (Rolling Ball, and NPC Helper). I plan on utilizing dynamic patrol checkpoints and obstacles and want to create feelings such as "quirky, reactive, panicked, decisive". I plan on utilizing Behaviour Trees, and based on certain preconditions different types of steering behaviours would make sense. For instance, if the *Rolling Ball* detects the player, then the "Flee" steering behaviour will be utilized. For the NPC Helper, given that a positional threshold between the Rolling Ball and the Helper has been crossed, it would be interesting to implement a Cohesion behaviour in order to give the NPC a sense of desire to "follow and protect". 
