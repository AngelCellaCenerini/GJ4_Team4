using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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

    // Movement
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    // States
    bool isMovementPressed;
    bool isInteractPressed;
    string interacting = "none";
    // Multipliers
    float walkSpeed = 5.0f;
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

    //music box
    private bool musicGrabbed = false;
    public GameObject musicBox;
    private bool musicBuffer = false;
    private bool musicPlaying = false;

    //snowman
    private float snmCurrentHealth = 100;
    private float snmMaxHealth = 100;
    public Image healthbar;

    //UI variables
    private int dayCounter = 1;
    [SerializeField] TextMeshProUGUI calendarTxt;
    [SerializeField] GameObject startingTxt;
    [SerializeField] GameObject blizzardTxt;
    [SerializeField] GameObject shovelTxt;

    //sounds
    private bool isPlaying = false;
    private bool songSwitch = false;
    public AudioSource audio;
    public AudioSource audio2;
    public GameObject audioSource2;
    public AudioClip shovelingSFX;
    public AudioClip tune1SFX;
    public AudioClip tune2SFX;
    public AudioClip footstepSFX;
    public AudioClip footstepSlowSFX;

    public AudioClip grabShovelSFX;
    public AudioClip grabMusicBoxSFX;
    public AudioClip dropItemSFX;

    public AudioClip bellRingingSFX;
    public AudioClip blizzardSFX;

    // Angel
    // Snow Particle System
    //public GameObject blizzardSnow;
    public ParticleSystem snowParticles;
    //

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
        // Interact
        playerInput.CharacterControls.Interact.started += onInteract;
        playerInput.CharacterControls.Interact.canceled += onInteract;
    

        StartCoroutine(TriggerBlizzard(12.0f));
        StartCoroutine(ChangeDay(16.0f));
        StartCoroutine(InitialTxt());

        healthbar.fillAmount = snmMaxHealth;

        //Audio Resource
        audio = GetComponent<AudioSource>();
<<<<<<< Updated upstream
        audio2 = audioSource2.GetComponent<AudioSource>();
=======

        // Angel
        // Snow Particles
        snowParticles = snowParticles.GetComponent<ParticleSystem>();
        //
