using UnityEngine;
using RuntimeInspectorNamespace;
using System.Linq;

public class WorkshopManager : MonoBehaviour
{
    public string defaultPrefabName = "Room";
    public GameObject prefabToSpawn;
    public Transform spawnParent;
    public RuntimeHierarchy hierarchy;
    public RuntimeInspector inspector;
    public Camera gizmoCamera;

    public float gridSpacing = 1f;
    public float zoomSpeed = 5f, minZoom = 2f, maxZoom = 20f;
    Vector3 lastMouse;
    void Start()
    {
        // ✅ 直接生成 Hero，然后传送
        SpawnPrefab("Hero");
        var heroWrapper = spawnParent.Cast<Transform>().FirstOrDefault(w => w.name.Contains("Hero"));
        if (heroWrapper)
        {
            heroWrapper.position = new Vector3(956.8f, 538.6f, 0f);
            Debug.Log("✅ Hero 已传送到 (956.8, 538.6)");
        }

        // ✅ 直接生成 LevelExit，然后传送
        SpawnPrefab("LevelExit");
        var exitWrapper = spawnParent.Cast<Transform>().FirstOrDefault(w => w.name.Contains("LevelExit"));
        if (exitWrapper)
        {
            exitWrapper.position = new Vector3(963.3f, 539.9f, 0f); // 可自行改
            Debug.Log("✅ LevelExit 已传送到 (936.3, 539.9)");
        }

        SpawnPrefab("Ground");
        var groundWrapper = spawnParent.Cast<Transform>().FirstOrDefault(w => w.name.Contains("Ground"));
        if (groundWrapper)
        {
            groundWrapper.position = new Vector3(956.2f, 537.7f, 0f); // 可自行改
            Debug.Log("✅ Ground 已传送到 (956.2, 537.7)");
        }
    }


    /*──────── 相机缩放 / 平移 ────────*/
    void Update()
    {
        if (gizmoCamera && gizmoCamera.orthographic)
        {
            float sc = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(sc) > 0.001f)
                gizmoCamera.orthographicSize = Mathf.Clamp(
                    gizmoCamera.orthographicSize - sc * zoomSpeed, minZoom, maxZoom);
        }

        if (Input.GetMouseButtonDown(1)) lastMouse = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMouse;
            Vector3 mov = gizmoCamera.ScreenToWorldPoint(lastMouse) -
                            gizmoCamera.ScreenToWorldPoint(lastMouse + delta);
            gizmoCamera.transform.position += new Vector3(mov.x, mov.y, 0);
            lastMouse = Input.mousePosition;
        }
    }

    /*──────── 生成新物体 ────────*/

    public void SpawnPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Workshop/" + prefabName);
        if (!prefab)
        {
            Debug.LogError($"❌ Prefab {prefabName} 不在 Resources/Prefabs/Workshop/");
            return;
        }

        // 包装 Wrapper
        string wrapperName = prefabName + "_Wrapper";
        GameObject wrapper = new GameObject(wrapperName);
        wrapper.transform.SetParent(spawnParent, true);

        // 放到相机中心
        Vector3 center = gizmoCamera.ViewportToWorldPoint(
            new Vector3(0.5f, 0.5f, gizmoCamera.nearClipPlane + 2f));
        center.z = 1f;
        wrapper.transform.position = center;

        // 实例化 prefab 作为子物体
        GameObject child = Instantiate(prefab, wrapper.transform);
        child.name = prefabName;
        child.transform.localPosition = Vector3.zero;

        // 禁用物理脚本 & 添加碰撞盒
        PhysicsScriptDisabler.Disable(child);
        WrapperColliderUtils.AddBoxColliderToWrapper(wrapper, child);

        // 刷新 UI
        hierarchy?.Refresh();
        inspector?.Inspect(wrapper);

        Debug.Log($"✅ 已生成 {wrapper.name}");
    }

}
