using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatMenuScript : MonoBehaviour
{
    public PauzeScript pauzeScript;
    public void InstantiateWapens(string weaponName) {
        pauzeScript.Resume();
        Destroy(GameObject.FindGameObjectWithTag("Player").GetComponent<PickUpController>().primary.gameObject);
        GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>().LoadWeapon(weaponName, 0);
    }
    public void ChangeIslandType(string island) {
        pauzeScript.Resume();
        SceneManager.LoadScene(island);
    }
}
