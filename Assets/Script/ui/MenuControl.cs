using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏菜单控制脚本
/// </summary>
public class MenuControl : MonoBehaviour
{
    /// <summary>
    /// 游戏对象
    /// </summary>
    PuzzleGame game
    {
        get { return GameLoader.instance.puzzleGame; }
    }

    /// <summary>
    /// 数据对象
    /// </summary>
    GameData data
    {
        get { return GameLoader.instance.dataManager.gameData; }
    }

    /// <summary>
    /// 动画播放对象
    /// </summary>
    UIPlayAnimation playAnim;

    /// <summary>
    /// 记录菜单是否显示
    /// </summary>
    public bool show = true;

    /// <summary>
    /// 开始按钮 文本 对象
    /// </summary>
    UILocalize startLabel;

    /// <summary>
    /// 开始按钮 对象
    /// </summary>
    UIButton startButton;

    /// <summary>
    /// 重新开始游戏的按钮
    /// </summary>
    GameObject restartButton;

    /// <summary>
    /// 背景列表
    /// </summary>
    UIPopupList backgroundList;

    /// <summary>
    /// 个数列表
    /// </summary>
    UIPopupList countList;

    /// <summary>
    /// 图像列表
    /// </summary>
    UIPopupList imageList;

    /// <summary>
    /// 形状列表
    /// </summary>
    UIPopupList shapeList;

    /// <summary>
    /// 风格列表
    /// </summary>
    UIPopupList styleList;

    /// <summary>
    /// 是否显示原图
    /// </summary>
    UIToggle imageToggle;


    /// <summary>
    /// 拼图旋转转换
    /// </summary>
    UIToggle rotateToggle;

    /// <summary>
    /// 是否显示所有
    /// </summary>
    UIToggle showAllToggle;

    /// <summary>
    /// 平铺拼图按钮
    /// </summary>
    UIButton tileButton;


    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        // 获取动画播放对象
        playAnim = gameObject.AddComponent<UIPlayAnimation>();

        // 设置动画
        playAnim.target = GetComponent<Animation>();

        // 开始按钮
        Transform start = transform.Find("Button - Start");
        // 获取开始控制 对象
        startButton = start.GetComponent<UIButton>();
        // 获取开始按钮 文本对象
        startLabel = start.Find("Label").GetComponent<UILocalize>();


        // 重新开始按钮
        restartButton = transform.Find("Button - ReStart").gameObject;


        // 背景 列表
        backgroundList = transform.Find("Pop Up List - Background").GetComponent<UIPopupList>();



        // 是否 显示原始图像
        imageToggle = transform.Find("Check Box - Show Original Image").GetComponent<UIToggle>();



        // 是否 旋转拼图 
        rotateToggle = transform.Find("Check Box - Rotate Piece").GetComponent<UIToggle>();


        // 拼图个数 列表
        countList = transform.Find("Pop Up List -  Piece Count").GetComponent<UIPopupList>();


        // 拼图图像 列表
        imageList = transform.Find("Pop Up List - Piece Image").GetComponent<UIPopupList>();



        // 拼图形状 列表
        shapeList = transform.Find("Pop Up List - Piece Shape").GetComponent<UIPopupList>();



        // 拼图风格 列表
        styleList = transform.Find("Pop Up List - Piece Style").GetComponent<UIPopupList>();


        // 是否显示所有拼图 按钮
        showAllToggle = transform.Find("Toggle Button - Show All").GetComponent<UIToggle>();


