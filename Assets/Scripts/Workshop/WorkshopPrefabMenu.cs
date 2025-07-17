using UnityEngine;
using UnityEngine.UI;
using TMPro; // ���� TMP_Text

public class WorkshopPrefabMenu : MonoBehaviour
{
    [Header("UI References")]
    public Transform buttonContainer;      // ScrollView/Viewport/Content
    public Button buttonTemplate;          // ��ťģ�壨���أ�
    public string prefabFolder = "Prefabs/Workshop";
    public WorkshopManager workshopManager;

    void Start()
    {
        // ��� Content �Ƿ��в������
        EnsureLayout(buttonContainer.gameObject);

        // ɨ�� Prefab
        var prefabs = Resources.LoadAll<GameObject>(prefabFolder);
        Debug.Log($"[PrefabMenu] �ҵ� {prefabs.Length} �� Prefab");

        if (prefabs.Length == 0) return;

        foreach (var prefab in prefabs)
            CreateButtonForPrefab(prefab.name);

        // �������ģ��
        buttonTemplate.gameObject.SetActive(false);
    }

    void CreateButtonForPrefab(string prefabName)
    {
        var btnObj = Instantiate(buttonTemplate, buttonContainer);
        btnObj.gameObject.SetActive(true);

        // ֧�� TextMeshPro
        var textComp = btnObj.GetComponentInChildren<Text>();
        if (textComp) textComp.text = prefabName;
        else
        {
            var tmpText = btnObj.GetComponentInChildren<TMP_Text>();
            if (tmpText) tmpText.text = prefabName;
        }

        btnObj.onClick.AddListener(() =>
        {
            Debug.Log($"[PrefabMenu] ������� {prefabName}");
            workshopManager.SpawnPrefab(prefabName);
        });
    }

    void EnsureLayout(GameObject content)
    {
        // ���û�� VerticalLayoutGroup���Զ����
        var vlg = content.GetComponent<VerticalLayoutGroup>();
        if (!vlg)
        {
            vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 10;
        }

        // ���û�� ContentSizeFitter���Զ����
        var csf = content.GetComponent<ContentSizeFitter>();
        if (!csf)
        {
            csf = content.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }
}
