# Behaviour Trees – Preparation

## 4.1 “Why BTs?”
### 1. What scaling problem do you hit (or can imagine hitting) with an FSM once you move from ~3 states to ~15 behaviours?

**Answer:**  
---
- There would be a lot of nested if/switch statements. At face value FSM reduces the amount of flags we have to set, but if they grow too large I would assume we instead would have to create a lot of nested switch stamements within each state. I also don't think it'd work that well with branching and conditional logic that lives on different depth levels.

### 2. Why is a BT easier to extend without rewriting everything than a big FSM?

**Answer:**  
---
- Because the nodes in and of themselves are very modular. A BT incorporates more high-level control flow at a design level where it'd be pretty easy to hook up a primitive node into an otherwise over complex structure. One big benefit of BTs is the fact that you can create a dynamic, modular and increasingly complex structure using basic building blocks.

## 4.2 Execution model (the mechanics that make BTs work)
### 3. What does **Running** mean in gameplay terms?

**Answer:**  
---
- **Running* is one of the 3 main states (commonly, Unreal features 2 additional states) that a node can return. If a node returns **Running*, it means this node will be visited again on the next BT tick. Meanwhile, a return state equivalent to either 'Success' or 'Failure' means that these nodes will not be visited again until their status is reset.

### 4. Give one concrete example of a node that would return **Running** for multiple ticks  
(e.g., `MoveTo(target)` or `Wait(2s)`).

**Answer:**  
---
- Well, let's look at `MoveTo(Target)`. We can assume that this function updates the object's position in a direction towards its target, supplied as a parameter. The object probably won't make it all the way to the target within a single BT tick, and thus, we would like to update our position until we do. So, in the instance that we don't run into some form of obstacle or find ourselves in a scenario that would make us want to deviate from our goal to move towards our target, we return the status code 'Running'. As mentioned above, this will cause the BT to contiously visit the node, update the object's position, until we either reach the target and return 'Success', or until we for some reason fail, in which case we return the status 'Failed'.

To build upon this, we could use something like an In-Parallel node which let's us execute some logic in parallel with the 'Running' node.  

### 5. Explain **Sequence vs Selector** using the example:  
“If I can see the player, chase. If not, patrol.”

**Answer:**  
---
- Sequence (Composite): A control flow node that executes it's children in order of priority, most commonly left to right. I say commonly, because Unreal (for example) allows you to number your nodes, which I assume means you have the ability to break this convention while still adhering to a prioritized flow of execution, albeit unconventional. A 'Sequence' is considered 'Failed' if any of it's children returns 'Failed'. The sequential node only returns a success if all it's children returns a success. In a programming context, the Sequence node could be described to be equivalent to the logical &&(AND) operator.

*Example:* Let's say we have a character who's on a treasure hunt. In order to gather the treasure, he must first spot it. This is our first status check. If our character fails to spot any treasure, well, then it would be very difficult for him to retrieve it. Thus, the sequential operation returns a 'Failed' status. However, let's say the character *does* spot a treasure. Then we run our next conditional check - he moves towards the chest. This node would likely continously return a 'Running' status until we reach the treasure's location. If we do, we finally return a 'Success' status. Now, the character's next task will be to *open* the chest. As he tries to open it, he notices that it's locked. Despite my poor choice of wording above (using the word 'task' in a node context) this node might be another Sequence node. This node runs two additional checks. First, it checks whether or not the character has lockpicking tools available. If he does, then we might checck if his lockpicking skill is high enough. In this scenario, we pass both tests, and thus, we return 'Success' to our parent (composite) Sequence node. We finally arrive at our last node, which is defined as a 'Leaf' node. Our Leaf node (sometimes called a 'Task' or 'Action' node) contains the logic that represents the character retrieving the contents of the chest, adding it to his inventory.

- Selector (Composite): Also a control flow node, but represents the logical ||(OR) operator. It executes it's children in a sequential order, left to right, but the 'Selector' node returns 'Success' if any of it's children returns a 'Success' status. This means that, for example, the first three children can return a 'Failed' status, but if the fourth, or n:th child returns 'Success', then the 'Selector' returns 'Success'. If all the selector's children fail, then so do the selector, unless there is some node logic that would modify it's value (such as an Inverter node).

## 4.3 Blackboard thinking

### 6. Propose 5 blackboard keys you’d want for a guard NPC.  
Include type (bool / float / vector / object).

**Answer:**  
---
- bool: IsPatrolling - If the guard is on patrol, then probably move towards the next waypoint.
- float: Speed - The guards current speed. Maybe the guard randomly accelerates/decelerates, and the patrol points change due to the current speed.
- vector: PlayerLocation - Have easy access to player's location.
- object: TargetActor - A reference to an object of importance.

### 7. Name one thing that should **not** be stored in the blackboard  
(something better kept inside an action node or component).

**Answer:**  
---
- Anything that is only relevant to a locally defined node scope should not live inside the BB. For example, we could have a locally defined counter that keeps track of the current amount of attemps to perform a task. This type of data should **not** live inisde the blackboard.

## 4.4 Connecting to earlier topics (AI stack)

### 8. In your own words, what belongs in:  
- The Behaviour Tree  
- Pathfinding (A* / NavMesh)  
- Steering / movement  

**Answer:**  
---
- Behaviour Tree: High-level design for logical flow of execution. It is the brain, or planner, that *decides* which action to perform. Conditional logic resulting in an action fits inside the BT. Logic that defines *what* to do.
- Pathfinding (A* / NavMesh): Calculations that assist in mapping out a graph consisting of nodes and edges to denote which nodes are connected to which. Logic that calculates a path between two (or more) objects. Logic that defines *where* to go.
- Steering / movement: Logic that updates an object's position in world space, and logic that affects how that movement is performed. Logic that defines *how* to get somewhere.

# 7. Vocabulary Checklist

You should be able to recognize and roughly explain:

- Tick  
- Success / Failure / Running  
- Selector / Sequence / Decorator  
- Action vs Condition  
- Blackboard key / variable

**Answer:**  
---
- Tick: Execution interval step within a Behaviour Tree.
- Success / Failure / Running: Potential process status codes returned on each tick.
- Selector / Sequence / Decorator: Different types of nodes wtihin a BT that define order and logical execution of nodes. A Selector / Sequeunce node is a control flow node, while a Decorator node is attached to a node in order to improve conditional decision-making.
- Action vs Condition: An Action contains execution logic that affects the state of the world, while a Condition is a prerequisite that is required to be met befor an action can be performed.
- Blackboard key / variable: Works like a Dictionary<TKey, TValue> data storage. The blackboard data can either be shared per blackboard instance, or it can be shared across multiple blackboards.
