using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;

#endif

public class LogCache
{
    public string MSzLogType;
    public bool Console;
    private readonly StringBuilder _mSzLog = new StringBuilder();

    //private int _count = 1;
    //public int Width = 20;

    public void Setup(string szLogType)
    {
        MSzLogType = szLogType;
    }

    public void PushLog(string szLog)
    {
        //_count = 1;
        _mSzLog.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
        _mSzLog.Append("\t");

        _mSzLog.Append(szLog);
        _mSzLog.Append("\n");

        if (_mSzLog.Length > 2000)
        {
            //m_szLog.Remove(0, 400);
        }

        //防止errorLog 太多，刷不出来
        if (MSzLogType.ToLower() == "error" && _mSzLog.Length > 1000)
        {
            _mSzLog.Remove(1000, _mSzLog.Length - 1000);
        }
    }

    public string GetAllLog()
    {
        return _mSzLog.ToString();
    }

    public void ClearLog()
    {
        _mSzLog.Remove(0, _mSzLog.Length);
    }
}

public class MyDebugger : MonoBehaviour
{
    public static MyDebugger Instance;

    /// <summary>
    /// 总开关
    /// </summary>
    public static bool IsOpen { get; set; }


    /// <summary>
    /// 显示log日志开关
    /// </summary>
    private bool _isLogShow = true;

    /// <summary>
    /// log日志输出文字大小
    /// </summary>
    private int _fontSize = 28;

    private LogCache _mPLogCache;

    #region 生命周期函数 ----------------------------

    internal void OnEnable()
    {
        Application.logMessageReceived += OnHandleLogEvent;
    }

    internal void OnDisable()
    {
        Application.logMessageReceived -= OnHandleLogEvent;
    }

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        //GameConfig.IsShowLog 【0】关闭 【1】开启
        //GameConfig.IsShowGM  【0】关闭 【1】开启
        if (Input.touchCount >= 3)
        {
            if (Input.touches[2].phase == TouchPhase.Began)
            {
                //手机上开启Log界面
                IsOpen = !IsOpen;
            }
        }

