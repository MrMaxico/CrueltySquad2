using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatMenuScript : MonoBehaviour
{
    public WeaponManager weaponManager;
    public void InstantiateWapens(string weaponName) {
        weaponManager = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();
        weaponManager.LoadWeapon(weaponName, 1);
    }
    public void ChangeIslandType(string island) {
        SceneManager.LoadScene(island);
    }
}
