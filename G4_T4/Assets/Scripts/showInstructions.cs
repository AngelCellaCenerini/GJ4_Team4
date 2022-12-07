using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showInstructions : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject instructionsMenu;

    public void toggleMenuInstructions()
    {
        if (mainMenu.activeSelf)
        {
            // Open Instruction Menu
            mainMenu.SetActive(false);
            instructionsMenu.SetActive(true);
        }
        else if (instructionsMenu.activeSelf)
        {
            // Close Instruction Menu
            mainMenu.SetActive(true);
            instructionsMenu.SetActive(false);
        }
    }
}
