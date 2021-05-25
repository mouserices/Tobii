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
            //return;
        }
        
        #region 检测设备连接状态

        bool gazeIsValid = getGazeIsValid();
        if (gazeIsValid && !_isGazeValid)
        {
            _isGazeValid = true;
            C2s_TobillConnectionState(1);
        }else if (!gazeIsValid && _isGazeValid)
        {
            _isGazeValid = false;
            C2s_TobillConnectionState(0);
        }

        #endregion

        #region 收集坐标点

        GazePoint gazePoint = TobiiAPI.GetGazePoint();

        if (gazePoint.IsRecent()
            && gazePoint.Timestamp > (_lastGazePoint.Timestamp + float.Epsilon))
        {
            _lastGazePoint = gazePoint;
            ScreenPos screenPos = new ScreenPos(gazePoint.Screen.x, gazePoint.Screen.y);
            _screenPosList.Add(screenPos);
        }

        #endregion
    }

    /// <summary>
    /// 设备是否连接成功
    /// </summary>
    /// <returns></returns>
    private bool getGazeIsValid()
    {
        return TobiiAPI.GetGazePoint().IsRecent();
    }

    public void Reset()
    {
        _isGazeValid = false;
        _screenPosList = new List<ScreenPos>();
        _lastGazePoint = GazePoint.Invalid;
        
    }

    public void C2s_TobillConnectionState(int isConnected)
    {
        ConnectionState state = new ConnectionState();
        state.State = isConnected;
        
        string json = JsonMapper.ToJson(state);
        StartWebSocket.GetInstance().SendAsync((int)PacketType.CONNEC_STATE,json);
    }
    
    public void C2s_SendPos()
    {
        ScreenPoint screenPoint = new ScreenPoint();
        screenPoint.vects = _screenPosList;
        
        string json = JsonMapper.ToJson(screenPoint);
        StartWebSocket.GetInstance().SendAsync((int)PacketType.SEND_POS,json);
    }
}
