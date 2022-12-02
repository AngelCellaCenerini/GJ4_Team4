using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

// ------ PERCY TO HELP YOU NAVIGATE FOR SOUND -------
// Control F this "SEARCH" and It'll bring you to the key areas

public class PlayerMovement : MonoBehaviour
{
    // Reference
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    int isWalkingHash;
    int isRunningHash;

    // Movement
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    // States
    bool isMovementPressed;
    bool isRunPressed;
    bool isInteractPressed;
    string interacting = "none";
    // Multipliers
    float walkSpeed = 5.0f;
    float runMultiplier = 3.0f;
    float rotationFractorPerFrame = 1.0f;
    float initialJumpVelocity;
    // Rotation
    float rotationFactorPerFrame = 15.0f;
    //blizzard stuff
    private bool blizzard = false;
    private bool blizzardWarning = false;
    [SerializeField] GameObject snow_1;
    [SerializeField] GameObject snow_2;
    [SerializeField] GameObject snow_3;
    private int snowLvl = 0;

    //shovel
    private bool shovelGrabbed = false;
    public GameObject shovel;
    private bool shovelBuffer = false;

    //snowman
    private float snmCurrentHealth = 100;
    private float snmMaxHealth = 100;
    private bool singing = false;
    public Image healthbar;

    //UI variables
    private int dayCounter = 1;
    [SerializeField] TextMeshProUGUI calendarTxt;
    [SerializeField] GameObject startingTxt;
    [SerializeField] GameObject blizzardTxt;
    [SerializeField] GameObject shovelTxt;

    void Awake()
    {
        // Reference
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        // Characte Animators
        //animator = gameObject.GetComponent<Animator>();

        //isWalkingHash = Animator.StringToHash("isWalking");
        //isRunningHash = Animator.StringToHash("isRunning");

        // Register Input & Cancel Movement
        // Walk
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        // // Run
        playerInput.CharacterControls.Run.performed += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
        // Interact
        playerInput.CharacterControls.Interact.started += onInteract;
        playerInput.CharacterControls.Interact.canceled += onInteract;
    

        StartCoroutine(TriggerBlizzard(12.0f));
        StartCoroutine(ChangeDay(16.0f));

        healthbar.fillAmount = snmMaxHealth;

        //------ SEARCH: when the game starts
    }

    void handleRotation()
    {
        // Update Character Orientation
        Vector3 positionToLookAt;
        // Position Character should point to
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        // Current Character Rotation
        Quaternion currentRotation = transform.rotation;

        // Check if Character is moving
        if (isMovementPressed)
        {
            // New Rotation based on updated position
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        // Manage Call back functions
        // Walk
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * walkSpeed;
        currentMovement.z = currentMovementInput.y * walkSpeed;
        // Run
        currentRunMovement.x = -currentMovementInput.x * runMultiplier;
        currentRunMovement.z = -currentMovementInput.y * runMultiplier;
        // Check if moving
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        //------ SEARCH: Walking area, footstep sounds?
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void onInteract(InputAction.CallbackContext context)
    {
        isInteractPressed = context.ReadValueAsButton();
        //------ SEARCH: interact area, sound cue for interaction
    }

    void handleAnimation() {

        // WALK ANIMATION
        // Manage Animation Switches
        //bool isWalking = animator.GetBool(isWalkingHash);
        //bool isRunning = animator.GetBool(isRunningHash);

        // Animate
        // if (isMovementPressed && !isWalking)
        // {
        //     // Walk
        //     animator.SetBool("isWalking", true);
        // }
        // else if (!isMovementPressed && isWalking)
        // {
        //     // Stop Walking
        //     animator.SetBool("isWalking", false);
        // }

        // if ((isMovementPressed && isRunPressed) && !isRunning)
        // {
        //     // Run
        //     animator.SetBool("isRunning", true);
        // }
        // else if ((!isMovementPressed || isRunPressed) && isRunning)
        // {
        //     // Stop Running
        //     animator.SetBool("isRunning", false);
        // }
    }

    void FixedUpdate()
    {
        // Move
        // Check if running
        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime * 1.6f);
        }

        // Animate
        //handleAnimation();
        handleRotation();

        //slow down the movement speed
        switch(snowLvl){
            case 0: snow_1.SetActive(false);
            snow_2.SetActive(false);
            snow_3.SetActive(false);
            break;
            case 1: snow_1.SetActive(true);
            snow_2.SetActive(false);
            snow_3.SetActive(false);
            snmCurrentHealth -= 0.01f;
            break;
            case 2: snow_1.SetActive(false);
            snow_2.SetActive(true);
            snow_3.SetActive(false);
            snmCurrentHealth -= 0.03f;
            break;
            case 3: snow_1.SetActive(false);
            snow_2.SetActive(false);
            snow_3.SetActive(true);
            snmCurrentHealth -= 0.05f;
            break;
            case 4: StopCoroutine(AddSnow(0f));
            snmCurrentHealth -= 0.05f;
            break;
        }

        //stop moving when singing
        if(singing){
            walkSpeed = 0.0f;
        } else if(blizzard) {
            walkSpeed = 1.5f;
        } else {
            walkSpeed = 5.0f;
        }

        healthbar.fillAmount = snmCurrentHealth / 100;

        if(isInteractPressed && interacting != "none"){
            StartCoroutine(Interaction(10.0f));
        }
    }

