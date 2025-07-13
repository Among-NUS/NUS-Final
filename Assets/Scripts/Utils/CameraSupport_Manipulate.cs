//// CameraSupport_Manipulate.cs
//using UnityEngine;

//public class CameraSupport_Manipulate : MonoBehaviour
//{
//    [Header("Camera Target")]
//    public Transform FollowTarget;

//    [Header("Camera Region (0~1)")]
//    public Vector2 FollowRegion = new Vector2(0.8f, 0.8f);

//    [Header("Look-ahead (正前方距离)")]
//    public float LookAheadX = 2f;

//    private CameraSupport cameraSupport;

//    void Awake()
//    {
//        cameraSupport = GetComponent<CameraSupport>();
//    }

//    void FixedUpdate()
//    {
//        if (FollowTarget != null)
//        {
//            // 若目标身上有 HeroBehaviour，按朝向加一个前视偏移
//            if (FollowTarget.TryGetComponent(out HeroBehaviour hero))
//            {
//                float dir = hero.FacingLeft ? -1f : 1f;
//                Vector3 aheadPos = hero.transform.position + new Vector3(dir * LookAheadX, 0f, 0f);
//                cameraSupport.PushCameraByPos(aheadPos, FollowRegion);
//            }
//            else
//            {
//                cameraSupport.PushCameraByPos(FollowTarget.position, FollowRegion);
//            }
//        }
//    }

//    public void MoveTo(float x, float y)
//    {
//        cameraSupport.SetCameraPosition(x, y);
//    }
//}
