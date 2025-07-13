using UnityEngine;

public class CameraZoomEffect : MonoBehaviour
{
    [Header("相机缩放设置")]
    public float zoomInSize = 2f;
    public float zoomDuration = 0.5f;
    public float holdDuration = 1.0f;

    [Header("主角透明度设置")]
    [Range(0f, 1f)]
    public float heroMinAlpha = 0.3f;

    public System.Action OnZoomInFinished;

    private Camera cam;
    private float originalSize;
    private float zoomTimer = 0f;
    private float holdTimer = 0f;

    private enum ZoomState { Idle, ZoomingIn, FadingOut, Holding, FadingIn, ZoomingOut }
    private ZoomState state = ZoomState.Idle;

    private GameObject hero;
    private SpriteRenderer heroSR;

    private float fadeStartAlpha;
    private float fadeTargetAlpha;
    private float fadeTimer = 0f;
    private float fadeDuration = 0f;
    private bool isFading = false;

    void Start()
    {
        cam = Camera.main;
        originalSize = cam.orthographicSize;

        hero = GameObject.FindGameObjectWithTag("Player");
        if (hero != null)
            heroSR = hero.GetComponent<SpriteRenderer>();
    }

    public void ZoomIn()
    {
        if (state != ZoomState.Idle) return;

        zoomTimer = 0f;
        state = ZoomState.ZoomingIn;
        Time.timeScale = 0f;
    }

    void StartFade(float from, float to, float duration)
    {
        if (heroSR == null) return;

        fadeStartAlpha = from;
        fadeTargetAlpha = to;
        fadeDuration = duration;
        fadeTimer = 0f;
        isFading = true;
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        switch (state)
        {
            case ZoomState.ZoomingIn:
                zoomTimer += dt;
                float tIn = Mathf.Clamp01(zoomTimer / zoomDuration);
                cam.orthographicSize = Mathf.Lerp(originalSize, zoomInSize, tIn);
                if (tIn >= 1f)
                {
                    state = ZoomState.FadingOut;
                    StartFade(1f, heroMinAlpha, zoomDuration); // 主角淡出
                }
                break;

            case ZoomState.FadingOut:
                if (!isFading)
                {
                    state = ZoomState.Holding;
                    holdTimer = 0f;
                    OnZoomInFinished?.Invoke(); // 放大 & 淡出完成，通知 GameManager
                }
                break;

            case ZoomState.Holding:
                holdTimer += dt;
                if (holdTimer >= holdDuration)
                {
                    state = ZoomState.FadingIn;
                    StartFade(heroMinAlpha, 1f, zoomDuration); // 主角淡入
                }
                break;

            case ZoomState.FadingIn:
                if (!isFading)
                {
                    state = ZoomState.ZoomingOut;
                    zoomTimer = 0f;
                }
                break;

            case ZoomState.ZoomingOut:
                zoomTimer += dt;
                float tOut = Mathf.Clamp01(zoomTimer / zoomDuration);
                cam.orthographicSize = Mathf.Lerp(zoomInSize, originalSize, tOut);
                if (tOut >= 1f)
                {
                    state = ZoomState.Idle;
                    Time.timeScale = 1f;
                }
                break;
        }

        // 淡入/淡出动画
        if (isFading && heroSR != null)
        {
            fadeTimer += dt;
            float ft = Mathf.Clamp01(fadeTimer / fadeDuration);
            float alpha = Mathf.Lerp(fadeStartAlpha, fadeTargetAlpha, ft);
            var c = heroSR.color;
            heroSR.color = new Color(c.r, c.g, c.b, alpha);
            if (ft >= 1f) isFading = false;
        }
    }
}
