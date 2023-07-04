using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponManager : MonoBehaviour {
    public GameObject[] weaponPrefabs;
    public string[] currentWeapons = new string[2];
    public GameObject[] newWeapons = new GameObject[2];

    public PickUpController pickUp;

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        StartCoroutine(LoadingDelay());
    }

    public void LoadWeapon(string weaponName, int slotIndex) {
        // Find the weapon prefab based on its name
        GameObject weaponPrefab = FindWeaponPrefab(weaponName);
        pickUp = GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>();
        if (weaponPrefab != null) {
            // Instantiate the weapon and assign it to the specified slot
            newWeapons[slotIndex] = Instantiate(weaponPrefab, transform);
            if (slotIndex == 0) {
                pickUp.holder = pickUp.primaryholder;
                pickUp.holdingPrimary = true;
                pickUp.holdingSecondary = false;
                pickUp.secondaryHolder.gameObject.SetActive(false);
                pickUp.primaryholder.gameObject.SetActive(true);
            } else if (slotIndex == 1) {
                Debug.Log("StinkBil");
                pickUp.holder = pickUp.secondaryHolder;
                pickUp.holdingPrimary = false;
                pickUp.holdingSecondary = true;
                pickUp.secondaryHolder.gameObject.SetActive(true);
                pickUp.primaryholder.gameObject.SetActive(false);
            }
            pickUp.PickUpGun(newWeapons[slotIndex].transform);
        } else {
            Debug.LogWarning("Weapon prefab not found: " + weaponName);
        }
    }

    private GameObject FindWeaponPrefab(string weaponName) {
        // Find the weapon prefab in the list based on its name
        foreach (GameObject weaponPrefab in weaponPrefabs) {
            if (weaponPrefab.GetComponent<GunData>().gunName == weaponName) {
                return weaponPrefab;
            }
        }

        return null; // Weapon prefab not found
    }
    private IEnumerator LoadingDelay() {
        Debug.Log("Kont");
        yield return new WaitForSeconds(2f);
        LoadWeapon(currentWeapons[0], 0);
        if(currentWeapons[0] != "") {
            LoadWeapon(currentWeapons[1], 1);
        }
    }
}
