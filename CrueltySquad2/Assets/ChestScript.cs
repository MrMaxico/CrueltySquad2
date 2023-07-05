using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChestScript : MonoBehaviour {
    public WeightedRandomList<GameObject> lootTable;
    public Transform gunSpawnLocation;
    private float cycleSpeed = 1;
    private bool waiting;
    public void Open() {
        for (int i = 0; !waiting && i < 10; i++) {
            waiting = true;
            WeaponCycle();
        }
    }
    private IEnumerator WeaponCycle() {
        yield return new WaitForSeconds(cycleSpeed);
        GameObject.Instantiate(lootTable.GetRandom(), transform.position, transform.rotation).GetComponent<Collider>().enabled = false;
        cycleSpeed -= 0.1f;
        waiting = false;
    }
}
