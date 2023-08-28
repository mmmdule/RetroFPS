using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();

        //un-pause at start
        paused = false;
        Time.timeScale = 1;
        ShootScript.enabled = true;
        AudioListener.pause = false;
        canvasPause.enabled = false;
        canvasPlay.enabled = true;
        //un-pause at start

        //Cursor.lockState = CursorLockMode.None;
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public static bool paused = false;
    [SerializeField]
    Canvas canvasPause;
    [SerializeField]
    Canvas canvasPlay;
    [SerializeField]
    Shoot ShootScript;
    void Update()
    {
        if(!paused && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))){
            paused = true;
            Time.timeScale = 0;
            ShootScript.enabled = false;
            AudioListener.pause = true;
            canvasPause.enabled = true;
            canvasPlay.enabled = false;
        }
        else if(paused && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))){
            paused = false;
            Time.timeScale = 1;
            ShootScript.enabled = true;
            AudioListener.pause = false;
            canvasPause.enabled = false;
            canvasPlay.enabled = true;
        }
        else if (paused && (Input.GetKeyDown(KeyCode.R))){
            PlayerPrefs.SetString("LevelToLoad", PlayerPrefs.GetString("GameOverLevel"));
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (paused && (Input.GetKeyDown(KeyCode.Q))){
            SceneManager.LoadScene(0);
        }
        else if(paused)
            return;
        
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)){
            //transform.rotation *= Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z), 3f * Time.deltaTime);
            transform.rotation *= Quaternion.Euler(0, transform.rotation.y + 180, 0);
        }
        if (Input.GetKeyDown(KeyCode.T)){
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0.00f, transform.rotation.z);
        }
        
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("KeyBoardCamAxis") * lookSpeed, 0);
        }
    }
}
