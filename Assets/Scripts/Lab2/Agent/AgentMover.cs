using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMover : MonoBehaviour
{
    public GridManager gm;
    public Pathfinder pathfinder;
    public float moveSpeed = 3f;

    private List<Node> currentPath;
    private int currentIndex = 0;

    private PlayerController pc;

    private void Start()
    {
        pc = FindFirstObjectByType<PlayerController>();
        StartCoroutine(FollowPlayer());

    }

    IEnumerator FollowPlayer()
    {
        while (true)
        {
            Debug.Log("corutine ran");
            Vector3 targetPos = pc.transform.position;
            Node targetNode = gm.GetNodeFromWorldPosition(targetPos);

            Vector3 startPos = transform.position;
            Node startNode = gm.GetNodeFromWorldPosition(startPos);

            var path = pathfinder.FindPath(startNode, targetNode);
            FollowPath(path);

            yield return new WaitForSeconds(2);
        }
    }

    public void FollowPath(List<Node> path)
    {
        currentPath = path;
        currentIndex = 0;
    }

    private void Update()
    {

        if (currentPath == null || currentPath.Count == 0) return;

        Node targetNode = currentPath[currentIndex];
        Vector3 targetPos = NodeToWorldPosition(targetNode);

        Vector3 direction = targetPos - transform.position;
        float magnitude = Mathf.Sqrt(direction.x * direction.x +
                                     direction.y * direction.y +
                                     direction.z * direction.z);

        if (magnitude < 0.05f)
        {
            currentIndex++;
            if (currentIndex >= currentPath.Count)
            {
                currentPath = null;
            }
            return;
        }

        Vector3 normalizedDir = direction / magnitude;
        transform.position += normalizedDir * moveSpeed * Time.deltaTime;
    }

    private Vector3 NodeToWorldPosition(Node node)
    {
        return new Vector3(node.x * gm.CellSize, transform.position.y, node.y * gm.CellSize);

    }
}
