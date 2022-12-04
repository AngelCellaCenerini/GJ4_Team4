using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowPilingBlizard : MonoBehaviour
{
    // Reference Snow objet and its animator
    public GameObject groundSnow;
    Animator snowAnimator;
    // Start is called before the first frame update
    void Start()
    {
        // Declare Animator in Awake() or Start()
        snowAnimator = groundSnow.GetComponent<Animator>();
    }

    // Update is called once per frame
    void playAnimation()
    {
        // Place this code when the blizard starts (inside an if() or whatever you have)
        // Start Piling 
        snowAnimator.SetBool("isSnowing", true);

        // Place this code when the blizard stops (inside an if() or whatever you have)
        // Stop Piling 
        snowAnimator.SetBool("isSnowing", false);

    }
}
