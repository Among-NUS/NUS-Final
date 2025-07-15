using UnityEngine;

[ExecuteAlways]
public class GridRenderer : MonoBehaviour
{
    public Color lineColor = Color.gray;
    public float gridSpacing = 1f;
    public float lineThickness = 0.5f;

    private Material lineMaterial;
    private Camera cam;

    void OnEnable()
    {
        cam = Camera.main;
        CreateLineMaterial();
    }

    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            // 打开 alpha 混合
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnPostRender()
    {
        if (!lineMaterial || cam == null) return;

        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        Vector3 camPos = cam.transform.position;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float left = camPos.x - camWidth / 2f;
        float right = camPos.x + camWidth / 2f;
        float bottom = camPos.y - camHeight / 2f;
        float top = camPos.y + camHeight / 2f;

        // 垂直线
        for (float x = Mathf.Floor(left / gridSpacing) * gridSpacing; x < right; x += gridSpacing)
        {
            GL.Vertex(new Vector3(x, bottom, 0));
            GL.Vertex(new Vector3(x, top, 0));
        }

        // 水平线
        for (float y = Mathf.Floor(bottom / gridSpacing) * gridSpacing; y < top; y += gridSpacing)
        {
            GL.Vertex(new Vector3(left, y, 0));
            GL.Vertex(new Vector3(right, y, 0));
        }

        GL.End();
    }
}
