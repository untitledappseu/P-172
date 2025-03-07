using UnityEngine;
using System.Collections.Generic;

public class AStarPathfinding
{
    private GridManager grid;

    public AStarPathfinding(GridManager gridManager)
    {
        grid = gridManager;
    }

    public List<Node> FindPath(Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            // Find the node with the lowest F cost
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // If we've reached the target
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // Check all the neighbors
            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                // Skip if the neighbor is not walkable or is already evaluated
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                // Calculate new path cost
                float newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                // If the new path is better or the neighbor is not in the open set
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // No path found
        return null;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private float GetDistance(Node nodeA, Node nodeB)
    {
        // Manhattan distance, suitable for grid-based movement with no diagonals
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // For a platformer, we care more about horizontal distance
        return distX * 10f + distY * 5f;
    }
}