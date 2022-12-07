using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

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
    public bool blizzard = false;
    private bool blizzardWarning = false;
    [SerializeField] GameObject snow_1;
    [SerializeField] GameObject snow_2;
    [SerializeField] GameObject snow_3;
    private int snowLvl = 0;

    //shove;l
    public GameObject shovel;
    public GameObject shovelOutline;
    public GameObject shovelIcon;
    private bool shovelGrabbed = false;
    private bool shovelBuffer = false;
    private int shovelNum = 0;

    //music box
    private bool musicGrabbed = false;
    public GameObject musicBox;
    public GameObject musicBoxOutline;
    public GameObject musicBoxIcon;
    private bool musicBuffer = false;
    private bool musicPlaying = false;

    //snowman
    private float snmCurrentHealth = 100;
    private float snmMaxHealth = 100;
    public Image healthbar;
    // Animation
    public GameObject snowman;
    Animator snowmanAnimator;

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
    public ParticleSystem snowBlizard;
    // Snow Piling Up on Ground
    public GameObject groundSnow;
    Animator snowAnimator;

    // UI Inventory
    public RawImage inventoryHolder;
    public GameObject inventory;
    //

    void Awake()
    {
        // Reference
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        // Characte Animators
        animator = gameObject.GetComponent<Animator>();
        snowmanAnimator = snowman.GetComponent<Animator>();

        //isWalkingHash = Animator.StringToHash("isWalking");
        //isRunningHash = Animator.StringToHash("isRunning");

        // Register Input & Cancel Movement
        // Walk
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        // Interact
        playerInput.CharacterControls.Interact.started += onInteract;
        playerInput.CharacterControls.Interact.canceled += onInteract;
    

        StartCoroutine(TriggerBlizzard(10.0f));
        StartCoroutine(ChangeDay(8.0f));
        StartCoroutine(InitialTxt());

        healthbar.fillAmount = snmMaxHealth;

        //Audio Resource
        audio = GetComponent<AudioSource>();
        audio2 = audioSource2.GetComponent<AudioSource>();

        // Angel
        // Snow Particle System
        snowBlizard = snowBlizard.GetComponent<ParticleSystem>();
        // Snow Ground Animator
        snowAnimator = groundSnow.GetComponent<Animator>();
        // Set Collectibles Outlines & Icons False
        shovelOutline.SetActive(false);
        shovelIcon.SetActive(false);
        musicBoxOutline.SetActive(false);
        musicBoxIcon.SetActive(false);

        // Inventory UI
        inventoryHolder = inventory.GetComponent<RawImage>();
        //
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
        bool isWalking = animator.GetBool(isWalkingHash);

        // Animate
        // Walk
        /*if (isMovementPressed && !blizzard)
        {
           // Walk
           animator.SetBool("isWalking", true);
        }
        else if (!isMovementPressed)
        {
          // Stop Walking
          animator.SetBool("isWalking", false);
        }*/

        if (isMovementPressed && !musicPlaying)
        {
            if (blizzard)
            {
                /*if (!animator.GetBool("isBlizzarding"))
                {
                    // Walking
                    animator.SetBool("isBlizzarding", true);
                    Debug.Log("walk1");
                }*/
                if (!animator.GetBool("isBlizzarding") || animator.GetBool("isWalking"))
                {
                    // Walking
                    animator.SetBool("isBlizzarding", true);
                    animator.SetBool("isWalking", false);
                }
            }
            else
            {
                if (!animator.GetBool("isWalking") || animator.GetBool("isBlizzarding"))
                {
                    // Walking
                    animator.SetBool("isBlizzarding", false);
                    animator.SetBool("isWalking", true);
                }
            }
        }
        else if(!isMovementPressed || musicPlaying)
        {
            if (animator.GetBool("isWalking"))
            {
                // Stop Walking
                animator.SetBool("isWalking", false);
            }
            else if (animator.GetBool("isBlizzarding"))
            {
                // Stop Walking
                animator.SetBool("isBlizzarding", false);
            }
        }
    }

    void FixedUpdate()
    {
        // Move
        characterController.Move(currentMovement * Time.deltaTime * 1.6f);

        // Animate
        handleAnimation();
        // Snowman Animation
        handleSnowmanAnimation();
        handleRotation();
        // Inventory
        handleInventory();

        Debug.Log(snowLvl);

        //change snow level
        switch(snowLvl){
            case 0: snow_1.SetActive(false);
            snow_2.SetActive(false);
            snow_3.SetActive(false);
            break;
            case 1: snow_1.SetActive(true);
            snow_2.SetActive(false);
            snow_3.SetActive(false);
            //change health loss throughout the game
            if(dayCounter <= 8){
                snmCurrentHealth -= 0.01f;
            } else if(dayCounter >= 9 && dayCounter <= 17){
                snmCurrentHealth -= 0.03f;
            } else if(dayCounter >= 18){
                snmCurrentHealth -= 0.05f;
            }
            break;
            case 2: snow_1.SetActive(false);
            snow_2.SetActive(true);
            snow_3.SetActive(false);
            //change health loss throughout the game
            if(dayCounter <= 8){
                snmCurrentHealth -= 0.03f;
            } else if(dayCounter >= 9 && dayCounter <= 17){
                snmCurrentHealth -= 0.04f;
            } else if(dayCounter >= 18){
                snmCurrentHealth -= 0.06f;
            }
            break;
            case 3: snow_1.SetActive(false);
            snow_2.SetActive(false);
            snow_3.SetActive(true);
            //change health loss throughout the game
            if(dayCounter <= 8){
                snmCurrentHealth -= 0.05f;
            } else if(dayCounter >= 9 && dayCounter <= 17){
                snmCurrentHealth -= 0.06f;
            } else if(dayCounter >= 18){
                snmCurrentHealth -= 0.08f;
            }
            break;
            case 4: StopCoroutine(AddSnow(0f));
            if(dayCounter <= 8){
                snmCurrentHealth -= 0.05f;
            } else if(dayCounter >= 9 && dayCounter <= 17){
                snmCurrentHealth -= 0.06f;
            } else if(dayCounter >= 18){
                snmCurrentHealth -= 0.08f;
            }
            break;
        }

        //stop moving when singing
        if(musicPlaying){
            walkSpeed = 0.0f;
        } else if(blizzard) {
            walkSpeed = 4f;
            //StartCoroutine(DelaySnowWalk(2.0f));

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
            SceneManager.LoadScene("EndSceneBad");
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

    void handleInventory()
    {
        if (shovelGrabbed && isInteractPressed)
        {
            //Debug.Log("using shovel");
        }
        else
        {

        }
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
        //Debug.Log("out");
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
                    shovelNum += 1;
                    // Change Inventory UI
                    inventoryHolder.color = new Color32(41, 0, 171, 158);

                    if (dayCounter <= 8 && shovelNum == 1){
                        snowLvl -= 1;
                        shovelNum = 0;
                    } else if(dayCounter >= 9 && dayCounter <= 17 && shovelNum == 3){
                        snowLvl -= 1;
                        shovelNum = 0;
                    } else if(dayCounter >= 18 && shovelNum == 5){
                        snowLvl -= 1;
                        shovelNum = 0;
                    }
                    //text stuff
                    isInteractPressed = false;
                    if (!isInteractPressed)
                    {
                        // Change Inventory UI
                        StartCoroutine(changeUIColor(1.0f));
                    }
                    audio2.clip = shovelingSFX;
                    audio2.Play();
                    //AudioSource.PlayOneShot(shoveling, 1f);
                }
                if(!shovelGrabbed && musicGrabbed && !musicPlaying && isInteractPressed){
                    if(snmCurrentHealth <= 90){
                        switch(snowLvl){
                            case 0: if(dayCounter <= 8){
                                snmCurrentHealth += 10;
                            } else if(dayCounter >= 9 && dayCounter <= 17){
                                snmCurrentHealth += 8;
                            } else if(dayCounter >= 18){
                                snmCurrentHealth += 6;
                            }
                            break;
                            case 1: if(dayCounter <= 8){
                                snmCurrentHealth += 8;
                            } else if(dayCounter >= 9 && dayCounter <= 17){
                                snmCurrentHealth += 6;
                            } else if(dayCounter >= 18){
                                snmCurrentHealth += 4;
                            }
                            break;
                            case 2: if(dayCounter <= 8){
                                snmCurrentHealth += 6;
                            } else if(dayCounter >= 9 && dayCounter <= 17){
                                snmCurrentHealth += 4;
                            } else if(dayCounter >= 18){
                                snmCurrentHealth += 2;
                            }
                            break;
                            case 3: if(dayCounter <= 8){
                                snmCurrentHealth += 4;
                            } else if(dayCounter >= 9 && dayCounter <= 17){
                                snmCurrentHealth += 2;
                            } else if(dayCounter >= 18){
                                snmCurrentHealth += 1;
                            }
                            break;
                        }
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
                        audio2.clip = grabShovelSFX;
                        audio2.volume = 2.5f;
                        audio2.Play();
                        shovel.SetActive(false);
                        // Activate Shovel Outline & Icon
                        shovelOutline.SetActive(true);
                        shovelIcon.SetActive(true);

                    } else if(!shovelGrabbed){
                        // SEARCH: SHOVEL PICK UP 
                        shovel.SetActive(true);
                        // De-Activate Shovel & Icon Outline
                        shovelOutline.SetActive(false);
                        shovelIcon.SetActive(false);
                        gameObject.transform.position = new Vector3(-11f, 3f, -32);
                        audio2.clip = dropItemSFX;
                        audio2.volume = 2.5f;
                        audio2.Play();
                    }
                    shovelBuffer = true;
                } else if(musicGrabbed){
                    //Debug.Log("Can't hold more then 1 thing");
                }
            break;
            case "musicbox": 
                if(!musicBuffer && !shovelGrabbed){
                    musicGrabbed = !musicGrabbed;
                    if(musicGrabbed){
                        // SEARCH: MUSIC DROP 
                        audio2.clip = grabMusicBoxSFX;
                     
                        audio2.volume = 2.5f;
                        audio2.Play();
                        musicBox.SetActive(false);
                        // Activate Box Outline & Icon
                        musicBoxOutline.SetActive(true);
                        musicBoxIcon.SetActive(true);
                    } else if(!musicGrabbed){
                        // SEARCH: MUSIC PICK UP 
                        audio2.clip = dropItemSFX;
                        audio2.volume = 2.5f;
                        audio2.Play();
                        musicBox.SetActive(true);
                        // De-Activate Box Outline & Icon
                        musicBoxOutline.SetActive(false);
                        musicBoxIcon.SetActive(false);
                        gameObject.transform.position = new Vector3(3f, 3f, -10f);
                    }
                    musicBuffer = true;
                } else if(shovelGrabbed){
                    //Debug.Log("Can't hold more then 1 thing");
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
        StartCoroutine(ChangeDay(8.0f));
    }

    IEnumerator changeUIColor(float time)
    {
        yield return new WaitForSeconds(time);
        // Change Back Inventory UI
        inventoryHolder.color = new Color32(9, 0, 171, 76);
    }

    IEnumerator InitialTxt(){
        //warning text
        yield return new WaitForSeconds(14.0f);
        blizzardTxt.SetActive(true);
        yield return new WaitForSeconds(10.0f);
        blizzardTxt.SetActive(false);
    }

    // Timer to add snow
    IEnumerator AddSnow(float time)
    {
        yield return new WaitForSeconds(time);
        if(blizzard && snowLvl <= 2){
            snowLvl += 1;
           // afterBlizzard = true;
            StartCoroutine(AddSnow(3.0f));
        } else {
            StopCoroutine(AddSnow(0f));
        } //!blizzard
    }

    // trigger the blizzard, base is 12f
    IEnumerator TriggerBlizzard(float time)
    {
        yield return new WaitForSeconds(time);
        // Angel - Snow Particle System
        // Start Snow
        snowBlizard.Play();
        //

        yield return new WaitForSeconds(4.0f);
        //blizzard start
        blizzard = true;

        // Angel
        // Snow Starts Piling Up
        // Start Piling 
        snowAnimator.SetBool("isPiling", true);
        //

        //start snow
        if (blizzard){
            StartCoroutine(AddSnow(3.0f));
        }
        yield return new WaitForSeconds(7.0f);
        //stop blizzard
        blizzard = false;
        // Angel - Snow Particle System
        // Start Snow
        snowBlizard.Stop();
        // Angel
        // Snow Starts Piling Up
        // Stop Piling 
        snowAnimator.SetBool("isPiling", false);
        //
        //

        StartCoroutine(TriggerBlizzard(9.0f));
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
        // Change Inventory UI
        inventoryHolder.color = new Color32(41, 0, 171, 158);

        if (songSwitch){
            //play tune 1
            audio2.clip = tune1SFX;
            audio2.volume = 0.2f;
            audio2.Play();
            songSwitch = !songSwitch;

        } else if(!songSwitch){
            //play tune 2
            audio2.clip = tune2SFX;
            audio2.volume = 0.2f;
            audio2.Play();
            songSwitch = !songSwitch;
        }
        yield return new WaitForSeconds(time);
        musicPlaying = false;
        if (!musicPlaying)
        {
            StartCoroutine(changeUIColor(1.0f));
        }

    }

    public void handleSnowmanAnimation()
    {
        if (musicPlaying)
        {
            // Start Dancing
            snowmanAnimator.SetBool("isDancing", true);
        }
        else
        {
            // Stop Dancing
            snowmanAnimator.SetBool("isDancing", false);
        }
    }
}