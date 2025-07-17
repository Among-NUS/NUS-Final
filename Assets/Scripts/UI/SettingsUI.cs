using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("类别按钮")]
    public Button buttonKeybinds;
    public Button buttonHints;

    [Header("面板")]
    public GameObject keybindsPanel;
    public GameObject hintsPanel;

    [Header("按键与提示")]
    public Transform keybindsContent;
    public GameObject keybindEntryPrefab;
    public ScrollRect keybindsScrollRect; // ✅ 拖入 ScrollView 的 ScrollRect 组件

    public TextMeshProUGUI hintsText;

    void Start()
    {
        buttonKeybinds.onClick.AddListener(() => ShowPanel("keybinds"));
        buttonHints.onClick.AddListener(() => ShowPanel("hints"));

        PopulateKeybinds();
        PopulateHints();

        // 默认显示声音面板
        ShowPanel("keybinds");
    }

    void ShowPanel(string type)
    {
        keybindsPanel.SetActive(type == "keybinds");
        hintsPanel.SetActive(type == "hints");

        if (type == "keybinds")
        {
            ScrollToTop();
        }
    }

    void ScrollToTop()
    {
        // 强制刷新布局再滚动
        Canvas.ForceUpdateCanvases();
        if (keybindsScrollRect != null)
        {
            keybindsScrollRect.verticalNormalizedPosition = 1f;
        }
    }

    void PopulateKeybinds()
    {
        string[,] keys = {
        {"A", "Move left"},
        {"D", "Move right"},
        {"W", "Go upstairs"},
        {"S", "Go downstairs"},
        {"E", "Interact"},
        {"J", "Shoot"},
        {"U", "Save timestamp"},
        {"I", "Rewind to timestamp"},
        {"O", "Rewind with movement"},
        {"Esc", "Pause"},
        {"Backspace", "Restart"}
        };

        foreach (Transform child in keybindsContent)
            Destroy(child.gameObject);

        for (int i = 0; i < keys.GetLength(0); i++)
        {
            GameObject entry = Instantiate(keybindEntryPrefab, keybindsContent);
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = keys[i, 0];
            texts[1].text = keys[i, 1];
        }
    }

    void PopulateHints()
    {
        //hintsText.text = "Some Instructions";
    }
}
