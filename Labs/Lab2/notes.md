# Lab 2 – Checkpoint Questions

## 1.1 Grid Basics

### 1. In your own words, what is a node and what is an edge in this grid?
Describe what a grid tile represents as a node, and what connections between tiles represent as edges.

**Answer:**  
---
- In this scenario, the node is the tile, and the edge is boolean that determines whether or not the tile is walkable.
- The grid tiles each represent a specific walkable or unwalkable area. Based on the neighbouring relationship between the tiles (tile direction or octagonal direction), the boolean ``` walkable ``` represents the edge between the tiles.

### 2. How does the grid coordinate `(x, y)` map to world position `(x * cellSize, z * cellSize)`?
Explain the relationship between the grid indices and Unity world positions.

**Answer:**  
---
- Since we iterate over the grids rows and columns using ``` new Vector3(x * cellSize, 0, z * cellSize) ``` will give us a world position that is easily translatable to a 2-dimensional grid. For example, if we are currently iterating over (column 3, row 4), we will receive a world position equivalent to ``` new Vector3(3, 0, 4) ```.This makes it easy for us to instantiate the tiles in a grid-like fashion.

### 3. What happens if you try to access `nodes[x, y]` with coordinates outside the array bounds?
How can you prevent errors when accessing nodes?

**Answer:**  
---
- Since we're using an if-statement to determine whether or not the specified index is within the ranges of the grid's width and height, I would say that we are properly preventing attempts to access nodes that are out of bounds. If we attempt such a thing, we will receive a null value, which I think is relevant to the function.

## 1.2 Walkability & Toggling Tiles

### 4. What Unity function do you use to convert a screen position to a 3D ray?
Explain how raycasting detects which tile is clicked.

**Answer:**  
--- The function we're using is ``` ScreenPointToRay(Vector3 pos) ```, a helper function part of the Camera class. The full line of code is:
``` Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue()); ```
Here we are using the ``` Vector2 ``` return value from the mouse position, which gets implicitly converted to a ```Vector3```, which we're then passing as argument to the ```ScreenPointToRay``` function, which in turn returns a ```Ray``` object. The ray has it's origin in the near plane of the camera, and goes through the (x, y) pixel coordinates determined by the mouse click.

### 5. Why is it useful to visualize walkable vs non-walkable tiles?
What does this help you understand when debugging the grid or pathfinding?

**Answer:**  
---
- Visual representation of logic is always helpful as it gives a different perspective of the actual outcome. Visualizing a grid of say, 150x150, and trying to step through the entire algorithmic calculation which led you to a specific pixel only using your mind's inner eye can be quite taxing, and I would say you'd have to be a wonder child in order to do that properly. Different tools for debugging helps us as developers to narrow down problems, and different types of debugger tools helps us engage more of our brain capacity. In this case, a visual aid is very helpful in mapping out the algorithmic process.

### 6. In graph terms, what does toggling a tile to a wall represent?
How does changing a tile’s walkability affect the node and its edges?

**Answer:**  
---
- Toggling a tile represents the removal and addition of edges between a set of nodes. A set of tiles are considered walkable if an edge exists between them, which in this case is represented by the boolean member ```walkable```.

## 1.3 A* Pathfinding

### 7. In your code, where do you compute `g(n)`, `h(n)`, and `f(n)`?
Identify where in the algorithm each of these values is updated.

**Answer:**  
---
- We update these values in a number of places;
  - In the Node constructor, we set the default values for hCost and gCost.
  - The hCost and gCost values of each node gets reset in the beginning of the FindPath function.
  - Then we set the gCost to 0 for the starting node (since it takes 0 steps to move to the location where the object is currently at)
  - Then we set the heuristic (hCost) value to the sum of the absolute values of the start and target node components respectively.
  - As we iterate over the neighbouring nodes, we increment their gCost by the current nodes gCost + 1.

### 8. Why do we check `tentativeG < neighbour.gCost`?
Explain why this comparison is necessary when exploring neighbours.

**Answer:**  
---
- We compare the gCosts because we might've found a better (cheaper) route than the one we previously were on. If this new route is cheaper than the previous one, then this would have a direct impact on the gCost of the node, since the gCost represents the step cost from our starting node to the currently selected node.

### 9. What happens if your heuristic overestimates the true distance?
How does it affect the correctness of the path found by A*?

