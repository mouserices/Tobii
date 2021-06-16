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
        
        /*List<Point> points = new List<Point>();
        points.Add(new Point("1","2"));
        points.Add(new Point("1","3"));
        points.Add(new Point("1","4"));*/

        /*Point point = new Point();
        point.Vector2s = new List<vec>();
        point.Vector2s.Add(new vec(1,1));
        point.Vector2s.Add(new vec(2,1));
        point.Vector2s.Add(new vec(3,1));

        string json = JsonMapper.ToJson(point);
        Debug.Log(json);*/

        List<ScreenPos> _screenPosList = new List<ScreenPos>();
        for (int i = 0; i < 5; i++)
        {
            ScreenPos pos = new ScreenPos(i,2);
            _screenPosList.Add(pos);
        }
        
        JsonData json_vects = new JsonData();

        for (int i = 0; i < _screenPosList.Count; i++)
        {
            JsonData json_pos = new JsonData();
            json_pos["x"] = _screenPosList[i].x;
            json_pos["y"] = _screenPosList[i].y;
            json_vects.Add(json_pos);
        }
        
        JsonData jsonData = new JsonData();
        jsonData["vects"] = json_vects;
        
        Debug.Log("-----"+ jsonData.ToJson());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Point
{
    public List<vec> Vector2s;
}

public class vec
{
    public double x;
    public double y;

    public vec(double _x,double _y)
    {
        x = _x;
        y = _y;
    }
}
