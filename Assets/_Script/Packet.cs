using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packet
{
    // type：1001， des：描述连接状态，         Data：ConnectionState
    // type：1002， des：阅读完成，请求坐标，    Data：空
    // type：1003， des：返回坐标集合           Data: ScreenPoint
    // type: 1004,  des:重新开始阅读           Data：空
    public int Type;
    public string Data;
}

public enum PacketType
{
    CONNEC_STATE = 1001,
    REQUEST_POS = 1002,
    SEND_POS = 1003,
    RE_START = 1004,
}

public class ConnectionState
{
    //1:已连接 2：已断线
    public int State;
}

public class ScreenPoint
{
    public List<ScreenPos> vects;
}

public class ScreenPos
{
    public double x;
    public double y;

    public ScreenPos(double _x,double _y)
    {
        x = _x;
        y = _y;
    }
}






