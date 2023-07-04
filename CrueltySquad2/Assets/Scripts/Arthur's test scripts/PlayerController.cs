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
    private Vector3 moveDirection;
    private bool exitingSlope;

    // Update is called once per frame
    private void Start() {
        cameraPitch = 0f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Ground") {
            onGround = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Ground") {
            onGround = false;
        }
    }
    void Update() {
        if (!PauzeScript.gameIsPaused) {
            Vector3 rotateBody = new Vector3();
            Vector3 rotateCam = new Vector3();

            //Jump
            Vector3 jump = new Vector3();
            if (isGrounded || infJump) {
                if (Input.GetKeyDown("space")) {
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
                pickUpController.gunicon.enabled = true;
                gunScript.currentGunData.reloadSound.Play();
                pickUpController.gunicon.texture = pickUpController.primary.GetComponent<GunData>().icon;
                gunScript.updateAmmoCount();
            } else if (Input.GetKeyDown(KeyCode.Alpha2) && !gunScript.reloading) {
                pickUpController.primaryholder.gameObject.SetActive(false);
                pickUpController.secondaryHolder.gameObject.SetActive(true);
                pickUpController.holdingPrimary = false;
                pickUpController.holdingSecondary = true;
                gunScript.currentGunData.reloadSound.Play();
                pickUpController.gunicon.enabled = true;
                if (pickUpController.secondary == null) {
                    pickUpController.gunicon.enabled = false;
                }
                pickUpController.gunicon.texture = pickUpController.secondary.GetComponent<GunData>().icon;
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
        enemyHealthBarLvl.text = "Lvl. " + Teleporter.islandNumber.ToString();
        enemyHealthBar.value = hit.transform.GetComponent<Health>().GetHealth() / hit.transform.GetComponent<Health>().GetMaxHealth();
        enemyHealthBarName.text = hit.transform.GetComponent<EnemyStats>().name;
    }
    private void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f);
        //movement
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
        MovePlayer();
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
        // limiting speed on slope
        if (OnSlope() && !exitingSlope) {
            if (rb.velocity.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;
        }
        rb.AddForce(Vector3.down * 19f, ForceMode.Acceleration);
    }

    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

}