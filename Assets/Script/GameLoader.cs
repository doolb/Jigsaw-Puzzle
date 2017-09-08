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

    public Camera viewcam;

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

        puzzleGame.cam = viewcam;
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

        // 注册 菜单控制 事件
        uiControl.onShowMenu.Add(new EventDelegate(menuControl.Show));

        // 注册 游戏结束 事件
        puzzleGame.onGameEnd.Add(new EventDelegate(() => uiControl.ShowFinish(puzzleGame.record.ToString())));


        // 结束界面 显示结束后，显示 纪录
        uiControl.onFinishEnd.Add(new EventDelegate(() => recordControl.Show<Record>(puzzleGame.record)));

        // 关闭纪录界面后显示菜单
        recordControl.onHide.Add(new EventDelegate(menuControl.Show));
    }
}
