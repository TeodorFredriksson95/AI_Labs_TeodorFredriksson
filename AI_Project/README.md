# AI Project Technical Report (Futuregames, 2025)

## 1. Game / Prototype Description

### Overview
The game consists of a small scene, featuring a player character and two NPC agents, named *'The Rolling Ball, aka "TRB"'* and *'Helper Agent, aka "H:A"*. The game objective of the game is to catch The Rolling Ball (TRB), while simultaneously avoiding the Helper Agent's (H:A) efforts to interrupt the players advancements. My ambition was to create a geometrically interesting scene which in turn would allow for more interesting agent navigation. It's worth to note that while the project does contain a gameplay loop, this project is focused around the behavior, movement and decision-making of the agents.

### Player Interaction
The player can run and jump in order to catch TRB and avoid H:A.

### AI Behaviour  
**TRB**
- Plans a path from it's current location to a desired location based on pre-defined waypoints, either at random or in order, depending on whether or not the agent feels threatened by the player's presence.
- Navigates between waypoints.
- Given frontal detection of the player, TRB flees to a random waypoint that is considered "safe" based on the player's current location and direction.
- Jumps in fright if approached by the player from behind, before attempting to run away.
- Returns to its original patrol route once it has reached a safe destination.
- Alerts its companion, H:A, of the player's presence and location upon detection of player.
- If TRB does not feel threatened, has been On-Duty (Patrol state) for long enough, and H:A can match these requirements, TRB enters "Chill Mode" and navigates toward a shared "Chill Location" where it will relax for a moment together with H:A before returning to its route.

**H:A**
- Plans a path from it's current location to a desired location based on pre-defined waypoints, either at random or in order, depending on whether or not the agent has detected (and dealt with) the players presence.
- Navigates between waypoints.
- Given detection of player, or if TRB has alerted H:A of its presence, H:A charges towards the players location, knocking the player away upon impact.
- If the player escapes H:As line of sight after having been previously detected, and stays hidden for a long enough time, H:A navigates to thhe last known location of the player.
- If the last known location of the player is out of reach, H:A will move as close as it can towards the last known location. If enough time has passed without H:A being able to reach the location, it resumes it's patrol path, which is then chosen at random.
- If H:A has been on patrol for a certain amount of time, and its companion TRB is ready to chill, it enters "Chill Mode" and navigates towards the shared "Chill Location". 

## 2. AI Design

### Techniques Used
- Finite State Machine (FSM)
- Behavior Tree (BT)
- Unity Behavior Graph (UBG)
- Unity NavMesh
- Unity NavMeshAgent
- Seek
- Arrive
- Flee
- Avoidance (inherent by Unity NavMeshAgent)

### How the Techniques Are Combined

**TRB**
Due to TRBs limited complexity, the decision was made to use a Finite State Machine (FSM) for its AI capabilites. The FSM is responsible for alternating between states such as "Patrol", "ReturnToPatrol", "Startled", "RunningAway", and "Chilling". The transitions between these states is determined mainly by a perception system which features a frontal cone detection system and a rear "jump scare" radius.

In order for TRB to be able to enter the "Chilling" state as well as alert H:A of the player's presence, it makes use of a state event channel which is connected to H:As behavior graph. The messages being sent to and from RTB is faciliated by a Coordinator class which acts as a middleman in order to perform necessary checks of relevant states before allowing certain states to be entered.

During "Patrol" state TRB makes use of the NavMeshAgent components inherent steering capabilites, with modifications to parameters mainly affecting deceleration and acceleration. The NavMeshAgent is at times disabled in order for TRB to be able to perform actions such as "JumpScare". During the "RunningAway" state, a larger set of parameters are modified in order to give TRB a flight response with greater granular control based on Craig .W Reynolds rule for the "Flee" steering behavior.

**H:A**
While TRBs AI capabilites was initially perceived as primitive, H:A was deemed to be more complex while also running a larger "risk" of growing in complexity over time as features evolved. While it was deemed to be more complex, the design of the game did not necessitate action planning in the way that GOAP otherwise facilitates it. Thus, the decision was made to make use of a *Behavior Tree*. The flow of actions and controls was designed and implemented using Unity's **Behavior Graph**, while action-related logic was hand crafted.

I wanted H:A to encapsulate a form of "Charge" ability. While it made sense to make use of Craig Reynold's "Flee" calculation formula for TRB's "Startled" state, I decided that in order to achieve what I wanted for the Charge ability, it was simply best to make use the NavMeshAgent's inherent "Arrive" behavior but with heavy modifications to the speed, acceleration and above all - decelaration. I did not want a "linear" degradation of speed, but rather a hard stop once the agent reached it's desired location. I felt like the knockback effect that would be imposed on the player upon agent-player impact would feel a lot more natural if the agent didn't come to a slow halt before applying some kind of directional force. The idea of it did not make sense to me.

Like TRB, H:A makes use of arrival and avoidance behavior during "Patrol" state. Furthermore, H:A's NavMeshAgent is also disabled during "Chill Mode", in order to allow for jumping..

### AI Decision Parameters Overview
Both TRB and H:A rely heavily on perception of their environment as a deterministic value in order to decide between actions, hopefully making them seem reactive in nature. When perception is not triggered by an "odd occurance" (such as a threat in the form of a player being detected), their decision-making is currently rather static, with decision-making depending on certain timer values in order to determine if it's time to "Patrol" or time to "Chill". 

