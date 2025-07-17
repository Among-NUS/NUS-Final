using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;

    void Start()
    {
        gameOverPanel.SetActive(false); // 开始时隐藏
    }

    // 触发 Game Over 时调用这个
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        // ✅ 找到名为 gameOverText 的子物体并修改 TMP_Text
        var textTransform = gameOverPanel.transform.Find("DeadWayText");
        if (textTransform != null)
        {
            TMP_Text tmpText = textTransform.GetComponent<TMP_Text>();
            if (tmpText)
                tmpText.text = "Hit by Bullet";
        }
        else
        {
            Debug.LogWarning("⚠ gameOverText 子物体没找到");
        }

        Time.timeScale = 0f;  // 暂停游戏
    }

    public void ShowGhostGameOver()
    {
        gameOverPanel.SetActive(true);

        // ✅ 找到名为 gameOverText 的子物体并修改 TMP_Text
        var textTransform = gameOverPanel.transform.Find("DeadWayText");
        if (textTransform != null)
        {
            TMP_Text tmpText = textTransform.GetComponent<TMP_Text>();
            if (tmpText)
                tmpText.text = "Causality Broken";
        }
        else
        {
            Debug.LogWarning("⚠ gameOverText 子物体没找到");
        }

        Time.timeScale = 0f;  // 暂停游戏
    }


    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExitButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
