using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour {
    public Transform primaryholder;
    public Transform primary;
    public Transform secondaryHolder;
    public Transform secondary;
    public Transform holder;
    public float dropForwardForce;
    public float dropUpwardForce;
    public GameObject cam;
    public RaycastHit hit;
    public bool holdingPrimary;
    public bool holdingSecondary;
    public bool pickUpDelay;

    private void Start() {
        holdingPrimary = true;
        holdingSecondary = false;
    }
    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.E)) {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1000)) {
                if (hit.transform && !pickUpDelay && hit.transform.CompareTag("Gun")) {
                    if (holdingPrimary && primary == null) {
                        Debug.Log("Gun");
                        PickUpGun(hit.transform);
                    } else if (holdingSecondary && secondary == null){
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
        Rigidbody gunRigidbody = gunTransform.GetComponent<Rigidbody>();
        if (holdingPrimary) {
            holder = primaryholder;
            primary = gunTransform;
        } else if(holdingSecondary) {
            holder = secondaryHolder;
            secondary = gunTransform;
        }
        gunTransform.SetParent(holder);
        gunTransform.position = holder.position;
        gunTransform.rotation = holder.rotation;
        gunTransform.GetComponent<MeshCollider>().enabled = false;
        gunRigidbody.useGravity = false;
        gunRigidbody.freezeRotation = true;
        gunRigidbody.isKinematic = true;
        pickUpDelay = false;
        // Additional gun pickup logic if needed
    }
    void SwapGun(Transform gunTransform) {
        if (holdingPrimary) {
            holder = primaryholder;
        } else if (holdingSecondary) {
            holder = secondaryHolder;
        }
        Rigidbody gunRigidbody = holder.GetChild(0).GetComponent<Rigidbody>();
        //Set parent to null
        holder.GetChild(0).SetParent(null);

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
        if (holdingPrimary) {
            primary.GetComponent<MeshCollider>().enabled = true;
        } else {
            secondary.GetComponent<MeshCollider>().enabled = true;
        }
        StartCoroutine(PickUpDelay(gunTransform));
    }
    IEnumerator PickUpDelay(Transform gunTransform) {
        yield return new WaitForSeconds(.2f);
        PickUpGun(gunTransform);
    }
}
