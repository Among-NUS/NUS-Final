using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroUI : MonoBehaviour
{
    public GameObject IntroduceCanvas;
    public GameObject PauseCanvas;
    public GameObject DialogCanvas;
    public GameObject InGameUICanvas;
    private static bool hasIntroBeenShown = false;

    // Start is called before the first frame update
    void Start()
    { 
        if (hasIntroBeenShown)
        {
            OnSkipButtonClicked();
        }

        Time.timeScale = 0f;
        PauseCanvas.SetActive(false);
        InGameUICanvas.SetActive(false);
        if (DialogCanvas != null) DialogCanvas.SetActive(false);
        IntroduceCanvas.SetActive(true);
    }
    public void OnSkipButtonClicked()
    {
        Time.timeScale = 1f;
        hasIntroBeenShown = true;
        PauseCanvas.SetActive(true);
        InGameUICanvas.SetActive(true);
        if (DialogCanvas != null) DialogCanvas.SetActive(true);
        IntroduceCanvas.SetActive(false);
    }
}
