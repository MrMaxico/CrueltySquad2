using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public IslandGenerator generator;
    public Vector2 gridSize;
    public Vector3 nodeSize;
    public bool calculateGridSizeBasedOnIslandFormat;
    public Node[,] nodes;
    public LayerMask unwalkable;

    public void CreateGrid(IslandGenerator n_generator)
    {
        generator = n_generator;

        if (calculateGridSizeBasedOnIslandFormat)
        {
            gridSize.x = generator.xSize / nodeSize.x;
            gridSize.y = generator.zSize / nodeSize.z;
        }

        nodes = new Node[Mathf.RoundToInt(gridSize.x + 2), Mathf.RoundToInt(gridSize.y + 2)];

        for (int i = 0; i < gridSize.x + 2; i++)
        {
            for (int j = 0; j < gridSize.y + 2; j++)
            {
                nodes[i, j] = new Node(new Vector3(i * nodeSize.x, FindHeight(new Vector3(i * nodeSize.x, transform.position.y, j * nodeSize.y)), j * nodeSize.y), IsNodeWalkable(new Vector3(i * nodeSize.x, transform.position.y, j * nodeSize.y)));
            }
        }

        for (int i = 0; i < gridSize.x + 2; i++)
        {
            for (int j = 0; j < gridSize.y + 2; j++)
            {
                nodes[i, j].neighbors = GetNeighbours(nodes, i, j);
                nodes[i, j].gCost = 9999;
                nodes[i, j].hCost = 9999;
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Node startNode = GetClosestNode(startPosition);
        Node targetNode = GetClosestNode(targetPosition);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return GeneratePath(startNode, targetNode);

            foreach (Node neighbor in currentNode.neighbors)
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                float moveCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (moveCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = moveCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // No path found
        return null;
    }

    private List<Vector3> GeneratePath(Node startNode, Node targetNode)
    {
        LinkedList<Vector3> path = new LinkedList<Vector3>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.AddFirst(currentNode.position);
            currentNode = currentNode.parent;
        }

        return new List<Vector3>(path);
    }

    public float FindHeight(Vector3 position)
    {
        if (Physics.Raycast(position, -transform.up, out RaycastHit hit, 1000))
        {
            return hit.point.y;
        }
        return position.y;
    }

    public bool IsNodeWalkable(Vector3 n_position)
    {
        if (Physics.Raycast(n_position, -transform.up, out RaycastHit hit, 1000))
        {
            if (hit.point.y > (generator.bottomVertHeight + generator.extraNoice) * 2)
            {
                return false;
            }
            else
            {
                return !(Physics.CheckSphere(hit.point, nodeSize.x, unwalkable));
            }
        }
        return false;
    }

    public List<Node> GetNeighbours(Node[,] nodes, int x_index, int y_index)
    {
        List<Node> returnList = new List<Node> { };

        for (int i = x_index - 1; i <= x_index + 1; i++)
        {
            if (i >= 0 && i < gridSize.x + 1)
            {
                for (int j = y_index - 1; j <= y_index + 1; j++)
                {
                    if (j >= 0 && j < gridSize.y + 1)
                    {
                        if (nodes[i, j].walkable)
                        {
                            returnList.Add(nodes[i, j]);
                        }
                    }
                }
            }
        }

        return returnList;
    }

    public Node GetClosestNode(Vector3 position)
    {
        float distanceToCurrentClosest = 9999;
        Vector2 index = new Vector2(0,0);

        for (int i = 0; i <= gridSize.x + 1; i++)
        {
            for (int j = 0; j <= gridSize.x + 1; j++)
            {
                if (Vector3.Distance(position, nodes[i, j].position) < distanceToCurrentClosest && nodes[i, j].walkable)
                {
                    distanceToCurrentClosest = Vector3.Distance(position, nodes[i, j].position);
                    index = new Vector2(i, j);
                }
            }
        }
        return nodes[Mathf.RoundToInt(index.x), Mathf.RoundToInt(index.y)];
    }

    private float GetDistance(Node nodeA, Node nodeB)
    {
        return Vector3.Distance(nodeA.position, nodeB.position);
    }

    //public void OnDrawGizmos()
    //{
    //    if (nodes == null)
    //    {
    //        return;
    //    }

    //    for (int i = 0; i < gridSize.x + 2; i++)
    //    {
    //        for (int j = 0; j < gridSize.y + 2; j++)
    //        {
    //            if (nodes[i, j].walkable)
    //            {
    //                Gizmos.color = Color.white;
    //            }
    //            else
    //            {
    //                Gizmos.color = Color.red;
    //            }
    //            Gizmos.DrawCube(nodes[i, j].position, nodeSize);
    //        }
    //    }
    //}
}

public class Node : ScriptableObject
{
    public Vector3 position;
    public bool walkable;
    public float gCost;
    public float hCost;
    public Node parent;
    public List<Node> neighbors;

    public Node(Vector3 n_position, bool n_walkable)
    {
        position = n_position;
        walkable = n_walkable;
        neighbors = new List<Node>();
    }

    public float fCost => gCost + hCost;
}
