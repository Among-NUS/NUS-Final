using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : MonoBehaviour
{
    [Header("auto initialize")]
    public float scanFan = 40f;
    public float rotateSpeed = 45f;//in degree
    public float rotateRange = 180f;
    public float scanLength = 10f;
    public ScanMode sm = ScanMode.BACKANDFORTH;
    public int numberOfRay = 2;

    [Header("auto update")]
    [SerializeField]
    List<GameObject> capturedTarget = new List<GameObject>();
    [SerializeField]
    List<RaycastHit2D> hits = new List<RaycastHit2D>();

    // Start is called before the first frame update
    void Start()
    {
        numberOfRay = Mathf.Max(2,numberOfRay);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        castRay();
        isSwitch();
    }
    void rotateBackAndForth()
    {

    }
    void castRay()
    {
        float gap = scanFan / (numberOfRay-1);
        float initAngle = transform.rotation.eulerAngles.z - scanFan/2;
        float angleRad;
        Vector3 angleVector;
        hits.Clear();
        for (int i=0;i<numberOfRay;i++)
        {//顺序的扫描
            angleRad = (initAngle + gap * i)*Mathf.Deg2Rad;
            angleVector = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad),0);
            hits.Add(Physics2D.Raycast(transform.position,angleVector,scanLength));
            Debug.DrawLine(transform.position, transform.position + scanLength * angleVector,Color.red);
        }
        checkScanResult();
    }
    void checkScanResult()
    {   
        List<GameObject> newTarget = new List<GameObject>();
        for (int i=0;i<numberOfRay;i++)
        {
            if (hits[i].collider!=null)
            {
                if ((hits[i].collider.gameObject.GetComponent<HeroBehaviour>() != null))
                {
                    if (newTarget.Count == 0)
                    {
                        newTarget.Add(hits[i].collider.gameObject);
                    }
                    else if (!newTarget.Contains(hits[i].collider.gameObject))
                    {
                        newTarget.Add(hits[i].collider.gameObject);
                    }
                }
            }
        }
        capturedTarget = newTarget;
    }
    void isSwitch()//需要这个对象有switch模块
    {
        if (GetComponent<Switch>()!=null)
        {
            GetComponent<Switch>().isOn = (capturedTarget.Count != 0);//当检测到物体时激活开关
        }
    }
    public enum ScanMode
    {
        BACKANDFORTH
    }
    public List<GameObject> getCapturedTarget()
    {
        return capturedTarget;
    }
}
