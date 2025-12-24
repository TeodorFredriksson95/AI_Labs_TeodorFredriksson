# Behaviour Tree – Reflection Questions

## Reflection Questions

### 1. Compare FSM vs Behaviour Tree for this guard.  
What became easier? What became harder?

**Answer:**  
---
The hardest part is to initially understand the flow of actions via control nodes. The behavior graph in Unity does however give a much greater overview of the of the flow, once the flow/execution of nodes are more fully understood. It does feel more extensible, however experience tells me (accidentally did the project before this lab, unfortunately) that it can get messy if your architecture is not on point. A huge plus is that the behavior graph is an extremely efficient tool. It would've taken me *a lot* longer to write the code for the same kind of perception --> planning --> action flow.

### 2. The `Navigate To Target` node returns **Running** while the guard is moving.  
What would happen if it returned **Success** immediately instead?  
Describe the visual behaviour you’d see.

**Answer:**  
---
That node would not be re-entered, meaning that the guard would come to a halt, as the navigation is updated each frame, or rather, each time the node gets visited (as long as it returns **Running**). Currently the way that it's set up, means that the "Try In Order" control node will first attempt to visit the "Patrol" branch. However, we have set a conditional flag of "HasLineOfSight", which is not met. Thus, we since we're using a selector node, we move on to the next-in-order branch, where the `Navigate To Target` action lives. Since we are returning 'Running' until the action either succeeds by reaching the target, or fails due to some other reason (world event, gets killed, other priorities interfere), we will continue executing the navigation thanks to the 'Running' state being returned.

### 3. What did you put in the blackboard, and why does it belong there  
(vs inside a single node)?

**Answer:**  
---
The variables that currently live inside the blackboard are mostly related to the perception of the agent. They do belong there, becuase in this "Chase/Patrol/Attack" kind of behavior that is defined for this agent, the variables related to perception are key in order to define how the agent sees the world, and what it reacts to. Putting these variables in the Blackboard gives the entire instanced graph access to data which is necessary when it comes to planning, and making decisions. You could say it's how different internalized behaviors communicate with each other.

### 4. If you were to scale this guard into a “real enemy,” what would you add next:  
- More sensors  
- More actions  
- More structure (subgraphs)  
- Better movement

Explain why.

**Answer:**  
---
- Evade timer
  - For example, what happens if the player is still within LoS, but unreachable? We would need to set a timer that checks, for example, if the agent is currently chasing the player, but cannot move (due to pathing not finding a suitable edge to move across)
- States representing different combat phases, such as "Enraged", "Fear" and "AlertOthers"
  - I would do this to increase the threat potential of a single enemy. Of course, this would be well suited to certain scenarios and enemy types.
- Randomized pathing and navigation during patrol. Right now, it's pretty static, following a sequence of A --> B --> C --> D -- > Back to A
  - More dynamic gameplay, harder to fully analyize a perceived static behavior.
