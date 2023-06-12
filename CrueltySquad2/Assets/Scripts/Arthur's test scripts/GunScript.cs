using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform muzzle;
    public LayerMask hitLayers;
    public GunData currentGunData;
    private float nextFireTime;
    private Vector3 originalRotation;
    public PickUpController pickUpController;
    public PlayerController playerController;
    public PlayerStats playerStats;
    public bool shot;
    public bool reloading;
    private void Start() {
        originalRotation = transform.localRotation.eulerAngles;
    }

    private void Update() {
        if(pickUpController.holdingSecondary && pickUpController.secondary != null) {
            currentGunData = pickUpController.secondary.GetComponent<GunData>();
        }else if(pickUpController.holdingPrimary && pickUpController.primary != null) {
            currentGunData = pickUpController.primary.GetComponent<GunData>();
        } else {
            currentGunData = null;
        }
        if (currentGunData != null) {
            if (Input.GetMouseButton(0) && Time.time >= nextFireTime && currentGunData.currentAmmo >= 0 && !reloading) {
                nextFireTime = Time.time + currentGunData.fireRate;
                if (currentGunData.gunType == GunType.Pistol && !shot) {
                    Fire();
                    shot = true;
                } else if(currentGunData.gunType == GunType.Shotgun && !shot) {
                    FireShotgun();
                    shot = true;
                } else if(currentGunData.gunType == GunType.Rifle) {
                    Fire();
                }
            }else if(Input.GetMouseButton(0) && currentGunData.currentAmmo <= 0) {
                StartCoroutine(Reload());
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                StartCoroutine(Reload());
            }
        }
        if (Input.GetMouseButtonUp(0) && shot == true) {
            shot = false;
        }

    }

    private void Fire() {
        currentGunData.currentAmmo -= 1;
        // Add random rotation for bloom effect
        Vector3 bloomRotation = Random.insideUnitCircle * currentGunData.bloom;
        Quaternion rotation = Quaternion.Euler(originalRotation + bloomRotation);

        // Raycast from muzzle position
        Ray ray = new Ray(muzzle.position, muzzle.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, currentGunData.range, hitLayers)) {
            // Perform hit detection and damage logic here
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            DamageShotEnemy(hit);
        }

        // Draw a debug line to visualize the raycast
        Debug.DrawRay(ray.origin, ray.direction * currentGunData.range, Color.red, 0.1f);
    }
    private void FireShotgun() {
        currentGunData.currentAmmo -= 1;
        // Iterate over the number of shotgun pellets
        for (int i = 0; i < currentGunData.shotgunPelletCount; i++) {
            // Calculate a random direction for each pellet within the shotgun spread angle
            Vector3 pelletDirection = Quaternion.Euler(Random.Range(-currentGunData.shotgunSpreadAngle, currentGunData.shotgunSpreadAngle), Random.Range(-currentGunData.shotgunSpreadAngle, currentGunData.shotgunSpreadAngle), 0f) * muzzle.forward;

            // Raycast from muzzle position
            Ray ray = new Ray(muzzle.position, pelletDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, currentGunData.range, hitLayers)) {
                // Perform hit detection and damage logic here
                Debug.Log("Hit: " + hit.collider.gameObject.name + "With: " + currentGunData.gunType);
                DamageShotEnemy(hit);
            }

            // Draw a debug line to visualize the raycast
            Debug.DrawRay(ray.origin, ray.direction * currentGunData.range, Color.red, 0.1f);
        }
    }
    public void DamageShotEnemy(RaycastHit hit) {
        if (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Destroyable")) {
            if(hit.transform.GetComponent<Health>().GetHealth() <= currentGunData.damagePerBullet && !hit.transform.CompareTag("Destroyable")) {
                hit.transform.GetComponent<Health>().EnemyDeath();
                playerStats.AddExp(hit.transform.GetComponent<Health>().xpOnDeath);
            }
            hit.transform.GetComponent<Health>().Damage(currentGunData.damagePerBullet);
            playerController.UpdateEnemyHealthBar(hit);
        }
    }
    private IEnumerator Reload() {
        reloading = true;
        yield return new WaitForSeconds(currentGunData.reloadspeed);
        currentGunData.currentAmmo = currentGunData.magSize;
        reloading = false;
    }
}
