using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour {
    [Header("changeable variables", order = 0)]
    public int speed;
    public int sprintSpeed;
    public int jumpPower;
    public bool infJump;
    [Header("Debug variables", order = 1)]
    public float cameraPitch = 0.0f;
    public bool canJump;
    public bool enemyHealthBarActive;
    public Slider enemyHealthBar;
    public TextMeshProUGUI enemyHealthBarName;
    public Animator enemyHealthBarAnimator;
    private Animator weaponStatsAnimator;
    public float timer;
    private float gunStatsTimer;
    public GameObject lastGunStats;
    private Vector3 movement;
    [Header("Config variables", order = 2)]
    public int camSensitivity;
    public Transform cam;
    public Rigidbody rb;
    public PickUpController pickUpController;
    public GunScript gunScript;
    public GameObject pickUpUI;
    private bool raycastHit;
    public AudioSource walkingSound;
    public PauzeScript pauzeScript;


    // Update is called once per frame
    private void Start() {
        cameraPitch = 0f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag == "Ground") {
            canJump = true;
        }
    }
    public void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Ground") {
            canJump = false;
        }
    }
    void Update() {
        if (!PauzeScript.gameIsPaused) {
            Vector3 rotateBody = new Vector3();
            Vector3 rotateCam = new Vector3();

            //Jump
            Vector3 jump = new Vector3();
            if (canJump || infJump) {
                if (Input.GetKeyDown("space")) {
                    jump.y = jumpPower * 5;
                    rb.AddForce(jump, ForceMode.Impulse);
                    Debug.Log("jumped");
                    canJump = false;
                }
            }
            float mouseX = new float();
            float mouseY = new float();

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            rotateCam.x = mouseY;
            rotateBody.y = mouseX;
            transform.Rotate(rotateBody * camSensitivity);

            cameraPitch -= mouseY * camSensitivity;

            cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);


            cam.localEulerAngles = new Vector3(cameraPitch, 0, 0);

            if (Input.GetKeyDown(KeyCode.Alpha1) && !gunScript.reloading) {
                pickUpController.secondaryHolder.gameObject.SetActive(false);
                pickUpController.primaryholder.gameObject.SetActive(true);
                pickUpController.holdingSecondary = false;
                pickUpController.holdingPrimary = true;
                gunScript.currentGunData.reloadSound.Play();
                gunScript.updateAmmoCount();
            } else if (Input.GetKeyDown(KeyCode.Alpha2) && !gunScript.reloading) {
                pickUpController.primaryholder.gameObject.SetActive(false);
                pickUpController.secondaryHolder.gameObject.SetActive(true);
                pickUpController.holdingPrimary = false;
                pickUpController.holdingSecondary = true;
                gunScript.currentGunData.reloadSound.Play();
                gunScript.updateAmmoCount();
            }
        }
        if (enemyHealthBarActive) {
            timer += Time.deltaTime; // Increase the timer by the elapsed time

            if (timer >= 0.5) {
                // Perform your raycast here
                Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000)) {
                    raycastHit = true;
                    // Raycast hit something, do something with the hit information
                    if (hit.transform.CompareTag("Enemy")) {
                        UpdateEnemyHealthBar(hit);
                    } else {
                        enemyHealthBarAnimator.SetBool("isActive", false);
                    }
                }
                if (!raycastHit) {
                    enemyHealthBarAnimator.SetBool("isActive", false);
                }
                raycastHit = false;
                timer = 0f; // Reset the timer
            }
        }
        gunStatsTimer += Time.deltaTime; // Increase the timer by the elapsed time
        if (gunStatsTimer >= 0.5) {
            // Perform your raycast here
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000)) {
                // Raycast hit something, do something with the hit information
                if (hit.transform.CompareTag("Gun")) {
                    lastGunStats = hit.transform.GetComponent<GunData>().gunStatUI.transform.parent.parent.gameObject;
                    hit.transform.GetComponent<GunData>().gunStatUI.transform.parent.parent.gameObject.SetActive(true);
                    hit.transform.GetComponent<GunData>().gunStatUI.transform.parent.GetComponent<Animator>().Play("weapon stats popup");
                } else {
                    lastGunStats.SetActive(false);
                }
            }

            gunStatsTimer = 0f; // Reset the timer
        }

    }
    public void UpdateEnemyHealthBar(RaycastHit hit) {
        Debug.Log("Updating Enemy Heath bar");
        enemyHealthBarAnimator.SetBool("isActive", true);
        Debug.Log(hit.transform.GetComponent<Health>().GetHealth() / hit.transform.GetComponent<Health>().GetMaxHealth());
        enemyHealthBar.value = hit.transform.GetComponent<Health>().GetHealth() / hit.transform.GetComponent<Health>().GetMaxHealth();
        enemyHealthBarName.text = hit.transform.GetComponent<Enemy>().enemyType.ToString();
    }
    private void FixedUpdate()
    {
        //movement
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 moveVector = new Vector3();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveVector = transform.TransformDirection(movement) * sprintSpeed * Time.deltaTime;
        }
        else
        {
            moveVector = transform.TransformDirection(movement) * speed * Time.deltaTime;
           //walkingSound.enabled = true;
        }
        rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
    }


}