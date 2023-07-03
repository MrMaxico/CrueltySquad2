using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum GunType {
    Pistol,
    Rifle,
    Shotgun,
    // Add more gun types as needed
}

public class GunData : MonoBehaviour {
    public string gunName;
    public Texture icon;
    public float damagePerBullet;
    public float magSize = 30f;
    public float currentAmmo = 30f;
    public float fireRate = 0.5f;
    public float bloom = 0.05f;
    public float range = 100;
    public float reloadspeed = 1f;
    public GunType gunType = GunType.Pistol;
    public GameObject gunStatUI;
    public GameObject lootBeam;
    public AudioSource shootingSound;
    public AudioSource reloadSound;
    public ParticleSystem muzzleFlash;
    [Header("Settings for shotguns", order = 0)]
    public float shotgunSpreadAngle = 15f;
    public float shotgunPelletCount = 15f;
    // Other gun-related variables and functions can be added here
    public void Start() {
        for (int i = 0; i < Teleporter.islandNumber - 1; i++) {
            damagePerBullet += 0.2f * damagePerBullet;
        }
        gunStatUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gunName.ToString(); //Weapon name
        gunStatUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Teleporter.islandNumber.ToString(); //make Level of gun
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
