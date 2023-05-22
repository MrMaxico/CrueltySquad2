using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour {
    public Transform primaryHolder;
    public Transform primary;
    public GameObject secondaryHolder;
    public float dropForwardForce;
    public float dropUpwardForce;
    public GameObject cam;
    public RaycastHit hit;
    public bool holdingPrimary;
    public bool holdingSecondary;
    public bool pickUpDelay;


    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.E)) {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1000)) {
                if (hit.transform.CompareTag("Gun") && !pickUpDelay) {
                    if (primary == null) {
                        Debug.Log("Gun");
                        PickUpGun(hit.transform);
                    } else {
                        Debug.Log("Pressed E");
                        pickUpDelay = true;
                        SwapGun(hit.transform);
                    }
                }
            }
        }
    }

    void PickUpGun(Transform gunTransform) {
        primary = gunTransform;
        Rigidbody gunRigidbody = gunTransform.GetComponent<Rigidbody>();
        gunTransform.SetParent(primaryHolder);
        gunTransform.position = primaryHolder.position;
        gunTransform.rotation = primaryHolder.rotation;
        gunTransform.GetComponent<BoxCollider>().enabled = false;
        gunRigidbody.useGravity = false;
        gunRigidbody.freezeRotation = true;
        gunRigidbody.isKinematic = true;
        pickUpDelay = false;
        // Additional gun pickup logic if needed
    }
    void SwapGun(Transform gunTransform) {
        Rigidbody gunRigidbody = primary.GetComponent<Rigidbody>();
        //Set parent to null
        primary.SetParent(null);

        //Make Rigidbody not kinematic and BoxCollider normal
        gunRigidbody.isKinematic = false;
        gunRigidbody.useGravity = true;
        gunRigidbody.freezeRotation = false;
        //Gun carries momentum of player
        gunRigidbody.velocity = this.GetComponent<Rigidbody>().velocity;

        //AddForce
        gunRigidbody.AddForce(cam.transform.forward * dropForwardForce, ForceMode.Impulse);
        gunRigidbody.AddForce(cam.transform.up * dropUpwardForce, ForceMode.Impulse);
        //Add random rotation
        float random = Random.Range(-1f, 1f);
        gunRigidbody.AddTorque(new Vector3(random, random, random) * 10);
        primary.GetComponent<BoxCollider>().enabled = true;
        StartCoroutine(PickUpDelay(gunTransform));
    }
    IEnumerator PickUpDelay(Transform gunTransform) {
        yield return new WaitForSeconds(.2f);
        PickUpGun(gunTransform);
    }
}
