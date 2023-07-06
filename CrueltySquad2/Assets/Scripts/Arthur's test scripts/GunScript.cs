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
    public Animator melee;
    private bool firedRocket;
    private GameObject rocket;
    private bool rocketExploding;

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
                    shot = true;
                    Fire();
                } else if(currentGunData.gunType == GunType.Shotgun && !shot) {
                    shot = true;
                    FireShotgun();
                } else if(currentGunData.gunType == GunType.Rifle) {
                    Fire();
                } else if (currentGunData.gunType == GunType.RocketLauncher && !shot) {
                    shot = true;
                    FireRocketLauncher();
                }
            } else if(Input.GetMouseButton(0) && currentGunData.currentAmmo <= 0 && !reloading) {
                StartCoroutine(Reload());
            }
            if (Input.GetKeyDown(KeyCode.R) && !reloading) {
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
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= nextMeleeTime) {
            nextMeleeTime = Time.time + 0.4f;
            if (!shot) {
                Debug.Log("melee");
                Melee();
            }
        }
        if (Input.GetMouseButtonUp(0) && shot == true) {
            shot = false;
        }
        if (firedRocket) {
            if (!rocketExploding) {
                rocket.GetComponent<Rigidbody>().velocity = rocket.transform.forward * currentGunData.rocketSpeed;
            } else {
                rocket.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            // Collect nearby colliders within the explosion radius
            Collider[] colliders = Physics.OverlapSphere(rocket.transform.position, 0.1f);

            foreach (Collider nearbyObject in colliders) {
                // Apply explosion force to rigidbodies
                if (!nearbyObject.CompareTag("Gun") && !nearbyObject.CompareTag("Player")) {
                    rocketExploding = true;
                    GameObject.Instantiate(currentGunData.rocketExplodeAnim, rocket.transform.position, rocket.transform.rotation);
                    Collider[] hits = Physics.OverlapSphere(rocket.transform.position, currentGunData.explosionRadius);
                    foreach (Collider hit in hits) {
                        Debug.Log(hit.name + " hit by explosion");
                        if (hit.TryGetComponent<Rigidbody>(out Rigidbody hitRB)) {
                            hitRB.AddExplosionForce(currentGunData.explosionForce, rocket.transform.position, currentGunData.explosionRadius);
                        }
                        if (hit.TryGetComponent<Health>(out Health hitHP)) {
                            hitHP.GetComponent<Health>().Damage(currentGunData.damagePerBullet);
                        }
                    }
                    Destroy(rocket);
                    firedRocket = false;
                    rocketExploding = false;
                }
            }
        }

    }
    public void Melee() {
        melee.SetTrigger("IsPunching");
        // Raycast from muzzle position
        Ray ray = new Ray(muzzle.position, muzzle.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1.5f, hitLayers)) {
            // Perform hit detection and damage logic here
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            if (hit.transform.CompareTag("Enemy") || hit.transform.TryGetComponent<Health>(out Health hitHP) && hitHP.healthType == HealthType.prop || hit.transform.CompareTag("Spawner")) {
                if (hit.transform.GetComponent<Health>().GetHealth() <= playerStats.meleeDamage && !hit.transform.CompareTag("Destroyable")) {
                    hit.transform.GetComponent<Health>().EnemyDeath();
                    playerStats.AddExp(hit.transform.GetComponent<Health>().xpOnDeath);

                }
                hit.transform.GetComponent<Health>().Damage(playerStats.meleeDamage);
            }
            if (hit.transform.CompareTag("Spawner") || hit.transform.CompareTag("Enemy")) {
                playerController.UpdateEnemyHealthBar(hit);
            }
        }

    }
    private void Fire() {
        currentGunData.currentAmmo -= 1;
        updateAmmoCount();
        // Add random rotation for bloom effect
        Vector3 rayDirection = Quaternion.Euler(0.0f, Random.Range(-currentGunData.maxBloom, currentGunData.maxBloom), Random.Range(-currentGunData.maxBloom, currentGunData.maxBloom)) * muzzle.forward;
        if (!reloading) {
            currentGunData.shootingSound.Play();
            Debug.Log(currentGunData.muzzleFlash.name);
            currentGunData.muzzleFlash.Play();
        }
        // Raycast from muzzle position
        Ray ray = new Ray(muzzle.position, rayDirection);
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
                if (hit.transform.CompareTag("Enemy") || hit.transform.TryGetComponent<Health>(out Health hitHP) && hitHP.healthType == HealthType.prop) {
                    Debug.Log("Kont");
                    if (hit.transform.GetComponent<Health>().GetHealth() <= currentGunData.damagePerBullet && !hit.transform.CompareTag("Destroyable") && hit.collider != lastHit.collider) {
                        if (hit.collider != lastHit.collider) {
                            hit.transform.GetComponent<Health>().EnemyDeath();
                            playerStats.AddExp(hit.transform.GetComponent<Health>().xpOnDeath);
                        }
                    }
                    lastHit = hit;
                    if (hit.transform.CompareTag("Enemy")) {
                        Instantiate(enemyHitParticlePrefab, hit.point, Quaternion.identity);
                    } else {
                        Instantiate(hitParticlePrefab, hit.point, Quaternion.identity);
                    }
                    hit.transform.GetComponent<Health>().Damage(currentGunData.damagePerBullet);
                    if (hit.transform.TryGetComponent<EnemyStats>(out EnemyStats enemyStats) && enemyStats.name == "Spawner" || hit.transform.CompareTag("Enemy")) {
                        playerController.UpdateEnemyHealthBar(hit);
                    }
                } else {
                    Instantiate(hitParticlePrefab, hit.point, Quaternion.identity);
                }
            }
            // Draw a debug line to visualize the raycast
            Debug.DrawRay(ray.origin, ray.direction * currentGunData.range, Color.red, 0.1f);
        }

    }
    private void FireRocketLauncher() {
        currentGunData.currentAmmo -= 1;
        rocket = Instantiate(currentGunData.rocket, currentGunData.transform.position, currentGunData.transform.rotation);
        firedRocket = true;
    }
    public void DamageShotEnemy(RaycastHit hit) {
        if (hit.transform.CompareTag("Enemy") || hit.transform.TryGetComponent<Health>(out Health hitHP) && hitHP.healthType == HealthType.prop) {
            if (currentGunData != null) {
                if (hit.transform.GetComponent<Health>().GetHealth() <= currentGunData.damagePerBullet && hit.transform.GetComponent<Health>().healthType != HealthType.prop) {
                    hit.transform.GetComponent<Health>().EnemyDeath();
                    playerStats.AddExp(hit.transform.GetComponent<Health>().xpOnDeath);
                }
                hit.transform.GetComponent<Health>().Damage(currentGunData.damagePerBullet);
            }
            if (hit.transform.GetComponent<EnemyStats>().name == "Spawner" || hit.transform.CompareTag("Enemy")) {
                playerController.UpdateEnemyHealthBar(hit);
            }
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
        reloading = true;
        currentGunData.reloadSound.Play();
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
