

using System;
using UnityEngine;
using UnityEngine.AI;
using static EnemyPatrol;

public class PatrolState : IPatrol
{
    public void HandleInput(EnemyPatrol guard, GuardState state)
    {
        throw new NotImplementedException();
    }


    public void Update(EnemyPatrol guard)
    {
        throw new NotImplementedException();
    }

    //public void Update(EnemyPatrol guard, Transform player)
    //{
    //    float distance = Vector3.Distance(guard.transform.position, player.transform.position);
    //    if (distance <= chaseRange) currentState = GuardState.Chasing;

    //    if (!agent.pathPending && agent.remainingDistance < 0.5f)
    //    {
    //        agent.SetDestination(patrolPoints[destPoint].position);
    //        destPoint = (destPoint + 1) % patrolPoints.Length;
    //    }
    //}
}