    void OnEnable()
    {
        // Enable Input System (Action Map)
        playerInput.CharacterControls.Enable();
    }
    void OnDisable()
    {
        // Disable Input System (Action Map)
        playerInput.CharacterControls.Disable();
    }

    //check for any collisions
    void OnTriggerEnter(Collider target){
        Debug.Log("in");
        switch(target.gameObject.tag){
            case "snowman": interacting = "snowman";

            break;
            case "shovel": interacting = "shovel";
            break;
        }
    }
    //check when collisions stop (for interactions)
    void OnTriggerExit(Collider target){
        Debug.Log("out");
      switch(target.gameObject.tag){
            case "snowman": interacting = "none";
            break;
            case "shovel": interacting = "none";
                shovelBuffer = false;
            break;
        }
    }

    void collisionInteraction(){
        switch(interacting){
            case "snowman": 
                Debug.Log(":D");
                if(shovelGrabbed == true && snowLvl >= 1 && isInteractPressed){
                    //------ SEARCH: shoveling
                    snowLvl -= 1;
                    //text stuff
                    isInteractPressed = false;
                }
                if(shovelGrabbed == false && singing == false && isInteractPressed){
                    //------ SEARCH: singing
                    snmCurrentHealth += 10;
                    StartCoroutine(Singing(3.0f));
                    //text stuff
                    isInteractPressed = false;
                }
            break;
            case "shovel": 
                if(!shovelBuffer){
                    shovelGrabbed = !shovelGrabbed;
                    if(shovelGrabbed){
                        shovel.SetActive(false);
                    } else if(!shovelGrabbed){
                        shovel.SetActive(true);
                        gameObject.transform.position = new Vector3(-17f, 1.5f, 15);
                    }
                    shovelBuffer = true;
                }
            break;
        }
    }

    // change the day for 
    IEnumerator ChangeDay(float time)
    {
        yield return new WaitForSeconds(time);
        dayCounter += 1;
        calendarTxt.text = dayCounter.ToString();
        StartCoroutine(ChangeDay(16.0f));
    }

    // Timer to add snow
    IEnumerator AddSnow(float time)
    {
        yield return new WaitForSeconds(time);
        if(blizzard){
            snowLvl += 1;
            StartCoroutine(AddSnow(3.0f));
        } else {
            StopCoroutine(AddSnow(0f));
        } //!blizzard
    }

    // trigger the blizzard, base is 12f
    IEnumerator TriggerBlizzard(float time)
    {
        yield return new WaitForSeconds(time);
        //warning text
        blizzardTxt.SetActive(true);

        yield return new WaitForSeconds(6.0f);
        //remove warning text
        blizzardTxt.SetActive(false);
        blizzard = true;
        //start snow
        if(blizzard){
            StartCoroutine(AddSnow(3.0f));
        }
        yield return new WaitForSeconds(20.0f);
        blizzard = false;
        //reset blizzard after 12 sec
        StartCoroutine(TriggerBlizzard(12.0f));
    }

    // Add snow timer
    IEnumerator Interaction(float time)
    {
        collisionInteraction();
        yield return new WaitForSeconds(time);
    }

    // Singing
    IEnumerator Singing(float time)
    {
        singing = true;
        yield return new WaitForSeconds(time);
        singing = false;
    }
}