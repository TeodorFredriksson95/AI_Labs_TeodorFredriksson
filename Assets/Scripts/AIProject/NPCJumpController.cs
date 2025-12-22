using Unity;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.AIProject
{
    public class NPCJumpController
    {

            public bool isJumping;
            public bool isGrounded = true;
            public Vector3 tempVelocity;
            public float groundY;
            public float verticalVelocity;

            private readonly NavMeshAgent agent;
            private readonly Transform transform;
            private readonly float gravity;

            public NPCJumpController(NavMeshAgent agent, Transform transform, float gravity)
            {
                this.agent = agent;
                this.transform = transform;
                this.gravity = gravity;
            }

            public void StartJump(float jumpForce)
            {
                if (isJumping || !isGrounded) return;

                isJumping = true;
                isGrounded = false;

                tempVelocity = agent.velocity;
                agent.velocity = Vector3.zero;

                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;

                groundY = transform.position.y;
                verticalVelocity = jumpForce;
            }

            public void Tick()
            {
                if (!isJumping) return;

                verticalVelocity += gravity * Time.deltaTime;
                transform.position += Vector3.up * verticalVelocity * Time.deltaTime;

                if (transform.position.y <= groundY)
                    Land();
            }

            public void Land()
            {
                Vector3 pos = transform.position;
                pos.y = groundY;
                transform.position = pos;

                agent.velocity = tempVelocity;
                verticalVelocity = 0f;
                isJumping = false;
                isGrounded = true;

                agent.Warp(transform.position);
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.isStopped = false;
            }

            public bool IsJumping => isJumping;
        }
}
