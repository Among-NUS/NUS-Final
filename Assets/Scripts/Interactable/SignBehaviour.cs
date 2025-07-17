using UnityEngine;
using UnityEngine.UI;

public class SignBehaviour : MonoBehaviour
{
    [Header("���Ǳ�ǩ")]
    public string playerTag = "Player";

    [Header("Ҫ��ʾ��Panel")]
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
        // ֻ���ڷ�Χ�ڲż�ⰴ��
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

