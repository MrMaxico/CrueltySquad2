using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour {
    public Transform primaryHolder;
    public GameObject secondaryHolder;
    public GameObject cam;
    public RaycastHit hit;
    public bool holdingPrimary;
    public bool holdingSecondary;

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.E)) {
            Debug.Log("Pressed E");
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1000)) {
                if (hit.transform.CompareTag("Gun")) {
                    Debug.Log("Gun");
                    PickUpGun(hit.transform);
                }
            }
        }
    }

    void PickUpGun(Transform gunTransform) {
        gunTransform.SetParent(primaryHolder);
        gunTransform.position = primaryHolder.position;
        Rigidbody gunRigidbody = gunTransform.GetComponent<Rigidbody>();
        gunRigidbody.useGravity = false;
        gunRigidbody.freezeRotation = true;
        gunRigidbody.isKinematic = true;
        // Additional gun pickup logic if needed
    }
}
