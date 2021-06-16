using System.Collections;
using System.Collections.Generic;
using LitJson;
using Tobii.Gaming;
using UnityEngine;

public class Game : MonoBehaviour
{
    private static Game _instance;
    private bool _isGazeValid = false;
    private GazePoint _lastGazePoint;
    private List<ScreenPos> _screenPosList;
    private float timer = 0;

    public static Game GetInstance()
    {
        return _instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;

        StartWebSocket.GetInstance().Create();
        StartWebSocket.GetInstance().ConnectAsync();
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartWebSocket.GetInstance().GetIsConnected())
        {
            return;
        }

        #region 检测设备连接状态

        timer += Time.deltaTime;
        if (timer >= 2f)
        {
            timer = 0;
            bool isConnected = IsConnected();
            if (isConnected)
            {
                _isGazeValid = true;
                C2s_TobillConnectionState(1);
            }
            else if (!isConnected)
            {
                _isGazeValid = false;
                C2s_TobillConnectionState(0);
            }
        }

        #endregion

        #region 收集坐标点

        GazePoint gazePoint = TobiiAPI.GetGazePoint();

        //float.Epsilon
        if (gazePoint.IsRecent()
            && gazePoint.Timestamp > (_lastGazePoint.Timestamp + 1f))
        {
            _lastGazePoint = gazePoint;
            ScreenPos screenPos = new ScreenPos(Mathf.FloorToInt(gazePoint.Screen.x), Mathf.FloorToInt(gazePoint.Screen.y));
            _screenPosList.Add(screenPos);
        }

        #endregion
    }

    /// <summary>
    /// 设备是否连接成功
    /// </summary>
    /// <returns></returns>
    private bool IsConnected()
    {
        return TobiiAPI.IsConnected;
    }

    public void Reset()
    {
        _isGazeValid = false;
        _screenPosList = new List<ScreenPos>();
        _lastGazePoint = GazePoint.Invalid;
    }

    public void C2s_TobillConnectionState(int isConnected)
    {
        JsonData jsonData = new JsonData();
        jsonData["State"] = isConnected;
        StartWebSocket.GetInstance().SendAsync((int) PacketType.CONNEC_STATE, jsonData);
    }

    public void C2s_SendPos()
    {
        JsonData jsonData = new JsonData();

        for (int i = 0; i < _screenPosList.Count; i++)
        {
            JsonData json_pos = new JsonData();
            json_pos["x"] = _screenPosList[i].x;
            json_pos["y"] = _screenPosList[i].y;
            jsonData.Add(json_pos);
        }

        StartWebSocket.GetInstance().SendAsync((int) PacketType.SEND_POS, jsonData);
    }
}