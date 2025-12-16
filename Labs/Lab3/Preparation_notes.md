# Lecture 3 – Steering, Movement & Group / Swarm AI  
## Warm-Up & Preparation Questions

## 3.1 From paths to steering

### 1. In your own words, what is the difference between:  
- A path (like the list of nodes from A* in Lab 2), and  
- A steering behaviour (like seek or flee)?  

**Answer:**  
---
- A path calculated by an algorithm such as A* retrieves sequence of nodes specifying a valid (supposedly) path between the starting node and the goal node. It takes obstacles, heuristic cost and step cost into consideration. However, I would
- argue that these obstacles are static in nature as opposed to steering behaviours like seek or flee. Steering behaviours are dynamic in nature, and performs calculations on aspects such as speed, direction, force and rotation, based on
- dynamic detection in a local space.

### 2. Imagine an enemy in a maze game:  
A* finds a path of waypoints to the player.  
What can go wrong if the enemy just snaps its direction directly to the path every frame (no smoothing, no max turn rate)?

**Answer:**  
---
- I'm pretty sure this would appear quite laggy.

### 3. Think back to your Lab 1 patrolling guard:  
How would you like the guard’s movement to feel while it is patrolling or chasing?  
List 2–3 adjectives (e.g., “smooth”, “snappy”, “panicky”, “robotic”, “lazy”) and think about what kind of steering might cause that feel.

**Answer:**  
---
- Vigilant: Wander
- Reactive: Obstacle Avoidance, (Pursuit with Arrive). I intentionally separated OA from Pursuit&Arrive because I want the guard to leap full force towards my target in an aggressive manner. The worry of obstacles appearing in this
guards way is secondary!

## 3.2 Single-agent steering intuition

### 4. Suppose you have an agent with:  
- current position **P**,  
- current velocity **V**,  
- a target position **T**.

In words (no code needed):
- What does the desired direction from P to T look like?  
- What happens if you instantly set V = “desired direction * maxSpeed” every frame?  
  When might that look bad?

**Answer:**  
---
- Given to positional values represented as vectors, the desired direction from P to T would probably look somthing like this:
  ```
  Vector3 desired = (targetPosition - currentPosition);
  // Assume we're preparing this as a velocity vector with a max speed
  desired.magnitude = maxSpeed;
  Vector3 steer = (desired, this.velocity);
  Vector3 clampedSteer = MathF.Clamp(steer, maxSteer);
  rb.ApplyForce(clampedSteer);
  ```
- It would look choppy. If the target is moving, this would appear as laggy. Another issue with always using maxSpeed is that we risk overshooting our targets position (this is when Arrive comes into the picture).

### 6. What’s the practical difference between these steering intentions:  
- Seek a target vs Arrive at a target.  
- Flee from a target vs Evade a target that is moving.

**Answer:**  
---
- Seek will make no regard to it's current velocity in relation to the remaining distance to it's destination. We risk overshooting our targets position. Arrive accelerates/decelerates in proportion to the targets predicted future position.
- Flee behaves in the exact same way as Seek, but in the opposite direction (radially aligned). Similarly, Evade is the counterpart of Pursuit, but Evade uses the same steering mechanisms as Flee (trying to radially align, ie, turn in the opposite direction.
- 

### 7. Why might it be useful to limit how fast an agent can turn or change its velocity (limit steering force / acceleration), instead of letting it snap instantly?

**Answer:**  
---
- Otherwise it may give the appearance of being laggy. It depends on the use case though. It might make sense for a turret in a tower defense game to rotate instantly. 

## 3.3 Groups & swarms

### 8. In flocking / boids, describe in your own words:  
- Separation  
- Alignment  
- Cohesion  

**Answer:**  
---
- Separation: Enables the character to maintain a certain distance between itself and other local objects.
- Alignment: Calculates the average velocity, or forward vector, which gives neighbouring units the ability to align themselves via steering based on the average velocity parameter.
- Cohesion: Calculates the average direction per a group of locally sourced neighbour objects, and applies force towards that direction. This is useful when you want a character to approach and group together with some other unit(s).

### 9. Give one game example where each of these would be useful.

**Answer:**  
---

### 10. Imagine a flock of 50 birds using flocking rules:  
What might happen if you remove **separation** but keep alignment and cohesion?  
What if you remove **alignment** but keep separation and cohesion?

**Answer:**  
---
- The birds would try to align, while also trying to steer towards it's closest neighbour, without a desire to "avoid" crashing into them. I think ultimately, there would be a condensed flock of birds circuling in the middle, often overlapping each other.
- The birds would try to keep their distance, but they would also most likely crash at some form of intersection, as they would all try to converge together as a group, but would not take into consideration any alignment steering, which would otherwise keep the brids trying to steer in a uniform manner.
  

### 11. Think about performance:  
- Each boid needs to look at its neighbours.  
- If you have 500 boids, what might be an issue with the naïve “check all other agents” approach?  
- Can you think of a simple way to reduce the cost (even just conceptually)?

**Answer:**  
---
- It would be an insane amount CPU calls trying to perform the calculations for each individual boid.
- When calculating Alignment, Cohesion and Separation, you do so within a specified area, also called a "neighbourhood". Perhaps the neighbourhood radius could be reduced per flock member in order to reduce the amount parameters taken into
consideration for each calculation. Craig W. Reynolds talks about limiting the amount of steering components to be computed each simulation step, and rather leverage the characters momentum as a low-pass filter in when it comes to applying changes to steering. However, I am currently having a hard time wrapping my head around this. To be continued!

# 4. Small Pen-and-Paper / Pseudo-code Exercises (Optional)

### 1. Draw a simple steering scenario  
Draw a top-down scene with one agent, a target, and a wall.  
Sketch the desired curved path and mark where seek dominates vs avoid wall.

**Answer:**  
---

### 2. Combine two behaviours in words  
Suppose you have a “seek target” vector and an “avoid obstacles” vector.  
How might you combine them? (Weighted sum, priorities, override rules, etc.)

**Answer:**  
---

### 3. Tiny flock  
Draw 4–5 boids. For one boid, sketch:  
- Separation vector  
- Alignment vector  
- Cohesion vector

**Answer:**  
---

# 5. Technical Expectations & Engine Warm-Up

### 1. Be comfortable with basic vector operations  
(Adding vectors, scaling, normalizing, using deltaTime)

**Answer:**  
---

### 2. Be able to write a simple movement script  
Moves an agent in a direction at a given speed and optionally rotates it to face velocity.

**Answer:**  
---

### Suggested Mini-Coding Task  
Implement a simple seek behaviour in your engine.  
Experiment with speeds, moving targets, and optional arrival.

**Answer:**  
---
