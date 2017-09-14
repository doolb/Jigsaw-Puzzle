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

    /// <summary>
    /// 视口控制对象
    /// </summary>
    public static ViewCameraControl viewControl;

    /// <summary>
    /// 数据管理对象
    /// </summary>
    public static GameDataManager dataManager;

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        // 加载场景
        LoadScene();

        // 建立事件的联系
        BuildEvent();

        // 加载用户数据
        LoadData();
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

        // 加载视口控制
        LoadView();

        // 加载游戏
        LoadGame();

        // 加载 UI
        LoadUI();

    }

    /// <summary>
    /// 加载游戏
    /// </summary>
    void LoadGame()
    {
        // 加载 拼图游戏 预制体，并挂载游戏脚本
        puzzleGame = Instantiate<GameObject>(Resources.Load("Puzzle") as GameObject).
            AddComponent<PuzzleGame>();

        // 暂停游戏
        puzzleGame.Pause();
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


    #region 关联事件

    void BuildEvent()
    {
        MenuEvent();

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
            // 获取是否显示视口
            bool show = UIToggle.current.value;

            // 切换视口显示
            viewControl.Toggle(show);

            // 转换ui显示
            uiControl.ToggleView(show);
        }));



        // 注册 菜单控制 事件
        uiControl.onShowMenu.Add(new EventDelegate(menuControl.Show));

        
        // 游戏结束 时更新菜单控制
        puzzleGame.onGameEnd.Add(new EventDelegate(() => menuControl.UpdateButton(false)));

        // 注册 游戏结束 事件
        puzzleGame.onGameEnd.Add(new EventDelegate(() => 
            {
                string msg = puzzleGame.record.ToString();

                // 显示结束界面
                uiControl.ShowFinish(msg);

                // 保存纪录
                dataManager.gameData.records.Add(msg);
            }));

        // 结束界面 显示结束后，显示 纪录
        uiControl.onFinishEnd.Add(new EventDelegate(() => recordControl.Show<Record>(puzzleGame.record)));

        // 关闭纪录界面后显示菜单
        recordControl.onHide.Add(new EventDelegate(menuControl.Show));
    }

    /// <summary>
    /// 关联菜单事件
    /// </summary>
    void MenuEvent()
    {
        // 开始游戏
        menuControl.startButton.onClick.Add(new EventDelegate(() =>
            {
                // 隐藏菜单
                menuControl.Hide();

                // 开始游戏
                puzzleGame.StartGame();

                // 更新按钮文本
                menuControl.UpdateButton(puzzleGame.canContinue);

                // 显示菜单按钮
                uiControl.Show();

            }));

        // 重新开始游戏
        menuControl.restartButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(puzzleGame.ReStart));

        // 更改背景
        menuControl.backgroundList.onChange.Add(new EventDelegate(()=>
            {
                // 获取背景名
                string name = UIPopupList.current.value;

                // 更新背景
                puzzleGame.SetBackground(name);

                // 保存背景名
                dataManager.gameData.background = name;
            }));

        // 切换原图显示
        menuControl.imageToggle.onChange.Add(new EventDelegate(() =>
            {
                // 获取设置
                bool show = UIToggle.current.value;
                
                // 切换原图显示
                puzzleGame.ToggleImage(show);

                // 保存设置
                dataManager.gameData.showImage = show;
            }));

        // 转换拼图旋转
        menuControl.rotateToggle.onChange.Add(new EventDelegate(() =>
            {
                // 获取设置
                bool rotate = UIToggle.current.value;

                // 转换拼图旋转
                puzzleGame.ToggleRotate(rotate);

                // 保存设置
                dataManager.gameData.rotatePuzzle = rotate;
                menuControl.UpdateButton(puzzleGame.canContinue);
            }));

        // 设置拼图个数
        menuControl.countList.onChange.Add(new EventDelegate(() =>
            {
                // 获取个数
                string s = UIPopupList.current.value;
                int i = int.Parse(s);
                Count count = GetPieceCount(i);

                // 设置个数
                puzzleGame.SetPieceCount(count);

                // 保存个数
                dataManager.gameData.pieceCount = i;

                menuControl.UpdateButton(puzzleGame.canContinue);
            }));

        // 设置拼图图像
        menuControl.imageList.onChange.Add(new EventDelegate(() =>
            {
                // 获取名字
                string img = UIPopupList.current.value;

                // 设置图像
                puzzleGame.SetPieceImage(img);

                // 保存名字
                dataManager.gameData.pieceImage = img;
            }));

        menuControl.shapeList.onChange.Add(new EventDelegate(() =>
            {
                // 获取名字
                string name = UIPopupList.current.value;

                puzzleGame.SetPieceShape(name);

                // 
                dataManager.gameData.pieceShape = name;
            }));

        menuControl.styleList.onChange.Add(new EventDelegate(() =>
            {
                string name = UIPopupList.current.value;

                puzzleGame.SetPieceStyle(name);

                dataManager.gameData.pieceStyle = name;
            }));


        menuControl.showAllToggle.onChange.Add(new EventDelegate(() =>
            {
                puzzleGame.ShowAllOrNot(UIToggle.current.value);
            }));

        menuControl.tileButton.onClick.Add(new EventDelegate(puzzleGame.TilePiece));
    }
    #endregion

    /// <summary>
    /// 加载用户数据
    /// </summary>
    void LoadData()
    {
        // 挂载数据管理脚本
        dataManager = gameObject.AddComponent<GameDataManager>();

        menuControl.countList.value = dataManager.gameData.pieceCount.ToString();
        menuControl.imageList.value = dataManager.gameData.pieceImage;
        menuControl.shapeList.value = dataManager.gameData.pieceShape;
        menuControl.styleList.value = dataManager.gameData.pieceStyle;
        menuControl.rotateToggle.value = dataManager.gameData.rotatePuzzle;

        menuControl.backgroundList.value = dataManager.gameData.background;

        recordControl.Add<string>(dataManager.gameData.records);
    }


    void OnDestroy()
    {
        dataManager.Save();
    }


    /// <summary>
    /// 把拼图总个数，转换为 两个数的乘积
    /// </summary>
    /// <param name="count">要转换个数</param>
    /// <returns></returns>
    Count GetPieceCount(int count)
    {
        // 默认 为 （6，4）
        int x = 6, y = 4;
        switch (count)
        {
            // 24 = 6 x 4
            case 24: x = 6; y = 4; break;

            // 48 = 8 x 6
            case 48: x = 8; y = 6; break;

            // 63 = 9 x 7
            case 63: x = 9; y = 7; break;

            // 108 = 12 x 9
            case 108: x = 12; y = 9; break;

            // 192 = 16 x 12
            case 192: x = 16; y = 12; break;

            // 300 = 25 x 12
            case 300: x = 25; y = 12; break;

            // 520 = 26 x 20
            case 520: x = 26; y = 20; break;

            // 768 = 32 x 24
            case 768: x = 32; y = 24; break;
        }

        // 返回个数
        return new Count(x, y);
    }

}
