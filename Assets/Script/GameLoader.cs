using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏加载器
/// </summary>
public class GameLoader : MonoBehaviour
{
    /// <summary>
    /// ui 根对象
    /// </summary>
    public GameObject uiRoot;

    /// <summary>
    /// 3d ui 根对象
    /// </summary>
    public GameObject uiRoot3D;

    /// <summary>
    /// 游戏 ui root 对象
    /// </summary>
    public GameObject uiRootGame;

    /// <summary>
    /// 视口 ui root 对象
    /// </summary>
    public GameObject uiRootView;


    /// <summary>
    /// 单一实例
    /// </summary>
    public static GameLoader instance;


    /// <summary>
    /// ui 控制脚本
    /// </summary>
    [HideInInspector]
    public UIControl uiControl;

    /// <summary>
    /// 菜单控制脚本
    /// </summary>
    [HideInInspector]
    public MenuControl menuControl;

    /// <summary>
    /// 记录控制脚本
    /// </summary>
    [HideInInspector]
    public RecordControl recordControl;

    /// <summary>
    /// 视口控制脚本
    /// </summary>
    [HideInInspector]
    public ViewControl viewControl;

    /// <summary>
    /// 游戏控制脚本
    /// </summary>
    [HideInInspector]
    public PuzzleGame puzzleGame;


    /// <summary>
    /// 数据管理对象
    /// </summary>
    [HideInInspector]
    public GameDataManager dataManager;

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        if (!enabled) return;

        // 启动单实例
        if (instance == null) instance = this;

        if (instance != null && instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);



        // 挂载数据管理脚本
        dataManager = gameObject.AddComponent<GameDataManager>();



        // 加载场景
        LoadScene();

    }

    /// <summary>
    /// 固定时间执行
    /// </summary>
    void FixedUpdate()
    {
        // 显示时间
        uiControl.ShowTime(puzzleGame.gameTime);
    }


    #region 加载场景

    /// <summary>
    /// 加载场景
    /// </summary>
    void LoadScene()
    {
        // 加载 UI
        LoadUI();

        // 加载游戏
        LoadGame();

        // 加载视口
        LoadView();

        // 初始化 菜单控制脚本
        menuControl.Init();

        // 初始化 UI控制脚本
        uiControl.Init();

        // 暂停游戏
        puzzleGame.Pause();
    }

    /// <summary>
    /// 加载游戏
    /// </summary>
    void LoadGame()
    {
        // 加载 拼图游戏 预制体，并挂载游戏脚本
        puzzleGame = NGUITools.AddChild(uiRootGame,Resources.Load<GameObject>("Puzzle")).AddComponent<PuzzleGame>();

        // 设置游戏摄像机
        puzzleGame.cam = uiRootGame.transform.Find("Camera").GetComponent<Camera>();    
    }

    /// <summary>
    /// 加载 UI
    /// </summary>
    void LoadUI()
    {
        // 加载 游戏 菜单 预制体
        menuControl = NGUITools.AddChild(uiRoot3D, Resources.Load<GameObject>("Panel - Menu")).AddComponent<MenuControl>();

        // 加载 UI 预制体
        uiControl = NGUITools.AddChild(uiRoot, Resources.Load<GameObject>("Panel - UI")).AddComponent<UIControl>();


        // 加载 显示纪录 预制体
        recordControl = NGUITools.AddChild(uiRoot, Resources.Load<GameObject>("Panel - Record")).AddComponent<RecordControl>();

    }

    /// <summary>
    /// 加载视口
    /// </summary>
    void LoadView()
    {
        viewControl = NGUITools.AddChild(uiRootView, Resources.Load<GameObject>("View Control")).AddComponent<ViewControl>();
    }

    #endregion
}
