using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public static Dictionary<int,Transform[]> s_points = new Dictionary<int, Transform[]>();
    public Transform[] t1points;
    public Transform[] t2points;

    public static int NextPoint;

    private void Awake()
    {
        s_points.Add(1,t1points);
        s_points.Add(2,t2points);
        NextPoint = 0;
    }

    public static Transform GetSpawnPoint(int teamdex)
    {
        if (s_points == null)
            return null;
        if (NextPoint >= s_points[teamdex].Length)
        {
            NextPoint = 0;
        }
        int val = NextPoint;
        NextPoint++;
        return s_points[teamdex][val];
    }
}
