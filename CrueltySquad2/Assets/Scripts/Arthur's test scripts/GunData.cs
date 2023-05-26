using UnityEngine;

public enum GunType {
    Pistol,
    Rifle,
    Shotgun,
    // Add more gun types as needed
}

public class GunData : MonoBehaviour {
    public float magSize = 30f;
    public float currentAmmo = 30f;
    public float fireRate = 0.5f;
    public float bloom = 0.05f;
    public float reloadspeed = 1f;
    public GunType gunType = GunType.Pistol;
    [Header("Settings for shotguns", order = 0)]
    public float shotgunSpreadAngle = 15f;
    public float shotgunPelletCount = 15f;
    // Other gun-related variables and functions can be added here
}
