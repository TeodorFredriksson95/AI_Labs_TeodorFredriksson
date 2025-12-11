using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pathfinder : MonoBehaviour
{
    [Header("Grid")]
    public GridManager gridManager;

    [Header("Materials")]
    [SerializeField] private Material pathTile;
    [SerializeField] private Material walkableTile;

    private InputAction spaceAction;

    private void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
    }

    private void OnEnable()
    {
        spaceAction = new InputAction(
            name: "Interact",
            type: InputActionType.Button,
            binding: "<Keyboard/e>"
            );
        spaceAction.started += SetPathTiles;
        spaceAction.Enable();
    }

    private void OnDisable()
    {
        if (spaceAction != null)
        {
            spaceAction.performed -= SetPathTiles;
            spaceAction.Disable();
        }
    }

    public void SetPathTiles(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            List<Node> path = FindPath(gridManager.StartNode, gridManager.TargetNode);
            for (int i = 0; i < path.Count; i++)
            {
                MeshRenderer renderer = path[i].tile.GetComponent<MeshRenderer>();
                renderer.material = pathTile;
            }
        }
    }

    public List<Node> FindPath(Node startNode, Node targetNode)
    {

        // 1. Reset node costs
        gridManager.ResetNodes();


        // 2. Initialize openSet and closedSet
        List<Node> openSet = new List<Node> { startNode };
        List<Node> closedSet = new();

        // 3. Set gCost and hCost for startNode
        startNode.gCost = 0;
        startNode.hCost = HeuristicCost(startNode, targetNode);

        // 4. Loop until openSet is empty:
        while (openSet.Any())
        {
            Node current = openSet[0];
            // - pick node with lowest fCost
            foreach (var node in openSet)
            {
                if (node.fCost < current.fCost || node.fCost == current.fCost && node.hCost < current.hCost)
                    current = node;
            }


            // - if this is goalNode, reconstruct and return path
            if (current == targetNode)
            {
                Node currentPathTile = targetNode;
                List<Node> path = new List<Node>();
                int count = 100;

                while (currentPathTile != startNode)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.parent;
                    count--;

                    // Why, mister Tarodev? Why?
                    if (count < 0) throw new Exception();
                }

                path.Add(startNode);
                openSet.Remove(current);

                path.Reverse();

                return path;
            }

            // - otherwise, move it to closedSet
            else
            {
                closedSet.Add(current);
            }

            // - for each neighbour:
            IEnumerable<Node> neighbours = gridManager.GetNeighbours(current, true);
            foreach (var neighbour in neighbours)
            {
                // - skip if null, not walkable, or in closedSet
                if (neighbour != null && neighbour.walkable && !closedSet.Contains(neighbour))
                {
                    // - compute tentativeG = current.gCost + stepCost
                    float costToNeighbour = current.gCost + 1;

                    // - if tentativeG < neighbour.gCost:
                    if (costToNeighbour < neighbour.gCost)
                    {
                        // - update neighbour.parent, gCost, hCost
                        neighbour.parent = current;
                        neighbour.gCost = costToNeighbour;
                        neighbour.hCost = HeuristicCost(neighbour, targetNode);

                        // - ensure neighbour is in openSet
                        if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    float HeuristicCost(Node a, Node b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        return dx + dy;
    }
}
