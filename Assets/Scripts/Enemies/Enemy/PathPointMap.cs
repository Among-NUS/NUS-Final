using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPointMap : MonoBehaviour
{
    public List<GameObject> pathPointObjects = new List<GameObject>();//�洢��ȫͼ��·���㣬���˿��Բ�ѯ,ͼ�е�ĳ��������Ҫ�������ݵģ�����ֵ+50
    public MyGraph graph;
    public int level = 1;
    void Start()
    {//����ѡ��ͳһ��inspector��json���޸�
        //TextAsset jsonFile = Resources.Load<TextAsset>("Maps/Level"+level);
        //graph = JsonUtility.FromJson<MyGraph>(jsonFile.text);

        //Debug.Log($"��ͼ: {level}");
        //foreach (Node pathPoint in graph.nodes)
        //{
        //    Debug.Log($"pathP {pathPoint.index} neightbor is " +
        //        $"{pathPoint.neightbor[0]},{pathPoint.neightbor[1]},{pathPoint.neightbor[2]},{pathPoint.neightbor[3]},");
        //}
    }
}
[System.Serializable]
public class MyGraph
{
    public List<Node> nodes = new List<Node>();
}
[System.Serializable]
public class Node
{
    public int index;
    public int[] neightbor = new int[4];//-1 represent no connection
}
