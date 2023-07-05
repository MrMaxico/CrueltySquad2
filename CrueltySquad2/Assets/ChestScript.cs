using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChestScript : MonoBehaviour {
    public WeightedRandomList<GameObject> lootTable;
    public Transform gunSpawnLocation;
    public float cycleSpeed = 1;
    public float amountOfCycles = 10;
    private bool waiting;
    private bool Delay = true;
    public GameObject weapon;
    public GameObject prevWeapon;
    private int i;
    public Animator chestAnim;
    public float chestAnimDelay;
    public void Open() {
        if(i <= amountOfCycles && waiting == false) {
            waiting = true;
            StartCoroutine(WeaponCycle());
        } else if(waiting == false) {
            weapon = GameObject.Instantiate(lootTable.GetRandom(), gunSpawnLocation.position, gunSpawnLocation.rotation);
            weapon.transform.parent = gunSpawnLocation;
            waiting = true;
        }
    }
    private IEnumerator WeaponCycle() {
        if (i <= 0 && Delay) {
            chestAnim.SetTrigger("open");
            yield return new WaitForSeconds(chestAnimDelay);
            Delay = false;
        }
        prevWeapon = weapon;
        weapon = GameObject.Instantiate(lootTable.GetRandom(), gunSpawnLocation.position, gunSpawnLocation.rotation);
        weapon.GetComponent<Rigidbody>().useGravity = false;
        weapon.GetComponent<Collider>().enabled = false;
        weapon.GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(cycleSpeed);
        Destroy(weapon);
        i++;
        waiting = false;
        Open();
    }
}
