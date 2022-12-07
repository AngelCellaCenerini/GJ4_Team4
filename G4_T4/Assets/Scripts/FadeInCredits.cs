using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeInCredits : MonoBehaviour
{
    TextMeshProUGUI textContent;
    // Start is called before the first frame update
    void Start()
    {
        // Set Transparent
        textContent = this.GetComponent<TMPro.TextMeshProUGUI>();
        textContent.color = new Color(textContent.color.r, textContent.color.g, textContent.color.b, 0);
        // Delay Fading
        StartCoroutine(DelayFading(5.0f));
    }
    public IEnumerator DelayFading(float t)
    {
        yield return new WaitForSeconds(t);
        // Fade
        StartCoroutine(FadeTextToFullAlpha(3.0f));

    }
        public IEnumerator FadeTextToFullAlpha(float t)
    {
        textContent.color = new Color(textContent.color.r, textContent.color.g, textContent.color.b, 0);
        while (textContent.color.a < 1.0f)
        {
            textContent.color = new Color(textContent.color.r, textContent.color.g, textContent.color.b, textContent.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

}
