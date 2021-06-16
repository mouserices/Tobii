using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityWebSocket;

public class StartWebSocket
{
    private static StartWebSocket _instance;
    private WebSocket _webSocket;

    private bool _connected = false;
    // Start is called before the first frame update

    public static StartWebSocket GetInstance()
    {
        if (_instance == null)
        {
            _instance = new StartWebSocket();
        }

        return _instance;
    }

    public void Create()
    {
        // 创建实例
        //ws://192.168.11.196:7272
        string address = "ws://39.100.136.208:8080/y/websocket/sid";
        _webSocket = new WebSocket(address);
        _webSocket.OnOpen += OnOpen;
        _webSocket.OnClose += OnClose;
        _webSocket.OnMessage += OnMessage;
        _webSocket.OnError += OnError;
    }

    public void ConnectAsync()
    {
        _webSocket.ConnectAsync();
    }

    public void SendAsync(int type,JsonData jData)
    {
        JsonData jsonData = new JsonData();
        jsonData["Type"] = type;
        jsonData["Data"] = jData;
        
        string json = jsonData.ToJson();
        MyDebugger.AddLog("log",string.Format("WebSocket SendAsync, msg: {0}",json));
        _webSocket.SendAsync(json);
    }
    
    public void SendAsync(byte[] bytes)
    {
        _webSocket.SendAsync(bytes);
    }

    public void CloseAsync()
    {
        _connected = false;
        _webSocket.CloseAsync();
    }

    public bool GetIsConnected()
    {
        return _connected;
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        _connected = false;
        MyDebugger.AddLog("log",string.Format("WebSocket OnError, msg: {0}",e.Message));
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        MyDebugger.AddLog("log",string.Format("WebSocket OnMessage, msg: {0}",e.Data));
        Packet packet = JsonUtility.FromJson<Packet>(e.Data);
        if (packet.Type == (int)PacketType.REQUEST_POS)
        {
            Game.GetInstance().C2s_SendPos();
        }
        else if (packet.Type == (int)PacketType.RE_START)
        {
            Game.GetInstance().Reset();
        }
    }

    private void OnClose(object sender, CloseEventArgs e)
    {
        _connected = false;
        MyDebugger.AddLog("log",string.Format("WebSocket OnClose, StatusCode: {0}",e.StatusCode));
    }

    private void OnOpen(object sender, OpenEventArgs e)
    {
        MyDebugger.AddLog("log","WebSocket OnOpen");
        _connected = true;
        Game.GetInstance().Reset();
    }
}
