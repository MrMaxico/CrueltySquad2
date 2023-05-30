using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector3 nodeSize;
    public LayerMask unwalkable;
    public IslandGenerator generator;

    private void Start()
    {
        generator = GetComponent<IslandGenerator>();
    }

    public void OnDrawGizmos()
    {
        List<Field> fields = Fields(generator.GetVerts());
        for (int i = 0; i < fields.Count; i++)
        {
            if (fields[i].walkable)
            {
                Gizmos.color = Color.white;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawCube(fields[i].position, nodeSize);
        }
    }

    public List<Field> Fields(Vector3[] positions)
    {
        List<Field> fields = new List<Field>(capacity: positions.Length);
        for (int i = 0; i < positions.Length; i++)
        {
            fields.Add(new Field(positions[i], CheckIfFieldIsWalkable(positions[i])));
        }
        return fields;
    }

    bool CheckIfFieldIsWalkable(Vector3 fieldPosition)
    {
        return !(Physics.CheckSphere(fieldPosition, nodeSize.x, unwalkable));
    }
}

public class Field 
{
    public Vector3 position;
    public bool walkable;

    public Field(Vector3 n_pos, bool n_walk)
    {
        position = n_pos;
        walkable = n_walk;
    }
}
