using UnityEngine;
using UnityEngine.UI;

public class SignBehaviour : MonoBehaviour
{
    [Header("主角标签")]
    public string playerTag = "Player";

    [Header("要显示的Panel")]
    public GameObject panelToShow;

    private bool isPlayerInRange = false;

    public GameObject PauseCanvas;
    public GameObject DialogCanvas;
    public GameObject InGameUICanvas;

    // Start is called before the first frame update
    void Start()
    {
        if (panelToShow) panelToShow.SetActive(false);
    }

    void Update()
    {
        // 只有在范围内才检测按键
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale = 0f;
            PauseCanvas.SetActive(false);
            InGameUICanvas.SetActive(false);
            if (DialogCanvas != null) DialogCanvas.SetActive(false);

            panelToShow.SetActive(true);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            isPlayerInRange = false;
        }
    }
    public void OnSkipButtonClicked()
    {
        Time.timeScale = 1f;
        PauseCanvas.SetActive(true);
        InGameUICanvas.SetActive(true);
        if (DialogCanvas != null) DialogCanvas.SetActive(true);

        panelToShow.SetActive(false);
    }

}

