using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroUI : MonoBehaviour
{
    public GameObject IntroduceCanvas;
    private static bool hasIntroBeenShown = false;

    // Start is called before the first frame update
    void Start()
    { 
        if (hasIntroBeenShown)
        {
            IntroduceCanvas.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

        Time.timeScale = 0f;
        IntroduceCanvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Time.timeScale = 1f;
            hasIntroBeenShown = true;
            IntroduceCanvas.SetActive(false);
        }
    }
}
