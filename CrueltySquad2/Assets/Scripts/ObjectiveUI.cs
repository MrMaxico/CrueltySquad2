using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI islandNumber;
    public TextMeshProUGUI objectiveOne;
    public TextMeshProUGUI objectiveTwo;
    public GameObject teleporter;

    private void Start()
    {
        islandNumber.text = $"Island: {Teleporter.islandNumber}";
    }

    public void Update()
    {
        if (teleporter == null)
        {
            teleporter = GameObject.FindGameObjectWithTag("Teleporter");
        }
        else if (teleporter.GetComponent<Teleporter>().spawnersLeft > 0)
        {
            objectiveOne.text = $"Destroy all bug nests";
            objectiveTwo.text = $"Bug nests left: {teleporter.GetComponent<Teleporter>().spawnersLeft}";
        }
        else if (teleporter.GetComponent<Teleporter>().spawnersLeft <= 0)
        {
            objectiveOne.text = $"Find and activate the teleporter";
            objectiveTwo.text = $"";
        }
    }
}
