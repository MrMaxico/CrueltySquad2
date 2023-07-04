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
    public WeaponManager weaponManager;
    public PickUpController pickUp;

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
            pickUp = GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>();
            weaponManager = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();
            DontDestroyOnLoad(weaponManager.gameObject);
            weaponManager.currentWeapons[0] = pickUp.primary.gameObject.name;
            if(pickUp.secondary != null) {
                weaponManager.currentWeapons[1] = pickUp.secondary.gameObject.name;
            }
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>().primary != null)
            {
                Debug.Log($"Saving slot 1 as {GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>().primary.gameObject.name}");
                GameObject saveSlotOne = GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>().primary.gameObject;
                DontDestroyOnLoad(saveSlotOne);
                PickUpController.primarySavedGun = GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>().primary;
            }
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>().secondary != null)
            {
                Debug.Log($"Saving slot 2 as {GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>().secondary.gameObject.name}");
                DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>().secondary.gameObject);
                PickUpController.secondarySavedGun = GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>().secondary;
            }
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
