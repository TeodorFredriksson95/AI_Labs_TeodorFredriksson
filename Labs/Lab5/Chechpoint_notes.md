# GOAP – Warm-Up Questions

## 4.1 History & Positioning
### 1. In one sentence: what problem does GOAP solve that BTs often don’t?

**Answer:**  
---
- Scenarios where dynamic decision-making is required as opposed to hierarchal, scripted decisions.

### 2. Why is GOAP usually described as “STRIPS-like planning adapted for games”?  
(STRIPS: *A New Approach to the Application of Theorem Proving to Problem Solving*)

**Answer:**  
---
- I would assume that the key takeaway here is that STRIPS for games, as oppoosed to AI used in robotics, is partially that the games-implemented version of STRIPS doesn't require as rigid of a world & problem description and relations in the form of facts as robotics AI implementations do. 

### 3. Give one example where a Behaviour Tree is a better fit than GOAP,  
and one where GOAP is a better fit than a Behaviour Tree.

**Answer:**  
---
- Any situation where a sequence of actions can be statically designed lends itself to be more suited for BT's than GOAP. BTs are more "reactive" to conditions, while GOAP impements search mechanisms in order to find a sequence of actions that satisfy the requirements needed to reach a desired state. If I can map out a "if this, then that" scenario, then a BT is probably better suited. That is, if the scale of the planning is at a level that justifies not using FSM, in which case, FSM might be better suited. Also, it could be argued that GOAP sequences also follow a pattern of "if this, then that", however, in that scenario, the occurence that defined the "if" case and decision-making that defined the "then that" case are dynamic. The reasoning behind the decision-making can be explained in similar ways, but the sequence of root causes to whatever performed action in a GOAP depends on an ever changing state of the world.

- *Example 1:* I have an NPC that wants to enter a building. He begins by checking all the available doors. They are all locked, which means that the NPC will take an alternative approach. We, as authors, have stated that the next attempt will be to check all the windows for a possible entry. This usecase lends itself to be suited for a growing BT implementation.

- *Example 2:* We have an enemy NPC with two re-occuring, alternating goals: kill the player, and stay alive. During our simulation of this world space where both NPC and player exist, the player might actively try to kill the NPC. Depending on what conditions we have set for each action related to the goal in question, one goal might appear to be more urgent to fulfill than the other. In case of the NPC taking critical damage, the goal "Stay Alive" might be higher prioritzed. With this goal in mind, the decision planner will use some sort of search mechanism (usually A*) in order to determine the currently best course of action in order to reach the goal "Stay Alive", which might be to run for cover while the player is continuously shooting at it.

It may also be to enter a state of berserk and enter melee combant with the player. The likelyhood and outcome of each action is steered by the designers hand, in that they set the cost, effects, and conditions required to be met before an action can be taken. Whether or not an action *is* the best course of action can be highly subjective.

## 4.2 Let’s Make It Concrete: GOAP Ingredients

### 4. Define these in your own words:  
- World State  
- Goal  
- Action  
- Plan  
- Cost  

**Answer:**  
---
- World State: A set of values the determine the agents perspective of the world.
- Goal: A desired world state that the agent wishes to create.
- Action: A step taken by the agent, defined by its preconditions (conditions that must be true in order for the action to run), and its effects on the world state.
- Plan: A dynamically generated solution to a problem based on current world state with the objective of reaching a goal state.

### 5. What’s the difference between:  
- An action’s **declarative preconditions/effects** (planner-visible), and  
- A **procedural check** (runtime-only, e.g., raycast LOS)?

**Answer:**  
---
- Declarative: Preconditions, effects and facts that the planner knows about and is taken into account during decision-making.
- Procedural: Runtime checks that affect whether or not an action is actually able to execute. These results are not part of the planner's model, but still has an impact on the execution of an action.

## 4.3 GOAP as Search (Connect to Lab 2)

### 6. In A*, we had:  
- Nodes = positions  
- Edges = movement steps  

In GOAP planning, what are **nodes** and **edges**?

**Answer:**  
---
- Nodes: The different states of the world.
- Edges: Actions taken.

