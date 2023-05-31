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
                nodes[i, j] = new Node(new Vector3(i * nodeSize.x, transform.position.y, j * nodeSize.y), IsNodeWalkable(new Vector3(i * nodeSize.x, transform.position.y, j * nodeSize.y)));
            }
        }
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

    public void OnDrawGizmos()
    {
        if (nodes == null)
        {
            return;
        }

        for (int i = 0; i < gridSize.x + 2; i++)
        {
            for (int j = 0; j < gridSize.y + 2; j++)
            {
                if (nodes[i, j].walkable)
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawCube(nodes[i, j].position, nodeSize);
            }
        }
    }
}

public class Node : ScriptableObject
{
    public Vector3 position;
    public bool walkable;
    List<Node> neighbours;

    public Node(Vector3 n_position, bool n_walkable)
    {
        position = n_position;
        walkable = n_walkable;
    }
}