**Answer:**  
---
- If the heuristic cost overestimates the true distance, then the heuristic is consider inadmissable. if we take this key calculation into consideration: ´´´ f(n) = g(n) + h(n) ```, and h(n) is an overestimate, then the algorithm might skip exploring a node that is actually the conceptually correct one, because the algorithm may think that the node is more expensive than it is. This can lead to a sub-optimal or simply incorrect path calculation.

### 10. If you set `h(n) = 0` for all nodes, what classic algorithm does A* become?

**Answer:**  
---
- Dijkstra's algorithm: expands search based on the cheapeast travelled distance from start.

### 11. If you ignore `g(n)` completely and only sort by `h(n)`, what behavior do you expect to see?

**Answer:**  
---
- Greedy first-best search: Expands search based on the node that appears to be closest to the target.

## 1.4 Agent Movement

### 12. What is the difference between computing a path and moving along a path?

**Answer:**  
---
- Computing a path is equivalent to searching through a graph in order to find out the sequence of nodes that connects one node to another.
- Moving along a path is the execution of updating the position of an object, following the sequence of nodes determined by the calculated path.

### 13. What happens in your movement code if there is no valid path (FindPath returns null)?
How should this case be handled?

**Answer:**  
---
- Nothing happens. Currently the code returns "null" if a path isn't found. We could use this result to trigger some other kind of behaviour, such as an investigation state which would be defined as part of a finite state machine. Ultimately how this case is handled is up to what kind of behaviour you as the author would like to see. A default state could be to return to a know patrol point, and then execute a patrol sequence from there.

## 1.5 Dynamic Targets

### 14. If the goal (player) can move, how often should you recompute the path?
What trade-offs appear?

**Answer:**  
---
- There is no fixed rule in regards to how often you *should* recompute. More frequent recomputes would require more computational power. More frequent recomputes could mean that your AI appears to be more responsive, at the cost of computation power. Put the other way around, less frequent updates *could* mean less responsive AI behaviour. Or, to be more specific - The AI would by defintion be less responsive, but if this reduction in responsiveness is actually noticable to the player is an entirely different thing.

### 15. What happens if the player moves into a location that is currently unreachable due to walls?
How could you detect and handle that?

**Answer:**  
---
- This scenario would be caught by the fact that unless we find a valid sequence of nodes from the start node to the target node (player) we return a null value. What happens next depends on what you want to happen in this scenario. You could check if the NPC still has vision of the target, even if it can't reach it. If so, the NPC could be triggered to start using ranged attacks for example.

## 1.6 Reflection & Theory

### 16. How is your grid + A* system conceptually similar to Unity’s NavMesh + NavMeshAgent from Lab 1?
How is it different?

**Answer:**  
---
- It is similar in the way that Unity's NavMesh uses A* under the hood for pathfinding. In the previous lab, the pathfinding logic is mostly abstracted away due to the usage of Unity's NavMesh. Similarly, it is different in that we in this lab implement the graph system and pathfinding ourselves. We also move the agent ourselves, which is something that was otherwise abstracted away using the NavMeshAgents ``` SetDestination() ``` function. In the last lab, we also made use of a finite state machine, depending on the status of the NavMeshAgent. In this lab, we are not necessarily implementing any defined structure for an FSM.

### 17. In which situations would you prefer:
- A NavMesh with built-in pathfinding?  
- A custom grid-based A* like this?

**Answer:**  
---
- The built-in NavMesh is beneficial if you are pressed for time and dont need granular control. NavMesh abstracts a lot of things away, such as pathfinding, movement, acceleration/deceleration. It also tends to work pretty well with uneven terrain such as slopes, caves, etc.
- A custom grid-based A* gives you more control of the graph and interactions that affect it. You're also able to update parts or all of the grid-based system during runtime, while NavMesh requires baking, which isn't something you can do efficiently during runtime.

### 18. Imagine a large RTS map with hundreds of units:
- What performance issues might appear with naive A* usage?  
- What strategies could reduce pathfinding cost?

**Answer:**  
---
- Simultaneous A* calcs for a large number of units would be costly. You could implement things like path-caching or "follow-leader" types of calcs where a group of units follow the path of a leader unity which has performed the A* calc, and thus reducing the number of similar A* cals being performed. 

### 19. How does diagonal movement (8-neighbour) change the shape of paths?
How do you ensure the heuristic remains admissible?

**Answer:**  
---
- Using cardinal movement (4-neighbour) gives an L-shaped path. Diagonal gives, as the name suggests, the option for a diagonal path, and is no longer constrainer to the L-shaped path.
- In order to account for an 8-neighbour situation, you should take into account the diagonal step cost. Generally, diagonal step costs are considered to be 1.4, whereas cardinal movement is generally considered to be 1. The difference in calculating the travel cost can be described as the following: ```(√2 × step size instead of 1 × step size)```.

### 20. If you introduce different terrain costs (road, mud, water), how should the algorithm handle this?
How could you encode danger (e.g., enemy vision tiles) as a cost?

**Answer:**  
---
- This is how we currently calculate the gCost: ```float costToNeighbour = current.gCost + 1;```. To take into account an additional terrain cost, we could do something like this instead: ```float costToNeighbour = current.gCost + neighbour.terrainCost;```. If we wanted to account for danger, then 'terrainCost' would be replaced with an appropriate danger field. 


### 21. How does handling multiple agents affect pathfinding and movement?
How could you implement simple avoidance?

**Answer:**  
---
- Given a naive A* implementation, two different units could try to occupy the same space, leading to path conflicts and undesired behaviour. This can be mitigated by for example marking an occupied tile as, well, just that - occupied. If a tile is occupied, we could consider this node temporarily disconnected from the graph and thus removing it as a potential node for the other NPCS, until it gets resolved and once again becomes available.