### 7. Why do action costs matter?  
Give an example where two plans achieve the goal but you prefer one.

**Answer:**  
---
- Given a scenario where two different actions exist that both satisfy the desired world state, and each action has their preconditions fulfilled, an action cost lets the agent determine which action to perform. Let's say an agent has the goal "Kill player". It has two actions readily available: "Kick Player", and "Choke Player". The precondition for both actions is to be within appropriate range, which we will say that they are. For some reason though, this agent is wearing incredibly heavy boots, which ultimately increases the cost of kicking. Given this scenario, with no further preconditions or world states ,the agent will likely opt for a "Choke" action, because it is considered to be cheaper than the "Kick" action. This is related to the search mechanisms often used. One key factor of search algorithms is that they try to find the cheapest path to a desired goal, which we can see represented in this example. 

## 4.4 Failure & Replanning (The “Real Game” Part)

### 8. Name three reasons a valid plan might fail during execution  
(even if it was valid when planned).

**Answer:**  
---
- The desired goal state may change at any time, rendering the execution of a previously planned and desired action unwanted.
- The action might be exposed to some kind of randomness which causes execution to fail.
- The state of the world might have changed since the plan for the action was made.

### 9. What should the agent do when a plan fails:  
always replan instantly, or sometimes continue? Why?

**Answer:**  
---
- For optimization reasons, instant replanning may not be desired, but rather be throttled.
- Some actions make sense to be repeated, even if the desired goal was not met. Given an attempt to kick down a locked door, the attempted breach may not succed on first try. There is no garuantee that the breach succeeds on the second attempt, but it would be realistic to propose that it *might*. In this scenario, it could make sense to try a certain amount of kicks before making a new plan. 

## 4.5 Integration with the AI Stack

### 10. Where should each of these live in a robust design?  
- Perception / sensing  
- World state update  
- Planning  
- Action execution  
- Movement (NavMesh / A* / steering)

**Answer:**  
---
- I am a little bit confused by the questioning. It would make sense for each of these categories to live in separated modules, making use of composition when required. Planning shouldn't necessarily be tied to all executions, and movement shouldn't be tied to all planning. World state could be updated from anywhere, given the relationship between a particular state and occurence. Perception would also most likely be a separate component, part of a bigger architecture.

# 5. Small Pen-and-Paper Exercises  
(10–20 minutes, recommended)

## Exercise A: Design a Mini GOAP Domain

**Scenario:**  
“Guard wants to be safe while engaging the player.”

### Define:

- 6–10 world-state facts (bools are fine), e.g.:  
  - HasAmmo  
  - InCover  
  - SeesPlayer  
  - HasGrenade  
  - IsHurt  
  - AtCoverSpot  

**Answer:**  
---
- SeesPlayer
- HasInterruptReady
- IsChasing
- IsPatrolling
- SeesCompanion

### Define 5–8 actions with:  
- Preconditions  
- Effects  
- Cost  

Example actions:  
- TakeCover  
- Reload  
- Shoot  
- ThrowGrenade  
- MoveToCover  
- FindCoverSpot  

**Answer:**  
---
- FindPlayer
- MoveToPlayer
- ChargePlayer
- FindCompanion
- MoveToCompanion
- CoverCompanion

### Now answer:  
What plan do you expect when:  
- HasAmmo = false  
- SeesPlayer = true  
- InCover = false  
- CoverSpotKnown = true  

**Answer:**  
---
- I would expect the action "ThrowGrenade" to be executed.

## Exercise B: Spot the “Lying Action” Problem

### Create an action that claims an effect that isn’t always true  
(e.g., `Shoot ⇒ PlayerDead`).

**Answer:**  
---
- `ChargePlayer => PlayerIncapacited`

### What bug does this cause in planners?  
What’s a better modeling approach?

**Answer:**  
---
- It assumes state without verification. In the "Shoot" scenario, it assumes the player is dead, which could lead to the agent following a new plan which is based on the potentially incorrect belief that the player is dead. It could also lead to the agent being able to perform actions that require that the player is dead, which might get very weird. A better approach would be to only model garuanteed effects and use a verification process to determine whether or not a potentially unsuccessful action was actually performed before updating state.
