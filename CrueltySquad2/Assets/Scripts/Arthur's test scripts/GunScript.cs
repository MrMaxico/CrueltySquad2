using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunScript : MonoBehaviour
{
    public Transform muzzle;
    public LayerMask hitLayers;
    public GunData currentGunData;
    private float nextFireTime;
    private float nextMeleeTime;
    private Vector3 originalRotation;
    public PickUpController pickUpController;
    public PlayerController playerController;
    public PlayerStats playerStats;
    public TextMeshProUGUI ammoCount;
    public bool shot;
    public bool reloading;
    public GameObject reloadAnim;
    public List<RaycastHit> lastHits = new List<RaycastHit>();
    public GameObject hitParticlePrefab;
    public GameObject enemyHitParticlePrefab;
    public RaycastHit lastHit;
    public float meleeDamage;
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
        } else {
            if (Input.GetMouseButton(0) && Time.time >= nextMeleeTime) {
                nextMeleeTime = Time.time + 0.4f;
                Debug.Log("melee");
                if (!shot) {
                    Melee();
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && shot == true) {
            shot = false;
        }

    }
    public void Melee() {
        // Raycast from muzzle position
        Ray ray = new Ray(muzzle.position, muzzle.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200, hitLayers)) {
            // Perform hit detection and damage logic here
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            DamageShotEnemy(hit);
        }

    }
    private void Fire() {
        currentGunData.currentAmmo -= 1;
        updateAmmoCount();
        // Add random rotation for bloom effect
        Vector3 bloomRotation = Random.insideUnitCircle * currentGunData.bloom;
        Quaternion rotation = Quaternion.Euler(originalRotation + bloomRotation);
        if (!reloading) {
            currentGunData.shootingSound.Play();
            Debug.Log(currentGunData.muzzleFlash.name);
            currentGunData.muzzleFlash.Play();
        }
        // Raycast from muzzle position
        Ray ray = new Ray(muzzle.position, muzzle.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, currentGunData.range, hitLayers)) {
            // Perform hit detection and damage logic here
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            if (hit.transform.CompareTag("Enemy")) {
                Instantiate(enemyHitParticlePrefab, hit.point, Quaternion.identity);
            } else {
                Instantiate(hitParticlePrefab, hit.point, Quaternion.identity);
            }
            DamageShotEnemy(hit);
        }

        // Draw a debug line to visualize the raycast
        Debug.DrawRay(ray.origin, ray.direction * currentGunData.range, Color.red, 0.1f);
    }
    private void FireShotgun() {
        currentGunData.currentAmmo -= 1;
        updateAmmoCount();
        // Iterate over the number of shotgun pellets
        for (int i = 0; i < currentGunData.shotgunPelletCount; i++) {
            // Calculate a random direction for each pellet within the shotgun spread angle
            Vector3 pelletDirection = Quaternion.Euler(Random.Range(-currentGunData.shotgunSpreadAngle, currentGunData.shotgunSpreadAngle), Random.Range(-currentGunData.shotgunSpreadAngle, currentGunData.shotgunSpreadAngle), 0f) * muzzle.forward;

            // Raycast from muzzle position
            Ray ray = new Ray(muzzle.position, pelletDirection);
            RaycastHit hit;
            if (!reloading) {
                currentGunData.shootingSound.Play();
                currentGunData.muzzleFlash.Play();
            }
            if (Physics.Raycast(ray, out hit, currentGunData.range, hitLayers)) {
                // Perform hit detection and damage logic here
                Debug.Log("Hit: " + hit.collider.gameObject.name + "With: " + currentGunData.gunType);
                if (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Destroyable")) {
                    if (hit.transform.GetComponent<Health>().GetHealth() <= currentGunData.damagePerBullet && !hit.transform.CompareTag("Destroyable") && hit.collider != lastHit.collider) {
                        if (hit.collider != lastHit.collider) {
                            hit.transform.GetComponent<Health>().EnemyDeath();
                            playerStats.AddExp(hit.transform.GetComponent<Health>().xpOnDeath);
                        }
                    }
                    lastHit = hit;
                    if (hit.transform.CompareTag("Enemy")) {
                        Instantiate(enemyHitParticlePrefab, hit.point, Quaternion.identity);
                    }
                    hit.transform.GetComponent<Health>().Damage(currentGunData.damagePerBullet);
                } else {
                    Instantiate(hitParticlePrefab, hit.point, Quaternion.identity);
                }
            }
            // Draw a debug line to visualize the raycast
            Debug.DrawRay(ray.origin, ray.direction * currentGunData.range, Color.red, 0.1f);
        }

    }
    public void DamageShotEnemy(RaycastHit hit) {
        if (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Destroyable")) {

            if(null == currentGunData && hit.transform.GetComponent<Health>().GetHealth() <= playerStats.meleeDamage && !hit.transform.CompareTag("Destroyable") || hit.transform.GetComponent<Health>().GetHealth() <= currentGunData.damagePerBullet && !hit.transform.CompareTag("Destroyable")) {
                hit.transform.GetComponent<Health>().EnemyDeath();
                playerStats.AddExp(hit.transform.GetComponent<Health>().xpOnDeath);
            }
            if(currentGunData == null) {
                hit.transform.GetComponent<Health>().Damage(playerStats.meleeDamage);
            } else {
                hit.transform.GetComponent<Health>().Damage(currentGunData.damagePerBullet);
            }
            playerController.UpdateEnemyHealthBar(hit);
        }
    }
    public void updateAmmoCount() {
        if (pickUpController.holdingPrimary) {
            ammoCount.text = pickUpController.primary.GetComponent<GunData>().currentAmmo.ToString();
        } else if (pickUpController.holdingSecondary){
            ammoCount.text = pickUpController.secondary.GetComponent<GunData>().currentAmmo.ToString();
        }
    }

    private IEnumerator Reload() {
        currentGunData.reloadSound.Play();
        reloading = true;
        ammoCount.enabled = false;
        reloadAnim.SetActive(true);
        yield return new WaitForSeconds(currentGunData.reloadspeed);
        currentGunData.currentAmmo = currentGunData.magSize;
        reloadAnim.SetActive(false);
        ammoCount.enabled = true;
        updateAmmoCount();
        reloading = false;
    }
}
