using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏加载器
/// </summary>
public class GameLoader : MonoBehaviour
{
    /// <summary>
    /// ngui ui 根对象
    /// </summary>
    public GameObject uiRoot;

    /// <summary>
    /// ngui 3d ui 根对象
    /// </summary>
    public GameObject uiRoot3D;


    public static GameLoader instance;

    /// <summary>
    /// ui 控制脚本
    /// </summary>
    public UIControl uiControl;

    /// <summary>
    /// 菜单控制脚本
    /// </summary>
    public MenuControl menuControl;

    /// <summary>
    /// 记录控制脚本
    /// </summary>
    public RecordControl recordControl;

    /// <summary>
    /// 游戏控制脚本
    /// </summary>
    public PuzzleGame puzzleGame;

    /// <summary>
    /// 视口控制对象
    /// </summary>
    public ViewCameraControl viewControl;

    /// <summary>
    /// 数据管理对象
    /// </summary>
    public GameDataManager dataManager;

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
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

        // 加载视口控制
        LoadView();

        // 加载游戏
        LoadGame();

        // 初始化 菜单控制脚本
        menuControl.Init();

        // 初始化 UI控制脚本
        uiControl.Init();

        // 初始化 视口控制脚本
        viewControl.Init(transform, uiRoot.transform.Find("Panel - UI"));

        // 暂停游戏
        puzzleGame.Pause();
    }

    /// <summary>
    /// 加载游戏
    /// </summary>
    void LoadGame()
    {
        // 加载 拼图游戏 预制体，并挂载游戏脚本
        puzzleGame = Instantiate<GameObject>(Resources.Load("Puzzle") as GameObject).
            AddComponent<PuzzleGame>();

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



    // 加载视图
    void LoadView()
    {
        // 加载视图预制体，并挂载脚本
        viewControl = Instantiate<GameObject>(Resources.Load<GameObject>("Main Camera Control")).AddComponent<ViewCameraControl>();

        // 设置 摄像头
        viewControl.cam = GetComponent<Camera>();

    }
    #endregion
}
