using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityWebSocket;

public class StartWebSocket : MonoBehaviour
{
    private WebSocket _webSocket;
    // Start is called before the first frame update
    void Start()
    {
        Create();
        ConnectAsync();
    }
    public void Create()
    {
        // 创建实例
        string address = "ws://localhost:7272";
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

    public void SendAsync(string str)
    {
        _webSocket.SendAsync(str);
    }
    
    public void SendAsync(byte[] bytes)
    {
        _webSocket.SendAsync(bytes);
    }

    public void CloseAsync()
    {
        _webSocket.CloseAsync();
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError(string.Format("WebSocket OnError, msg: {0}",e.Message));
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(string.Format("WebSocket OnMessage, msg: {0}",e.Data));
    }

    private void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log(string.Format("WebSocket OnClose, StatusCode: {0}",e.StatusCode));
    }

    private void OnOpen(object sender, OpenEventArgs e)
    {
        Debug.Log("WebSocket OnOpen");
        this.SendAsync("hello word");
    }
}
