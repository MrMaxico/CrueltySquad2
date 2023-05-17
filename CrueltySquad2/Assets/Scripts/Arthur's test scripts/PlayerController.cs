using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    [Header("changeable variables", order = 0)]
    public int speed;
    public int sprintSpeed;
    public int jumpPower;
    public bool infJump;
    [Header("Debug variables", order = 1)]
    public float cameraPitch = 0.0f;
    public bool canJump;
    [Header("Config variables", order = 2)]
    public int camSensitivity;
    public Transform cam;
    public Rigidbody rb;


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
        //Movement
        Vector3 move = new Vector3();
        Vector3 rotateBody = new Vector3();
        Vector3 rotateCam = new Vector3();

        float v = new float();
        float h = new float();
        float j = new float();

        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        

        move.x = h;
        move.z = v;
        if (Input.GetKey(KeyCode.LeftShift)) {
            transform.Translate(move * Time.deltaTime * sprintSpeed);
        } else {
            transform.Translate(move * Time.deltaTime * speed);
        }

        //Jump
        Vector3 jump = new Vector3();
        if (canJump || infJump) {
            if (Input.GetKeyUp("space")) {
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
    }



}