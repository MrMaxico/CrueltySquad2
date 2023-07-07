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
    public float jumpPower;
    public bool infJump;
    public float swimRotationSpeed;
    public float swimSpeed;
    public float swimUpSpeed = 1;
    public float swimGravity;
    [Header("Debug variables", order = 1)]
    public float cameraPitch = 0.0f;
    public bool onGround;
    public bool enemyHealthBarActive;
    public Slider enemyHealthBar;
    public TextMeshProUGUI enemyHealthBarName;
    public TextMeshProUGUI enemyHealthBarLvl;
    public Animator enemyHealthBarAnimator;
    private Animator weaponStatsAnimator;
    public float timer;
    private float gunStatsTimer;
    public GameObject lastGunStats;
    private Vector3 movement;
    public bool inWater;
    [Header("Config variables", order = 2)]
    public int camSensitivity;
    public Transform cam;
    public Rigidbody rb;
    public PickUpController pickUpController;
    public GunScript gunScript;
    public GameObject pickUpUI;
    private bool raycastHit;
    public AudioSource walkingSound;
    public bool isGrounded;
    public float playerHeight;
    private RaycastHit slopeHit;
    public float maxSlopeAngle;
    public GameObject uiSlot1;
    public GameObject uiSlot2;
    public GameObject swimmingUI;

    // Update is called once per frame
    private void Start() {
        cameraPitch = 0f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Ground") {
            onGround = true;
        }
        if(other.gameObject.tag == "Water") {
            rb.velocity = new Vector3(0, 0, 0);
            inWater = true;
            swimmingUI.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Ground") {
            onGround = false;
        }
        if (other.gameObject.tag == "Water") {
            rb.velocity = new Vector3(0, 0, 0);
            inWater = false;
            swimmingUI.SetActive(false);
        }
    }
    void Update() {
        if (!PauzeScript.gameIsPaused) {
            Vector3 rotateBody = new Vector3();
            Vector3 rotateCam = new Vector3();

            //Jump
            Vector3 jump = new Vector3();
            if (isGrounded || infJump) {
                if (Input.GetKeyDown("space") && !inWater) {
                    jump.y = jumpPower * 5;
                    rb.AddForce(jump, ForceMode.Impulse);
                    Debug.Log("jumped");
                    onGround = false;
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
                gunScript.ammoCount.enabled = true;
                uiSlot2.SetActive(false);
                uiSlot1.SetActive(true);
                gunScript.currentGunData.reloadSound.Play();
                gunScript.updateAmmoCount();
            } else if (Input.GetKeyDown(KeyCode.Alpha2) && !gunScript.reloading) {
                pickUpController.primaryholder.gameObject.SetActive(false);
                pickUpController.secondaryHolder.gameObject.SetActive(true);
                pickUpController.holdingPrimary = false;
                pickUpController.holdingSecondary = true;
                gunScript.ammoCount.enabled = true;
                uiSlot2.SetActive(true);
                uiSlot1.SetActive(false);
                gunScript.currentGunData.reloadSound.Play();
                gunScript.updateAmmoCount();
            }
            var yVel = rb.velocity.y;
            var localV = transform.InverseTransformDirection(rb.velocity);
            localV.x = 0.0f;
            localV.y = 0.0f;
            var worldV = transform.TransformDirection(localV);
            worldV.y = yVel;
            rb.velocity = worldV;
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
            if (Input.GetKey(KeyCode.G)) {
                if (!GetComponent<Health>().godMode) {
                    GetComponent<Health>().godMode = true;
                } else {
                    GetComponent<Health>().godMode = false;
                }
            }
        }
        gunStatsTimer += Time.deltaTime; // Increase the timer by the elapsed time
        if (gunStatsTimer >= 0.5) {
            // Perform your raycast here
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2)) {
                // Raycast hit something, do something with the hit information
                if (hit.transform.CompareTag("Gun")) {
                    pickUpUI.SetActive(true);
                    lastGunStats = hit.transform.GetComponent<GunData>().gunStatUI.transform.parent.parent.gameObject;
                    hit.transform.GetComponent<GunData>().gunStatUI.transform.parent.parent.gameObject.SetActive(true);
                    hit.transform.GetComponent<GunData>().gunStatUI.transform.parent.GetComponent<Animator>().Play("weapon stats popup");
                } else {
                    pickUpUI.SetActive(false);
                    if (lastGunStats != null) {
                        lastGunStats.SetActive(false);
                    }
                }
            }
            gunStatsTimer = 0f; // Reset the timer
        }
    }
    public void UpdateEnemyHealthBar(RaycastHit hit) {
        Debug.Log("Updating Enemy Heath bar");
        enemyHealthBarAnimator.SetBool("isActive", true);
        Debug.Log(hit.transform.GetComponent<Health>().GetHealth() / hit.transform.GetComponent<Health>().GetMaxHealth());
        enemyHealthBarLvl.text = "Lvl. " + Teleporter.islandNumber.ToString();
        enemyHealthBar.value = hit.transform.GetComponent<Health>().GetHealth() / hit.transform.GetComponent<Health>().GetMaxHealth();
        enemyHealthBarName.text = hit.transform.GetComponent<EnemyStats>().name;
    }
    private void FixedUpdate()
    {
        if (inWater) {
            Swim();
        } else {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f)) {
                if (!hit.transform.CompareTag("Water")) {
                    isGrounded = true;
                }
            } else {
                isGrounded = false;
            }
            //movement
            movement.x = Input.GetAxis("Horizontal");
            movement.z = Input.GetAxis("Vertical");
            MovePlayer();
        }
    }

    private void Swim() {
        // Get input for movement and rotation
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float swimUp = Input.GetKey(KeyCode.Space) ? swimUpSpeed : -swimGravity;

        // Calculate movement and rotation vectors
        Vector3 movement = new Vector3(moveHorizontal, swimUp, moveVertical) * swimSpeed * Time.deltaTime;
        // Apply movement and rotation to the rigidbody
        rb.MovePosition(rb.position + transform.TransformDirection(movement));
        GetComponent<Health>().Damage(GetComponent<Health>().GetMaxHealth() / 200);
    }

    private void MovePlayer() {
        Vector3 moveVector = new Vector3();
        if (Input.GetKey(KeyCode.LeftShift)) {
            moveVector = transform.TransformDirection(movement) * sprintSpeed * Time.deltaTime;
        } else {
            moveVector = transform.TransformDirection(movement) * speed * Time.deltaTime;
            //walkingSound.enabled = true;
        }

        if (isGrounded) {
            rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
        } else {
            // Check for obstacles in front of the player
            float radius = 0.3f; // Adjust the radius as needed
            float distance = moveVector.magnitude;
            Vector3 direction = moveVector.normalized;
            RaycastHit hit;

            if (Physics.SphereCast(transform.position, radius, direction, out hit, distance)) {
                // If an obstacle is detected, adjust the movement vector
                moveVector = direction * (hit.distance - radius);
            }

            rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
        }
        rb.AddForce(Vector3.down * 19f, ForceMode.Acceleration);
    }
}