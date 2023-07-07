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
            weaponManager.currentWeapons[0] = pickUp.primary.GetComponent<GunData>().gunName;
            weaponManager.primaryGunIslandNumber = pickUp.primary.GetComponent<GunData>().ogislandNummer;
            if(pickUp.secondary != null) {
                weaponManager.currentWeapons[1] = pickUp.secondary.GetComponent<GunData>().gunName;
                weaponManager.secondaryGunIslandNumber = pickUp.primary.GetComponent<GunData>().ogislandNummer;
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
