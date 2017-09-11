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

    /// <summary>
    /// 游戏背景
    /// </summary>
    public static Background background;

    /// <summary>
    /// ui 控制脚本
    /// </summary>
    public static UIControl uiControl;

    /// <summary>
    /// 菜单控制脚本
    /// </summary>
    public static MenuControl menuControl;

    /// <summary>
    /// 记录控制脚本
    /// </summary>
    public static RecordControl recordControl;

    /// <summary>
    /// 游戏控制脚本
    /// </summary>
    public static PuzzleGame puzzleGame;

    public static ViewCameraControl viewControl;


    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        // 加载场景
        LoadScene();
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    void LoadScene()
    {
        // 加载背景
        LoadBackground();

        // 加载游戏
        LoadGame();

        // 加载视口控制
        LoadView();

        // 加载 UI
        LoadUI();

        // 建立事件的联系
        BuildEvent();

    }

    /// <summary>
    /// 加载背景
    /// </summary>
    void LoadBackground()
    {
        // 加载背景预制体, 并挂载控制脚本
        background = Instantiate<GameObject>(Resources.Load("Background") as GameObject).
            AddComponent<Background>();
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


    void BuildEvent()
    {
        // 获取视口显示对象
        Transform viewWindow = uiControl.transform.Find("View Window");

        // 关联视口控制事件
        viewWindow.Find("Zoom Bar").GetComponent<UISlider>().onChange.Add(new EventDelegate(viewControl.OnZoom));

        // 获取视口显示组件
        UIViewport viewport = GetComponent<UIViewport>();

        // 关联左上角
        viewport.topLeft = viewWindow.transform.Find("TopLeft");

        // 关联右下角
        viewport.bottomRight = viewWindow.transform.Find("BottomRight");


        // 关联视口显示和隐藏
        uiControl.transform.Find("Check Box - View").GetComponent<UIToggle>().onChange.Add(new EventDelegate(() =>
            {
                bool show = UIToggle.current.value;
                viewControl.Toggle(show);

                uiControl.ToggleView(show);
            }));



        // 注册 菜单控制 事件
        uiControl.onShowMenu.Add(new EventDelegate(menuControl.Show));

        // 注册开始游戏事件
        menuControl.onStartGame.Add(new EventDelegate(uiControl.Show));


        // 注册 游戏结束 事件
        puzzleGame.onGameEnd.Add(new EventDelegate(() => uiControl.ShowFinish(puzzleGame.record.ToString())));

        // 结束界面 显示结束后，显示 纪录
        uiControl.onFinishEnd.Add(new EventDelegate(() => recordControl.Show<Record>(puzzleGame.record)));

        // 关闭纪录界面后显示菜单
        recordControl.onHide.Add(new EventDelegate(menuControl.Show));
    }

    // 加载视图
    void LoadView()
    {
        // 加载视图预制体，并挂载脚本
        viewControl = Instantiate<GameObject>(Resources.Load<GameObject>("Main Camera Control")).AddComponent<ViewCameraControl>();

        // 设置 摄像头
        viewControl.cam = GetComponent<Camera>();

    }
}
