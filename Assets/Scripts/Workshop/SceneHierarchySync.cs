using UnityEngine;
using UnityEngine.EventSystems;
using RuntimeInspectorNamespace;

/// <summary>
/// �� Scene/Game ��ͼ�е�������� RuntimeHierarchy ѡ��״̬����ͬ��
/// </summary>
public class SceneHierarchySync : MonoBehaviour
{
    [Tooltip("����ʱ�㼶������")]
    public RuntimeHierarchy hierarchy;

    [Tooltip("�������߼������ (һ���� Gizmo ���)")]
    public Camera gizmoCamera;

    [Tooltip("��Щ Layer ���������")]
    public LayerMask pickableLayers = ~0;   // Ĭ��ȫ��

    void Update()
    {
        // ����������겻�� UI ��
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = gizmoCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, pickableLayers))
            {
                // �ҵ����� "_Wrapper" ����
                Transform t = hit.collider.transform;
                while (t && !t.name.EndsWith("_Wrapper"))
                    t = t.parent;

                if (t != null && hierarchy != null)
                {
                    hierarchy.Select(t, RuntimeHierarchy.SelectOptions.None);
                }
            }
        }
    }
}
