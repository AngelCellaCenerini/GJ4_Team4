using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    public GameObject lanternLight;
    public GameObject windowLight;
    bool isFlickering = true;

    // Start is called before the first frame update
    void Start()
    {
        lanternLight.SetActive(true);
        windowLight.SetActive(true);
        isFlickering = false;
    }

    void FixedUpdate (){
        if (!isFlickering && !lanternLight.activeSelf)
        {
            lanternLight.SetActive(true);
        }

        if (!isFlickering && !windowLight.activeSelf)
        {
            windowLight.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "houseWalls")
        {
            isFlickering = true;
            StartCoroutine(FlickerOn(Random.Range(0.01f, 0.05f)));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "houseWalls")
        {
            isFlickering = false;
            if (!lanternLight.activeSelf)
            {
                lanternLight.SetActive(true);
            }
            if (!windowLight.activeSelf)
            {
                windowLight.SetActive(true);
            }
        }
    }


    public IEnumerator FlickerOn(float t)
    {
        if (isFlickering)
        {
            yield return new WaitForSeconds(t);
            if (lanternLight.activeSelf)
            {
                lanternLight.SetActive(false);
                StartCoroutine(FlickerOff(Random.Range(0.05f, 0.1f)));
            }
            if (windowLight.activeSelf)
            {
                windowLight.SetActive(false);
                StartCoroutine(FlickerOff(Random.Range(0.05f, 0.1f)));
            }
        }
    }

    public IEnumerator FlickerOff(float t)
    {
        if (isFlickering)
        {
            yield return new WaitForSeconds(t);
            if (!windowLight.activeSelf)
            {
                windowLight.SetActive(true);
                StartCoroutine(FlickerOn(1.0f));
            }
            if (!lanternLight.activeSelf)
            {
                lanternLight.SetActive(true);
                StartCoroutine(FlickerOn(1.0f));
            }
        }
    }
}
