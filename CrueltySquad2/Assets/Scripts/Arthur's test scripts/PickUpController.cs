using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public GameObject primaryHolder;
    public GameObject secondaryHolder;
    public RaycastHit hit;

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.E)) {
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1000)) {
                
                print(hit.collider.gameObject.tag);
            }
        }
    }
}
