using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpController : MonoBehaviour {
    [Header("Holders for Guns", order = 0)]
    public Transform primaryholder;
    public static Transform primarySavedGun;
    public Transform primary;
    public Transform secondaryHolder;
    public static Transform secondarySavedGun;
    public Transform secondary;
    public Transform holder;
    public GameObject startingPistol;
    [Header("Medkit Pickup Radius and variables", order = 1)]
    public Transform medkitPickUpSphere;
    public float medkitPickUpRadius;
    public float medkitHealPercentage;
    [Header("Drop variables", order = 2)]
    public float dropForwardForce;
    public float dropUpwardForce;
    [Header("References", order = 3)]
    public GameObject cam;
    public RaycastHit hit;
    public Health playerHealth;
    [Header("Boolians", order = 4)]
    public bool holdingPrimary;
    public bool holdingSecondary;
    public bool pickUpDelay;
    public RawImage gunicon;
    public RawImage secondaryicon;
    public void Start() {
        holdingPrimary = true;
        holdingSecondary = false;
        if (Teleporter.islandNumber == 1)
        {
            PickUpGun(Instantiate(startingPistol, transform).transform);
        } else {

            holder = secondaryHolder;
            holdingPrimary = false;
            holdingSecondary = true;
            secondaryHolder.gameObject.SetActive(true);
            primaryholder.gameObject.SetActive(true);
            Debug.Log(secondarySavedGun);
            if (secondarySavedGun != null)
            {
                PickUpGun(secondarySavedGun);
                Debug.Log("secondary gun spawned");
            }
            holdingSecondary = false;
            holder = primaryholder;
            holdingPrimary = true;
            if (primarySavedGun != null)
            {
                PickUpGun(primarySavedGun);
                Debug.Log("primary gun spawned");
            }
            secondaryHolder.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update() {
        Debug.Log(primarySavedGun);

        if (Input.GetKey(KeyCode.E)) {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2)) {
                if (hit.transform && !pickUpDelay && hit.transform.CompareTag("Gun")) {
                    if (holdingPrimary && primary == null) {
                        Debug.Log("Gun");
                        PickUpGun(hit.transform);
                    } else if (holdingSecondary && secondary == null){
                        Debug.Log("Gun");
                        PickUpGun(hit.transform);
                        gunicon.enabled = true;
                    } else {
                        Debug.Log("Pressed E");
                        pickUpDelay = true;
                        SwapGun(hit.transform);
                    }
                }
                else if (hit.transform.CompareTag("Teleporter"))
                {
                    hit.transform.parent.transform.gameObject.GetComponent<Teleporter>().Teleport();
                }
            }
        }
        Collider[] colliders = Physics.OverlapSphere(medkitPickUpSphere.transform.position, medkitPickUpRadius);
        foreach (Collider collider in colliders) {

            if (collider.CompareTag("Medkit") && playerHealth.GetHealth() <= playerHealth.GetMaxHealth()) {
                Debug.Log("healing");
                playerHealth.Heal(playerHealth.GetMaxHealth() * medkitHealPercentage / 100);
                Destroy(collider.gameObject);
            }
        }

    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(medkitPickUpSphere.transform.position, medkitPickUpRadius);
    }

    public void PickUpGun(Transform gunTransform) {
        if (this.GetComponent<PlayerController>().lastGunStats != null) {
            this.GetComponent<PlayerController>().lastGunStats.SetActive(false);
        }
        Rigidbody gunRigidbody = gunTransform.GetComponent<Rigidbody>();

        gunTransform.GetComponent<GunData>().reloadSound.Play();
        if (holdingPrimary == true) {
            holder = primaryholder;
            primary = gunTransform;
        } else if (holdingSecondary == true) {
            holder = secondaryHolder;
            secondary = gunTransform;
        }
        Debug.Log("snot aap");
        gunRigidbody.useGravity = false;
        gunTransform.SetParent(holder);
        gunTransform.position = holder.position;
        gunTransform.rotation = holder.rotation;
        gunTransform.GetComponent<MeshCollider>().enabled = false;
        gunTransform.GetComponent<GunData>().reloadSound.Play();
        gunRigidbody.freezeRotation = true;
        gunRigidbody.isKinematic = true;
        pickUpDelay = false;
        gunTransform.GetComponent<GunData>().lootBeam.SetActive(false);
        cam.GetComponent<GunScript>().updateAmmoCount();
        gunicon.texture = gunTransform.GetComponent<GunData>().icon;
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

        gunRigidbody.GetComponent<GunData>().lootBeam.SetActive(true);

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