## 3. Implementation Notes

### Key Classes / Blueprints

### Components
- NavMesh
- NavMeshAgent
  - A large portion of my agents pathing and navigation is built on top of these components, which enable the mapping of the graph, and in turn, efficient pathing in order to determine nodes to navigate between. The structure of nodes and edges is done by baking the scene.
-  AgentLinkMover
  - A customized UnityNavMeshComponents script that modifies the way agents travers between NavMeshLinks. I used this in order to modify the curve trajectory of the agents jumps between NavMeshes as well as the speed at which they travel.
- RollingBallController
  - Encapsulates the FSM logic, which in turn controls the movement and decision making process of TRB.
- AIPerception
  - A script that faciliates the agents detection of the player and maintenance of line of sight status.

### Data Structures
- Blackboard
  - Maintains a large majority of the relevant data necessary for the H:A decision-making process.
- Unity Behavior Graph
  - Allows for the visual design of H:A's overall behavior.
  - Stores world states such as "IsPlayerTagged".
  - Enables communication between TRB and H:A via Event Channels.
  - Encapsulates creation and execution of the leaf-node actions related to H:A.

### Performance Considerations
The biggest concern I have in relation to performance is the rate at which agents calculate desired paths. Currently I rely to a large degree on the internalized logic of ```agent.SetDestination(location)```. I think this is better for future-proofing when it comes to level design scaling, but since I don't have any dynamic obstacles I think it could be beneficial to refactor my movement logic to make use of NavMeshAgent's corners data, which is a result of the A* path calculation, and use that data as the basis for my traversal between nodes. The long-term sustainability I see with using SetDestination is the security it gives when it comes to the caching of the current waypoints.

## 4. Reflection

### What Went Well
Having taken some time to research steering behavior rules as defined by Craig W. Reynolds was a huge benefit when it came to understanding how the NavMeshAgent component exposes parameters which affect the agents' steering behaviors. It made it a lot easier to modify these parameters in a decisive manner, rather than spitballing between "works" and "does not work" settings.

I am happy with my planning phase. I recently discovered the web tool **Excalidraw** and I just loved the hand-drawn style of it. Pen and paper are good tools when it comes to conceptualizing ideas and understanding fundamentals, but during my planning phases I often times want to switch back and forth between inspiration, reading materials, etc. Having a web-based tool that gave me at least somewhat the feeling of "understanding" through pen and paper while also allowing me to be flexible between planning and research very nice.

I am happy with my scope. I feel like I challenged myself while still being able to systematically map out obstacles and corresponding solutions as they appeared, without feeling overwhelmed by an overscoped project idea featuring a plethora of unforeseen difficulties. Due to the level of scope, I felt like I had sufficient time to research and understand each problem set before progressing forward, which is something I greatly enjoyed. 

### What Could Be Improved
**Code Refactor**
I would refactor the perception system of the TRB agent. Perception is currently intertwined with the movement logic. I am kind of fine with it, due to the fact that the perception logic of the TRB is very limited, but still - I don't like that they dependent rather than co-existing.

**Movement Pattern**
I would've liked to give the TRB agent a more dynamic patrol pattern. One of the desired characteristics I had listed for TRB was "Oblivious". I feel like this could've been more clearly communicated to the player by making the TRB's movement patterns more irratic and unfocused. This might've been achieved by varying speeds between waypoints, random "go's" and "stops" with different rotations.

**Behavior Graph Re-design**
The behavior graph felt increasinly complex to work with, and I suspect it was due to my initial lack of understanding of control flow implementation in Unity. I would have liked to start completely fresh, re-designing the graph with a better understanding of control flows *and* with a more clear behavior pattern conceptualized. There were edge cases I had completely forgotten about, such as "what happens if the agent chases the player, but the player becomes unreachable?", which is a very common scenario in many games. This edge case fix was one of the very last things I implemented, and it did require some creative solutions, simply due to the already-in-place design of the tree. At times I tried to re-design larger portions of the graph, but often times there were dependency clashes that ultimately "forced" my line of thinking into certain directions.

**Animations, Materials and Sound**
As I planned this project, I had certain emotions I wanted to convey. Those emotions do become lacking in effect when animations and materials, even if simple, are missing. A charge feels more powerful if you can see the chargee lean forwards as they run, hear the *thud* as two objects collide, and see the sun reflected in a fear-stricken opponent.

### Lessons Learned
Graphs can grow large and complex, even from behavior considered "basic". I think following principles of design would be greatly beneficial, but I can't recall seeing any clear guidelines, such as coding standards like 'SOLID' and 'DRY', during my research.

Agent communication via Blackboard tools, such as Event Channels, felt very powerful in creating coherent AI implementations and behaviors, regardless of AI design pattern.

Next time, I would really like to try and spend a zealous amount of time on planning behavior in such a way that a branch is therotically explored until no conceivable edge case is left undiscovered, and then divide those behaviors up into branches. Then, like a detective with an evidence board, map out which branches branch into other branches, and from there, based on branch intersections, define development milestones. From there, I would like to break down the iterative steps required for each branch to reach that intersection, and start developing based on that roadmap. I feel this would make me better understand the necessary steps required to create certain behaviors, and I think it would help me plan my code architecture accordingly.
