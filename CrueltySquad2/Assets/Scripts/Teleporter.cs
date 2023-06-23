using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    static public int islandNumber;
    public int spawnersLeft;
    public List<RandomVariable> nextIslandPresets;
    bool open;

    private void Start()
    {
        if (islandNumber < 1)
        {
            islandNumber = 1;
        }
    }
    private void Update()
    {
        if (spawnersLeft < 1)
        {
            open = true;
        }
        else
        {
            open = false;
        }

        if (Input.GetKey(KeyCode.T))
        {
            open = true;
            Teleport();
        }
    }

    public void Teleport()
    {
        if (open)
        {
            islandNumber++;
            SceneManager.LoadScene(RandomIsland());
        }
    }

    string RandomIsland()
    {
        List<string> islandTypeChanceList = new();
        foreach (RandomVariable islandType in nextIslandPresets)
        {
            for (int i = 0; i < islandType.spawnChance; i++)
            {
                islandTypeChanceList.Add(islandType.String);
            }
        }
        return islandTypeChanceList[Mathf.RoundToInt(Random.Range(0, islandTypeChanceList.Count))];
    }
}