>>>>>>> Stashed changes
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
        // currentRunMovement.x = -currentMovementInput.x * runMultiplier;
        // currentRunMovement.z = -currentMovementInput.y * runMultiplier;
        // Check if moving
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        if (isMovementPressed && !musicPlaying) { 
            audio.clip = footstepSFX;
            audio.volume = 0.1f;
            audio.Play();
        }
        else
        {
            audio.Stop();
        }
    }

    void onInteract(InputAction.CallbackContext context)
    {
        isInteractPressed = context.ReadValueAsButton();
  
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
        characterController.Move(currentMovement * Time.deltaTime * 1.6f);

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
        if(musicPlaying){
            walkSpeed = 0.0f;
        } else if(blizzard) {
            walkSpeed = 4f;
        } else {
            walkSpeed = 10.0f;
            //+5
        }

        healthbar.fillAmount = snmCurrentHealth / 100;

        // if(isInteractPressed && interacting == "snowman"){
        //     float timer = 0;
        //     timer += Time.deltaTime;
        //     if(timer >= 100){
        //         StartCoroutine(Interaction(10.0f));
        //     }
        // } else if(isInteractPressed && interacting != "none"){
        //     StartCoroutine(Interaction(10.0f));
        // }

        if(isInteractPressed && interacting != "none"){
            StartCoroutine(Interaction(10.0f));
        }

        //end condition
        if(snmCurrentHealth <= 0){
            SceneManager.LoadScene("EndScene");
        } else if (dayCounter == 26){
            SceneManager.LoadScene("EndScene");
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
        switch(target.gameObject.tag){
            case "snowman": interacting = "snowman";
            break;
            case "shovel": interacting = "shovel";
            break;
            case "musicbox": interacting = "musicbox";
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
            case "musicbox": interacting = "none";
                musicBuffer = false;
            break;
        }
    }

    void collisionInteraction(){
        switch(interacting){
            case "snowman": 
                if(shovelGrabbed && !musicGrabbed && snowLvl >= 1 && isInteractPressed){
                    if(snowLvl >= 1){
                        snowLvl -= 1;
                    }
                    //text stuff
                    isInteractPressed = false;
                    audio.clip = shovelingSFX;
                    audio.Play();
                    //AudioSource.PlayOneShot(shoveling, 1f);
                }
                if(!shovelGrabbed && musicGrabbed && !musicPlaying && isInteractPressed){
                    if(snmCurrentHealth <= 90 && snowLvl == 0){
                        snmCurrentHealth += 10;
                    } else if(snmCurrentHealth <= 90 && snowLvl == 1){
                        snmCurrentHealth += 8;
                    } else if(snmCurrentHealth <= 90 && snowLvl == 2){
                        snmCurrentHealth += 5;
                    } else if(snmCurrentHealth <= 90 && snowLvl == 3){
                        snmCurrentHealth += 3;
                    }
                    StartCoroutine(Singing(3.0f));
                    //text stuff
                    isInteractPressed = false;
                }
            break;
            case "shovel": 
                if(!shovelBuffer && !musicGrabbed){
                    shovelGrabbed = !shovelGrabbed;
                    if(shovelGrabbed){
                        // SEARCH: SHOVEL DROP 
                        audio.clip = grabShovelSFX;
                        audio.volume = 2.5f;
                        audio.Play();
                        shovel.SetActive(false);

                    } else if(!shovelGrabbed){
                        // SEARCH: SHOVEL PICK UP 
                        shovel.SetActive(true);
                        gameObject.transform.position = new Vector3(-11f, 3f, -32);
                        audio.clip = dropItemSFX;
                        audio.volume = 2.5f;
                        audio.Play();
                    }
                    shovelBuffer = true;
                } else if(musicGrabbed){
                    Debug.Log("Can't hold more then 1 thing");
                }
            break;
            case "musicbox": 
                if(!musicBuffer && !shovelGrabbed){
                    musicGrabbed = !musicGrabbed;
                    if(musicGrabbed){
                        // SEARCH: MUSIC DROP 
                        audio.clip = grabMusicBoxSFX;
                     
                        audio.volume = 2.5f;
                        audio.Play();
                        musicBox.SetActive(false);
                    } else if(!musicGrabbed){
                        // SEARCH: MUSIC PICK UP 
                        audio.clip = dropItemSFX;
                        audio.volume = 2.5f;
                        audio.Play();
                        musicBox.SetActive(true);
                        gameObject.transform.position = new Vector3(3f, 3f, -10f);
                    }
                    musicBuffer = true;
                } else if(shovelGrabbed){
                    Debug.Log("Can't hold more then 1 thing");
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

    IEnumerator InitialTxt(){
        //warning text
        startingTxt.SetActive(true);
        yield return new WaitForSeconds(10.0f);
        startingTxt.SetActive(false);
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

        // Angel
        // Start Snow Particles
        snowParticles.Play();
        //

        yield return new WaitForSeconds(6.0f);
        //blizzard start
        blizzardTxt.SetActive(false);
        blizzard = true;
        //start snow
        if(blizzard){
            StartCoroutine(AddSnow(3.0f));
        }
        yield return new WaitForSeconds(20.0f);
        //stop blizzard
        blizzard = false;
<<<<<<< Updated upstream
=======
        // Angel
        // Start Snow Particles
        snowParticles.Stop();
        //
        //reset blizzard after 12 sec
>>>>>>> Stashed changes
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
        musicPlaying = true;
        if(songSwitch){
            //play tune 1
            audio2.clip = tune1SFX;
            audio2.Play();
            songSwitch = !songSwitch;

        } else if(!songSwitch){
            //play tune 2
            audio2.clip = tune2SFX;
            audio2.Play();
            songSwitch = !songSwitch;
        }
        yield return new WaitForSeconds(time);
        musicPlaying = false;
    }
}