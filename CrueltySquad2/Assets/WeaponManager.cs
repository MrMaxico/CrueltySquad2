using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {
    public List<string> weapons = new List<string>();

    // Add a weapon to the list
    public void AddWeapon(string weaponName) {
        weapons.Add(weaponName);
    }

    // Save the list of weapons to PlayerPrefs
    public void SaveWeapons() {
        // Convert the list to a comma-separated string
        string weaponsString = string.Join(",", weapons.ToArray());

        // Save the string to PlayerPrefs
        PlayerPrefs.SetString("Weapons", weaponsString);
    }

    // Load the list of weapons from PlayerPrefs
    public void LoadWeapons() {
        // Check if the PlayerPrefs key exists
        if (PlayerPrefs.HasKey("Weapons")) {
            // Get the string from PlayerPrefs
            string weaponsString = PlayerPrefs.GetString("Weapons");

            // Split the string into an array of weapon names
            string[] weaponNames = weaponsString.Split(',');

            // Clear the existing list
            weapons.Clear();

            // Add each weapon name to the list
            foreach (string weaponName in weaponNames) {
                weapons.Add(weaponName);
            }
        }
    }
}