        // 平铺拼图 按钮
        tileButton = transform.Find("Button - Tile Piece").GetComponent<UIButton>();


    }

    public void Init()
    {
        // 注册 Ui 事件
        UIEvent();

        // 注册游戏事件
        GameEvent();

        // 加载数据
        LoadData();

    }

    /// <summary>
    /// Ui 事件
    /// </summary>
    void UIEvent()
    {
        // 运行游戏事件
        startButton.onClick.Add(new EventDelegate(game.Run));

        // 重新开始游戏事件
        restartButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(game.ReStart));

        // 更新背景事件
        backgroundList.onChange.Add(new EventDelegate(() =>
        {
            // 获取背景名
            string name = UIPopupList.current.value;

            // 更新背景
            game.SetBackground(name);

            // 保存背景名
            data.background = name;
        }));

        // 切换原图显示事件
        imageToggle.onChange.Add(new EventDelegate(() =>
        {
            // 获取设置
            bool show = UIToggle.current.value;

            // 切换原图显示
            game.ToggleImage(show);

            // 保存设置
            data.showImage = show;
        }));

        // 切换旋转拼图事件
        rotateToggle.onChange.Add(new EventDelegate(() =>
        {
            // 获取设置
            bool rotate = UIToggle.current.value;

            // 转换拼图旋转
            game.ToggleRotate(rotate);

            // 保存设置
            data.rotatePuzzle = rotate;

            // 更新显示
            UpdateButton(game.canContinue);
        }));

        // 更改拼图个数事件
        countList.onChange.Add(new EventDelegate(() =>
        {
            // 获取个数
            string s = UIPopupList.current.value;
            int i = int.Parse(s);
            Count count = GetPieceCount(i);

            // 设置个数
            game.SetPieceCount(count);

            // 保存个数
            data.pieceCount = i;

            UpdateButton(game.canContinue);
        }));


        // 更改 拼图图像事件
        imageList.onChange.Add(new EventDelegate(() =>
        {
            // 获取名字
            string img = UIPopupList.current.value;

            // 设置图像
            game.SetPieceImage(img);

            // 保存名字
            data.pieceImage = img;
        }));

        // 更改 拼图形状事件
        shapeList.onChange.Add(new EventDelegate(() =>
        {
            // 获取名字
            string name = UIPopupList.current.value;

            // 设置形状
            game.SetPieceShape(name);

            // 保存名字
            data.pieceShape = name;
        }));

        // 更改 拼图风格 事件
        styleList.onChange.Add(new EventDelegate(() =>
        {
            // 获取 拼图风格 名字
            string name = UIPopupList.current.value;

            // 设置 拼图风格
            game.SetPieceStyle(name);

            // 保存 拼图风格
            data.pieceStyle = name;
        }));


        // 更改 显示所有拼图 事件
        showAllToggle.onChange.Add(new EventDelegate(() =>
        {
            game.ShowAllOrNot(UIToggle.current.value);
        }));

        // 平铺拼图 事件
        tileButton.onClick.Add(new EventDelegate(game.TilePiece));
    }

    /// <summary>
    /// 游戏事件
    /// </summary>
    void GameEvent()
    {
        // 关联进行游戏事件
        game.onGameRun.Add(new EventDelegate(() =>
        {

            // 隐藏菜单
            Hide();

            // 更新按钮显示
            UpdateButton(game.canContinue);
        }));

        // 关联暂停游戏事件
        game.onGamePause.Add(new EventDelegate(() =>
        {
            // 显示菜单
            Show();

            // 更新按钮显示
            UpdateButton(game.canContinue);
        }));

        // 游戏结束 时更新菜单控制
        game.onGameEnd.Add(new EventDelegate(() => UpdateButton(false)));
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    void LoadData()
    {

        countList.value = data.pieceCount.ToString();
        imageList.value = data.pieceImage;
        shapeList.value = data.pieceShape;
        styleList.value = data.pieceStyle;

        rotateToggle.value = data.rotatePuzzle;
        backgroundList.value = data.background;
        imageToggle.value = data.showImage;
    }

    #endregion


    /// <summary>
    /// 显示菜单
    /// </summary>
    public void Show()
    {
        // 如果正在显示，返回
        if (show) return;
        show = true;

        // 播放显示动画
        playAnim.Play(false);
    }

    /// <summary>
    /// 隐藏菜单
    /// </summary>
    public void Hide()
    {
        // 如果没有显示，返回
        if (!show) return;
        show = false;

        // 播放隐藏动画
        playAnim.Play(true);
    }

    /// <summary>
    /// 根据游戏状态，更新按钮文本
    /// </summary>
    void UpdateButton(bool canContinue)
    {
        // 是否可以继续游戏
        if (canContinue)
        {
            // 更新按钮 为 "继续"
            startLabel.key = "continue";

            // 刷新显示
            startLabel.enabled = false;
            startLabel.enabled = true;

            // 隐藏重新开始 按钮
            restartButton.SetActive(true);
        }
        else
        {
            // 更新按钮 为 "开始"
            startLabel.key = "start";

            // 刷新显示
            startLabel.enabled = false;
            startLabel.enabled = true;
            // 显示重新开始 按钮
            restartButton.SetActive(false);
        }
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
