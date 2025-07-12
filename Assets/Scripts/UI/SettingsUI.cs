using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("类别按钮")]
    public Button buttonSound;
    public Button buttonKeybinds;
    public Button buttonHints;

    [Header("面板")]
    public GameObject soundPanel;
    public GameObject keybindsPanel;
    public GameObject hintsPanel;

    [Header("音量设置")]
    public Slider gameVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("按键与提示")]
    public Transform keybindsContent;
    public GameObject keybindEntryPrefab; // 每个按键项的 prefab（显示按键与描述）

    public TextMeshProUGUI hintsText;

    void Start()
    {
        // 初始化按钮事件
        buttonSound.onClick.AddListener(() => ShowPanel("sound"));
        buttonKeybinds.onClick.AddListener(() => ShowPanel("keybinds"));
        buttonHints.onClick.AddListener(() => ShowPanel("hints"));

        // 初始化滑动条
        gameVolumeSlider.onValueChanged.AddListener((value) => {
            Debug.Log("游戏音量: " + value);
            // 可设置 AudioListener.volume = value;
        });

        sfxVolumeSlider.onValueChanged.AddListener((value) => {
            Debug.Log("音效音量: " + value);
        });

        // 填充按键说明
        PopulateKeybinds();

        // 填充玩法提示
        PopulateHints();

        // 默认显示
        ShowPanel("sound");
    }

    void ShowPanel(string type)
    {
        soundPanel.SetActive(type == "sound");
        keybindsPanel.SetActive(type == "keybinds");
        hintsPanel.SetActive(type == "hints");
    }

    void PopulateKeybinds()
    {
        string[,] keys = {
            {"A", "Move left"},
            {"D", "Move right"},
            {"W", "Stairs up"},
            {"S", "Stairs down"},
            {"E", "Interact (destroy turret / pick up item)"},
            {"J", "Shoot"},
            {"U", "Record a timestamp"},
            {"I", "Travel back to the timestamp"},
            {"O", "Travel back to the timestamp with movement"},
            {"Esc", "Pause"},
            {"R", "Restart"},
        };

        foreach (Transform child in keybindsContent)
            Destroy(child.gameObject);

        for (int i = 0; i < keys.GetLength(0); i++)
        {
            GameObject entry = Instantiate(keybindEntryPrefab, keybindsContent);
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = keys[i, 0]; // 键
            texts[1].text = keys[i, 1]; // 描述
        }
    }

    void PopulateHints()
    {
        hintsText.text = "Some Instructions";
    }
}
