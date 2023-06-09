using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GunType {
    Pistol,
    Rifle,
    Shotgun,
    RocketLauncher,
    // Add more gun types as needed
}

public class GunData : MonoBehaviour {
    public string gunName;
    public Texture icon;
    public float damagePerBullet;
    public float magSize = 30f;
    public float currentAmmo = 30f;
    public float fireRate = 0.5f;
    public float maxBloom = 0.05f;
    public float range = 100;
    public float reloadspeed = 1f;
    public GunType gunType = GunType.Pistol;
    public GameObject gunStatUI;
    public GameObject lootBeam;
    public AudioSource shootingSound;
    public float maxPitch = 0.8f;
    public float minPitch = 1.0f;
    public AudioSource reloadSound;
    public ParticleSystem muzzleFlash;
    public int ogislandNummer;
    [Header("Settings for shotguns", order = 0)]
    public float shotgunSpreadAngle = 15f;
    public float shotgunPelletCount = 15f;
    [Header("Settings for Rocket Luancher", order = 1)]
    public GameObject rocket;
    public GameObject rocketExplodeAnim;
    public float rocketSpeed;
    public float explosionForce;
    public float explosionRadius;

    // Other gun-related variables and functions can be added here
    public void Start() {
        ogislandNummer = Teleporter.islandNumber;
        ApplyGunData(ogislandNummer);
    }
    public void ApplyGunData(int islandNummer) {
        ogislandNummer = islandNummer;
        if (gameObject != GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>().newWeapons[0] && gameObject != GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>().newWeapons[1]) {
            for (int i = 0; i < islandNummer - 1; i++) {
                damagePerBullet += 0.2f * damagePerBullet;
                Debug.Log("kaas");
            }
        }
        gunStatUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gunName.ToString(); //Weapon name
        gunStatUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Lvl. " + islandNummer.ToString(); //make Level of gun
        Debug.Log("kes");
        gunStatUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = " "; //make modifier name
        if (gunType == GunType.Shotgun) {
            int result = 5 * 15;
            gunStatUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = result.ToString();
        }
        gunStatUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = damagePerBullet.ToString(); //damage
        gunStatUI.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = reloadspeed.ToString(); //reload speed
        gunStatUI.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = magSize.ToString(); //magSize
        gunStatUI.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = fireRate.ToString(); //FireRate
        gunStatUI.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = range.ToString(); //Range
    }
}
