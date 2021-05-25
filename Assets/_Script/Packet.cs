using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packet
{
    // type：1001， des：描述连接状态，         Data：ConnectionState
    // type：1002， des：阅读完成，请求坐标，    Data：空
    // type：1003， des：返回坐标集合           Data: ScreenPos
    public int Type;
    public string Data;
}

public class ConnectionState
{
    //1:已连接 2：已断线
    public int State;
}

public class ScreenPos
{
    public Vector2[] vects;
}






