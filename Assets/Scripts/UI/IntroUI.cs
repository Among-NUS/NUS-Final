using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroUI : MonoBehaviour
{
    public GameObject IntroducePanel;
    private static bool hasIntroBeenShown = false;

    // Start is called before the first frame update
    void Start()
    { 
        if (hasIntroBeenShown)
        {
            IntroducePanel.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

        Time.timeScale = 0f;
        IntroducePanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Time.timeScale = 1f;
            hasIntroBeenShown = true;
            IntroducePanel.SetActive(false);
        }
    }
}
