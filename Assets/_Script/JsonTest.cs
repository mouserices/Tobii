using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        List<Point> points = new List<Point>();
        points.Add(new Point("1","2"));
        points.Add(new Point("1","3"));
        points.Add(new Point("1","4"));

        string json = JsonMapper.ToJson(points);
        Debug.Log(json);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Point
{
    public String X;
    public String Y;

    public Point(String x,String y)
    {
        X = x;
        Y = y;
    }
}
