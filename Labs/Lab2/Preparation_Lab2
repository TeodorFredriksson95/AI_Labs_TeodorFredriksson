# Lecture 2 – Preparation Questions 

## Graph Basics

### 1. In your own words, what is a graph?
Describe what nodes and edges are, and give one game example (e.g., nodes = rooms, edges = doors).

**Answer:**  
-  Nodes(verticess) and edges(connections) are key members that together define the structure of a linear graph.
-  *Example:* A talent tree where the upgrades could be considered the nodes and the path between certain upgrades the edges.
---

### 2. What is the difference between a weighted and an unweighted graph?
Give an example of what the weight could represent in a game level.

**Answer:**  
-  A weighted graph means that the edges have some form of value connected to them which could represent distance, cost or time. 
-  In an unweighted graph treats all edges equally, with no added representational value attributed to them.
-  *Example 1:* In relevance to the answer above regarding talent trees, the weight could represent the amount of levels you would have to achieve in order to be able to fully upgrade an ability, represented by a node.
-  *Example 2:* In the instance of an automated transport path between to points, the weight could be considered either the time it takes to transport the object between these two points, or the fee required to purchase the service of transport - or, the cost could be considered the combination of both. This form of automated transport service could be examplified by World of Warcraft's flight path system. A more time-expensive path might be considered more beneficial due to it's reduction in monetary expense rather than a shorter travel path that costs more money. 

---

### 3. Representing a simple game level  
Think of a simple level from a platformer, dungeon, etc.

How could you represent that level as:

#### • A grid graph?
**Answer:**  
-  Given a 2D grid, each tile in the grid could be considered a node. Depending on whether or not any individual tile has a link(edge) to any other adjacent tile, these two tiles could be considered walkable. Omitting the edge from a tile in a certain direction would indicate that there is an obstacle in that direction. 

#### • A waypoint graph?
**Answer:**  
-  In a waypoint graph, the nodes usually represent key locations, and the path between them the edges. Waypoint graphs are commonly used in AI pathfinding, and could be used to specify a guards patrol route.
---

## 3.2 Shortest Paths and Cost

### 4. Grid with cost = 1 per move (up/down/left/right)
- If you go from **(0, 0) → (0, 5)** with no obstacles, what is the path cost?  
- If you go from **(0, 0) → (3, 4)** (no diagonals, no obstacles), what is the shortest possible path cost?

**Answer:**  
- Path cost (0,0 → 0,5):  The path cost is 5. We traverse 5 times on the y-axis, and 0 on the x-axis.
- Path cost (0,0 → 3,4):  The past is 7. We traverse 3 times in the x-axis, and 4 times on the y-axis.
---

### 5. Why might we want different movement costs on some edges?
Example: walking on a road vs walking through mud or water.

**Answer:**  
-  We could create different types of AI behaviour if we use a weighted graph. Based on the cost, we could even impose effects on the object, such as slowed or increased movement speed in relation to the cost.
---

## 3.3 A* Intuition

### 6. In the A* formula `f(n) = g(n) + h(n)`:
- What does **g(n)** represent in a game grid?  
- What does **h(n)** represent?  

**Answer:**  
- g(n): It represents the distance(cost) from the current node to the start node.
- h(n): It represents the estimated travel cost from the current node to the target node.
---

### 7. Enemy unit on a tile map
- If we only look at **g(n)** (actual distance from start), which classic algorithm does that resemble?  
- If we only look at **h(n)** (estimated distance to goal), what might go wrong?

**Answer:**  
- Only g(n): Dijktstra's algorithm. (link: https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm)
- Only h(n): Greedy best-first search. This will only perform a search based on the heuristic value, which may not result in the shortest path. (https://www.codecademy.com/resources/docs/ai/search-algorithms/greedy-best-first-search)

