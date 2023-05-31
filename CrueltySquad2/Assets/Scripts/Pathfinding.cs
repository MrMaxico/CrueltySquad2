using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public OldGrid grid;
    public IslandGenerator generator;

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        Field startField = Vector3ToField(start);
        Field endField = Vector3ToField(end);


        return null;
    }

    public Field Vector3ToField(Vector3 vector3)
    {
        float closestDis = 9999;
        int closestIndex = -1;
        for (int i = 0; i < grid.Fields(generator.GetVerts()).Count; i++)
        {
            if (Vector3.Distance(vector3, generator.GetVerts()[i]) < closestDis)
            {
                closestDis = Vector3.Distance(vector3, generator.GetVerts()[i]);
                closestIndex = i;
            }
        }
        if (closestIndex != -1)
        {
            return grid.Fields(generator.GetVerts())[closestIndex];
        }
        return null;
    }

    public List<Field> Neighbours(Field field, float maxDistance)
    {
        return null;
    }

    public Field BestField(List<Field> fields, Field currentField, Field goalField)
    {
        return null;
    }
}