        if (Input.GetKeyUp("`"))
        {
            //pc上开启Log界面
            IsOpen = !IsOpen;
        }
    }

    #endregion --------------------------------------

    #region 事件返回 --------------------------------

    /// <summary>
    /// 错误堆栈时间返回
    /// </summary>
    /// <param name="logString">错误信息</param>
    /// <param name="stackTrace">跟踪堆栈</param>
    /// <param name="type">错误类型</param>
    private void OnHandleLogEvent(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            AddLog("Error", logString);
        }
    }

    public void OnApplicationQuit()
    {
        MDicLogCache.Clear();
        MDicLogCache = null;
    }

    #endregion --------------------------------------

    #region 接口和方法 ------------------------------

    public static Dictionary<string, LogCache> MDicLogCache = new Dictionary<string, LogCache>();

    private static LogCache GetLogCache(string szLogType)
    {
        if (MDicLogCache != null)
        {
            LogCache value;
            MDicLogCache.TryGetValue(szLogType, out value);
            return value;
        }

        return null;
    }

    private static void ClearLogCache(string szLogType)
    {
        LogCache value;
        MDicLogCache.TryGetValue(szLogType, out value);
        if (value != null)
        {
            value.ClearLog();
        }
    }

    public static void AddLog(string szLogType, string szLog)
    {
        LogCache pCache = GetLogCache(szLogType);
        if (pCache == null)
        {
            pCache = new LogCache();
            pCache.Setup(szLogType);
            if (MDicLogCache != null)
            {
                MDicLogCache.Add(szLogType, pCache);
            }
        }

        pCache.PushLog(szLog);

#if BLogShowConsole
        StringBuilder builder = new StringBuilder();
        builder.Append(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
        builder.Append("\t");
        builder.Append(szLog);
        builder.Append("\n");

        Debug.Log(builder.ToString());
#else
        bool isConsole = PlayerPrefs.GetInt(szLogType) == 1;
        pCache.Console = isConsole;

        if (szLogType.ToLower() == "error")
        {
            isConsole = true;
        }

        if (isConsole)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            builder.Append("\t");
            builder.Append(szLog);
            builder.Append("\n");

            Debug.Log(builder.ToString());
        }
#endif
    }

    #endregion --------------------------------------

    internal void OnGUI()
    {
        if (!IsOpen)
        {
            return;
        }

        GUI.Box(new Rect(0, 0, 2516, 2516), "");
        GUI.Box(new Rect(0, 0, 2516, 2516), "");

        //显示功能操作按钮
        LogFunctionBtnShow();

        GUI.skin.label.normal.textColor = GetCurrColorValue;

        //显示日志标签按钮
        LogTabBtnShow();

        if (_isLogShow)
        {
            //显示日志文字
            LogLabelShow();
        }

        if (GUI.Button(new Rect(Screen.width - 500f,20f,300f,100f),"模拟请求数据"))
        {
            Game.GetInstance().C2s_SendPos();
        }
    }

    #region 创建功能操作按钮 ------------------------

    private void LogFunctionBtnShow()
    {
        //适配缩放比（宽）
        float btnScaleWidth = Screen.width / 1920.0f;
        //适配缩放比（高）
        //float btnScaleHeight = Screen.height / 1080.0f;
        //按钮的宽
        float btnWidth = Screen.width * 0.1f;
        //按钮的高
        float btnHeight = Screen.height * 0.08f;

        GUIStyle style = new GUIStyle(GUI.skin.GetStyle("button")) {fontSize = 24 * (int) btnScaleWidth};

        float btnPosX = 20;
        float indexY = 110;

        // Rect rect = new Rect(btnPosX, btnHeight + indexY * 0, btnWidth, btnHeight);
        // if (GUI.Button(rect, "显示log", style))
        // {
        //     IsLogLabelShow();
        // }

        Rect rect = new Rect(btnPosX, btnHeight + indexY * 0, btnWidth, btnHeight);
        if (GUI.Button(rect, "清理所有", style))
        {
            LogLabelClearAll();
        }

        rect = new Rect(btnPosX, btnHeight + indexY * 1, btnWidth, btnHeight);
        if (GUI.Button(rect, "放大字体", style))
        {
            LogLabelFontSize(1);
        }

        rect = new Rect(btnPosX, btnHeight + indexY * 2, btnWidth, btnHeight);
        if (GUI.Button(rect, "缩小字体", style))
        {
            LogLabelFontSize(0);
        }

        rect = new Rect(btnPosX, btnHeight + indexY * 3, btnWidth, btnHeight);
        if (GUI.Button(rect, "输出为txt", style))
        {
            LogLabelSaveToTxt();
        }

        rect = new Rect(btnPosX, btnHeight + indexY * 4, btnWidth, btnHeight);
        if (GUI.Button(rect, "清屏", style))
        {
            LogLabelClear();
        }

        rect = new Rect(btnPosX, btnHeight + indexY * 5, btnWidth, btnHeight);
        if (GUI.Button(rect, "切换颜色", style))
        {
            LogLabelChangeColor();
        }
    }

    #endregion --------------------------------------

    #region 创建log页签按钮 -------------------------

    private void LogTabBtnShow()
    {
        //适配缩放比（宽）
        float btnScaleWidth = Screen.width / 1920.0f;
        //适配缩放比（高）
        float btnScaleHeight = Screen.height / 1080.0f;

        //计数
        int logIndex = 0;
        //按钮的宽
        float btnWidth = Screen.width * 0.08f;
        //按钮的高
        float btnHeight = Screen.height * 0.06f;
        //按钮起始坐标X位置
        float posX = 400.0f * btnScaleWidth;
        //按钮起始坐标Y位置
        float posY = 18.0f * btnScaleHeight;
        //一行按钮有几个
        int lineMax = 8;

        GUIStyle style = new GUIStyle(GUI.skin.GetStyle("button")) {fontSize = 22 * (int) btnScaleWidth};

        if (MDicLogCache != null && MDicLogCache.Count > 0)
        {
            foreach (var logCache in MDicLogCache)
            {
                LogCache pCache = logCache.Value;

                int nRow = logIndex % lineMax;
                int nLine = logIndex / lineMax;
                logIndex++;
                Rect rect = new Rect(posX + btnWidth * nRow, posY + btnHeight * nLine, btnWidth, btnHeight);
                if (GUI.Button(rect, pCache.MSzLogType, style))
                {
                    _mPLogCache = pCache;
                }
            }
        }
    }

    #endregion --------------------------------------

    #region 显示log日志 -----------------------------

    private Vector2 _mScroll = new Vector2(0, 0);

    /// <summary>
    /// 是否显示log开关
    /// </summary>
    public bool IsLogLabelShow()
    {
        _isLogShow = !_isLogShow;
        return _isLogShow;
    }

    /// <summary>
    /// GUI绘制log文字
    /// </summary>
    private void LogLabelShow()
    {
        GUIStyle gStyle = new GUIStyle
        {
            normal = {textColor = GetCurrColorValue},
            fontSize = this._fontSize,
            wordWrap = true
        };

        //总共有多少个标签
        int allCount = MDicLogCache == null ? 0 : MDicLogCache.Count;
        //标签的高度
        int btnHeight = (int) (Screen.height * 0.06f);
        //文字显示的位置
        float labelPosY = btnHeight * (allCount / 8.0f + 1.1f);

        GUILayout.BeginArea(new Rect(0, labelPosY, Screen.width / 1.2f, Screen.height - labelPosY));
        GUIStyle barStyle = new GUIStyle("verticalscrollbar");
        barStyle.fixedWidth = 50;
        _mScroll = GUILayout.BeginScrollView(_mScroll, false, false, null, barStyle);

        if (_mPLogCache != null)
        {
            GUILayout.Label(_mPLogCache.GetAllLog(), gStyle);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    #endregion --------------------------------------

    #region 清屏 ------------------------------------

    /// <summary>
    /// 清屏
    /// </summary>
    public void LogLabelClear()
    {
        ClearLogCache(_mPLogCache.MSzLogType);
    }

    /// <summary>
    /// 清理所有的页签
    /// </summary>
    public void LogLabelClearAll()
    {
        foreach (var logCache in MDicLogCache)
        {
            ClearLogCache(logCache.Key);
        }

        MDicLogCache.Clear();
    }

    #endregion --------------------------------------

    #region 控制log文字的大小 -----------------------

    /// <summary>
    /// 控制log文字的大小
    /// sizeType【0】缩小【1】放大
    /// </summary>
    public void LogLabelFontSize(int sizeType)
    {
        if (sizeType == 0)
        {
            this._fontSize -= 2;
        }
        else
        {
            this._fontSize += 2;
        }
    }

    #endregion --------------------------------------

    #region 获取当前log文字颜色 ---------------------

    private int _currColorId = 2;

    //红
    private readonly Color _color01 = new Color(255.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);

    //橙
    private readonly Color _color02 = new Color(249.0f / 255.0f, 111.0f / 255.0f, 0.0f / 255.0f);

    //黄
    private readonly Color _color03 = new Color(255.0f / 255.0f, 255.0f / 255.0f, 0.0f / 255.0f);

    //绿
    private readonly Color _color04 = new Color(0.0f / 255.0f, 128.0f / 255.0f, 0.0f / 255.0f);

    //蓝
    private readonly Color _color05 = new Color(0.0f / 255.0f, 0.0f / 255.0f, 255.0f / 255.0f);

    //紫
    private readonly Color _color06 = new Color(230.0f / 255.0f, 39.0f / 255.0f, 64.0f / 255.0f);

    //黑
    private readonly Color _color07 = new Color(0.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f);

    private Color GetCurrColorValue
    {
        get
        {
            Color color = _color01;
            if (_currColorId == 0)
            {
                color = _color01;
            }
            else if (_currColorId == 1)
            {
                color = _color02;
            }
            else if (_currColorId == 2)
            {
                color = _color03;
            }
            else if (_currColorId == 3)
            {
                color = _color04;
            }
            else if (_currColorId == 4)
            {
                color = _color05;
            }
            else if (_currColorId == 5)
            {
                color = _color06;
            }
            else if (_currColorId == 6)
            {
                color = _color07;
            }

            return color;
        }
    }

    /// <summary>
    /// 切换文字颜色
    /// </summary>
    public void LogLabelChangeColor()
    {
        _currColorId++;
        if (_currColorId > 6)
        {
            _currColorId = 0;
        }
    }

    #endregion --------------------------------------

    #region 将日志写入txt文件，保存到本地 -----------

    public void LogLabelSaveToTxt()
    {
        if (_mPLogCache != null)
        {
            string log = _mPLogCache.GetAllLog();

            string path = Application.persistentDataPath + "/Log/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.WriteAllText(string.Format("{0}{1}.txt", path, _mPLogCache.MSzLogType), log, Encoding.Default);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.RevealInFinder(path);
#endif
        }
    }

    #endregion --------------------------------------

    #region Debugger-Editor -------------------------

    public static void OnLogStateChanged()
    {
        ClearConsole();
        if (MDicLogCache == null)
        {
            return;
        }

        foreach (var item in MDicLogCache)
        {
            if (item.Value.Console)
            {
                PlayerPrefs.SetInt(item.Value.MSzLogType, 1);

                string[] logs = item.Value.GetAllLog().Split('\n');

                foreach (var lStr in logs)
                {
                    Debug.Log(lStr);
                }
            }
            else
            {
                PlayerPrefs.SetInt(item.Value.MSzLogType, 0);
            }
        }
    }

    public static void ClearConsole()
    {
#if UNITY_EDITOR
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        if (clearConsoleMethod != null)
        {
            clearConsoleMethod.Invoke(new object(), null);
        }
#endif
    }

    #endregion --------------------------------------
}