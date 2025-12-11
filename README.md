# A Lab Assignment 01
## Overview
The purpose of this README is primarily to document my progress related to the assignment **"A Simple FSM**, including my answers to the given checkpoint questions which are part of the assignment.

## Research ##
1. In your own words, what is a state in game AI? Give two examples.
A state in game AI is the representation of some flow of logic or behaviour that exists within a specific boundary.
### Examples ###
- A player flying through the air.
- A player ducking under a fallen log.
3. What triggers a transition between states? Give two examples.
- What triggers a transition is up to the author. The trigger can be virtually anything. It's up to you to define the ruleset where each trigger is responsible for a different outcome.
### Examples ###
- While an enemy AI has a player in field of view and is within range, the enemy AI will chase and attempt to attack the player. In this scenario, there are two conditions: the field of view, and the range.
- While a player is considered to be falling (state), an falling animation might play. When the player hits the ground, the falling animation transitions to one where the player is standing (state). In this the scenario, the transition could be triggered based on a boolean value that indicates whether the player is falling or not.
4. Why do you think game AI often uses simple techniques (FSMs) instead of “real AI” like deep learning?
- Because "real AI" is incredibly costly and complex, requiring huge amounts of computational power in order to perform their service. As far as I understand, the goal game AI is to create a believable appearance of "smart" AI capabilities, whether or not the "smartness" actually exists. I would dare guess that "real AI" has to take into account the possibility for a more extensive range of possibilites, while games would have an easier time to limit the potential interactions and thus expectations from the player. This would remove the need for creating overly complex and versatile AI. Also, simple techniques often have the benefit of being maintanable, which is very positive in a development context. 

## Q&A
**Q: What coordinate axes respond to “forward” and “up” in Unity?**

**A:** 
- "up" relates to the Y-coordinate in 3D world space. It's considered a shorthand for *Vector3(0, 1, 0)*. In 2D space, "up" still relates to the Y-coordinate, but is considered shorthand for *Vector2(0,1)*.
- "forward" relates the Z-coordinate in 3D world space. It's considered a shorthand for *Vector3(0, 0, 1)*.

**Q:What happens if the Guard has a NavMeshAgent but there is no baked NavMesh?**

**A:**
- A warning is thrown, indicating that the agent failed during creation. This can happen if the object which the NavAgent is attached too is too far away from or otherwise can't find an available NavMesh. In the above scenario, since no NavMesh has been baked, there is no available NavMesh data so the agent wont be able to find a suitable NavMesh to latch on to.

**Q: In your own words, what is the difference between a path and the movement along that path?**
- I would define a path as the definition of a starting point and an endpoint, with a sequential distance between the two points determined by the A* algorithm, selecting the closes intersectional points of the connected polygons. I would define movement along that path as the objects positional update from the starting position towards the end point, determined by that calculated path, until the end points has been reached.

**Q: Why do we check !_agent.pathfinding before reading remainingDistance?**

**A:**
- An accurate representation of the remainingDistance relies on the agent having properly processed the path, and thus, we want to return if the agent is still processing the path in order to not receive undefined behaviour.

**Q: What happens if we forget to use % waypoints.Length when incrementing the index?**

**A:**
- We would eventually try to access the array with an out of range index, and an error will be thrown.

## FSM Diagram ##
<img width="330" height="471" alt="image" src="https://github.com/user-attachments/assets/5d134577-ee33-4ace-8c65-6cd5cbd18936" />

**Q: What would the code look like if you did not have an enum and tried to manage states with only booleans like *isPatrolling*, *isChasing*? Why might that be harded to maintain?**

**A:** It would require the consistent addition of state checks in every if statement related to a state transition. If I add any kind of state as a feature, then each pre-existing state logic would have to implement a check to see whether or not this new state is active or not, depending on what kind of state transition overlap you want. This would be incredibly cumbersome, and error prone, as you would have to edit code in an increasing multitude of places. Each feature implementation would have the undeninable side effect of introducing bugs. It also wouldn't be very maintanable, depending on the scale of the program.

### Example ###
Let's say that we can only chase if we are not patrolling. That's fine. But, if we introduce the state "ReturnToPatrol", then we would most likely have to introduce a check to gauge the current state of this "ReturnToPatrol" state before we can initiate our chase. Likewise, the "ReturnToPatrol" state would potentially have to check if we're either already patrolling, or if we are chasing. And this type of state-checking would increase for every state we introduce.

**Q: Briefly exlain your FSM states and transitions**

**A:**

The FSM states are the following;
- *Patroling* - The enemy patrols between a number of checkpoints assigned via the Unity editor. It does so sequentially (no randomness implemented). Upon reaching the last checkpoint, a modulus operator is used to loop back to the first checkpoint. 
- *Chasing* - When the enemy spots a player inside it's field of view, or if the player enters the enemies aggro radius, it updates its "SetDestination()" call to reference the player position. 
- *Investigating* - If the enemy loses sight of the player, it moves to the last known location of the player. After a 5 second delay (editable via editor), it returns to it's previous checkpoint patrol destination. 
- *ReturnToPatrol* - If the player runs out of aggro range, the enemy abandons chase and returns to it's last known checkpoint destination. 

**Q: Point to the part of your code where the state change happens.**

**A:**
- *Patroling* - ```    public GuardState currentState = GuardState.Patrolling; ```
- *Chasing* - ``` if (distance <= aggroRadius) { currentState = GuardState.Chasing; } if (CanSeePlayer()) currentState = GuardState.Chasing;   ```
  
- *Investigating* -  ```
  if (!CanSeePlayer())
        {
            if (dropAggroCount >= dropAggroTime)
            {
                currentState = GuardState.Investigate;
                dropAggroCount = 0f;
            }
            dropAggroCount += Time.deltaTime;
        }
  ```
- *ReturnToPatrol* - ```         if (distanceToLastKnown < 0.5f)
        {
            investigationCounter += Time.deltaTime;

            if (investigationCounter >= InvestigationTime)
            {
                investigationCounter = 0f;
                currentState = GuardState.ReturningToPatrol;
            }
        }   ```


