using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class PathPointMap : MonoBehaviour
{
    public List<GameObject> pathPointObjects = new List<GameObject>();//存储了全图的路径点，敌人可以查询,图中的某条边是需要乘坐电梯的，则数值+50
    public MyGraph graph;
    public int level = 1;
    void Start()
    {//可以选择统一在inspector或json中修改
        //TextAsset jsonFile = Resources.Load<TextAsset>("Maps/Level"+level);
        //graph = JsonUtility.FromJson<MyGraph>(jsonFile.text);

        //Debug.Log($"地图: {level}");
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
