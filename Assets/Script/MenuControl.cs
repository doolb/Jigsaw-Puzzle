using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏菜单控制脚本
/// </summary>
public class MenuControl : MonoBehaviour
{
    /// <summary>
    /// 动画播放对象
    /// </summary>
    UIPlayAnimation playAnim;

    /// <summary>
    /// 记录菜单是否显示
    /// </summary>
    bool show = true;

    /// <summary>
    /// 开始按钮 文本 对象
    /// </summary>
    UILabel startLabel;

    /// <summary>
    /// 重新开始游戏的按钮
    /// </summary>
    GameObject restartButton;

    /// <summary>
    /// 开始游戏事件
    /// </summary>
    public List<EventDelegate> onStartGame = new List<EventDelegate>();

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        // 获取动画播放对象
        playAnim = gameObject.AddComponent<UIPlayAnimation>();

        // 设置动画
        playAnim.target = GetComponent<Animation>();

        // 注册 ngui 事件
        RegistryCallback();

        // 注册 游戏结束 事件
        GameLoader.puzzleGame.onGameEnd.Add(new EventDelegate(GameEnd));
    }

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

        // 暂停游戏
        GameLoader.puzzleGame.Pause();

        // 隐藏 UI
        GameLoader.uiControl.Hide();
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

        // 继续游戏
        GameLoader.puzzleGame.Continue();

        // 显示 UI
        GameLoader.uiControl.Show();
    }

    /// <summary>
    /// 游戏结束 时 回调
    /// </summary>
    void GameEnd()
    {
        // 更新按钮显示
        UpdateButton();
    }

    #region ngui callback

    /// <summary>
    /// 注册 ngui 事件
    /// </summary>
    void RegistryCallback()
    {
        // 寻找开始按钮
        Transform startButton = transform.Find("Button - Start");

        // 注册 开始游戏 事件
        startButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(StartGame));

        // 寻找开始按钮 文本对象
        startLabel = startButton.Find("Label").GetComponent<UILabel>();


        // 寻找 背景 列表, 并注册 事件
        transform.Find("Pop Up List - Background").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeBackground));

        // 寻找 是否 显示原始图像 , 并注册 事件
        transform.Find("Check Box - Show Original Image").GetComponent<UIToggle>().onChange.Add(new EventDelegate(ToggleImage));

        // 寻找 是否 旋转拼图  单选框, 并注册 事件
        transform.Find("Check Box - Rotate Piece").GetComponent<UIToggle>().onChange.Add(new EventDelegate(ToggleRotate));

        // 寻找 拼图个数 列表, 并注册 事件
        transform.Find("Pop Up List -  Piece Count").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeCount));

        // 寻找 拼图图像 列表, 并注册 事件
        transform.Find("Pop Up List - Piece Image").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeImage));

        // 寻找 拼图形状 列表, 并注册 事件
        transform.Find("Pop Up List - Piece Shape").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeShape));

        // 寻找 拼图风格 列表, 并注册 事件
        transform.Find("Pop Up List - Piece Style").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeStyle));

        // 寻找 是否显示所有拼图 按钮, 并注册 事件
        transform.Find("Toggle Button - Show All").GetComponent<UIToggle>().onChange.Add(new EventDelegate(ToggleShow));

        // 寻找 平铺拼图 按钮, 并注册 事件
        transform.Find("Button - Tile Piece").GetComponent<UIButton>().onClick.Add(new EventDelegate(TilePiece));


        // 寻找开始按钮，并注册事件
        restartButton = transform.Find("Button - ReStart").gameObject;
        restartButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(ReStartGame));
    }

    /// <summary>
    /// 根据游戏状态，更新按钮文本
    /// </summary>
    void UpdateButton()
    {
        // 如果没有创建拼图，或者需要重新创建
        if (GameLoader.puzzleGame.needRestart ||
            !GameLoader.puzzleGame.pieceCreated)
        {
            // 更新按钮 为 "开始"
            startLabel.text = "开始";

            // 显示重新开始 按钮
            restartButton.SetActive(false);
        }
        else
        {
            // 更新按钮 为 "继续"
            startLabel.text = "继续";

            // 隐藏重新开始 按钮
            restartButton.SetActive(true);
        }
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    void StartGame()
    {
        // 隐藏菜单
        Hide();

        // 开始游戏
        GameLoader.puzzleGame.StartGame();

        // 更新按钮文本
        UpdateButton();

        // 通知开始游戏事件
        for (int i = 0; i < onStartGame.Count; i++)
            onStartGame[i].Execute();
    }

    /// <summary>
    /// 更改背景
    /// </summary>
    void ChangeBackground()
    {
        // 更改背景
        GameLoader.background.ChangeBackground(UIPopupList.current.value);
    }

    /// <summary>
    /// 更改拼图个数
    /// </summary>
    void ChangeCount()
    {
        // 更改拼图个数
        GameLoader.puzzleGame.SetPieceCount(GetPieceCount(int.Parse(UIPopupList.current.value.Trim())));

        // 更新按钮文本
        UpdateButton();
    }

    /// <summary>
    /// 把拼图总个数，转换为 两个数的乘积
    /// </summary>
    /// <param name="count">要转换个数</param>
    /// <returns></returns>
    Vector2 GetPieceCount(int count)
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
        return new Vector2(x, y);
    }

    /// <summary>
    /// 更改拼图图像
    /// </summary>
    void ChangeImage()
    {
        // 更改拼图图像
        GameLoader.puzzleGame.SetPieceImage(UIPopupList.current.value);
    }

    /// <summary>
    /// 更改拼图形状
    /// </summary>
    void ChangeShape()
    {
        // 更改拼图形状
        GameLoader.puzzleGame.SetPieceShape(UIPopupList.current.value);
    }


    /// <summary>
    /// 更改拼图风格
    /// </summary>
    void ChangeStyle()
    {
        // 更改拼图风格
        GameLoader.puzzleGame.SetPieceStyle(UIPopupList.current.value);
    }

    /// <summary>
    /// 切换是否显示全就拼图
    /// </summary>
    void ToggleShow()
    {
        // 切换是否显示全就拼图
        GameLoader.puzzleGame.ShowAllOrNot(UIToggle.current.value);
    }

    /// <summary>
    /// 平铺拼图
    /// </summary>
    void TilePiece()
    {
        // 平铺拼图
        GameLoader.puzzleGame.TilePiece();
    }

    /// <summary>
    /// 切换显示原图
    /// </summary>
    void ToggleImage()
    {
        // 切换显示原图
        GameLoader.puzzleGame.ToggleImage(UIToggle.current.value);
    }

    /// <summary>
    /// 切换旋转
    /// </summary>
    void ToggleRotate()
    {
        // 切换旋转
        GameLoader.puzzleGame.ToggleRotate(UIToggle.current.value);

        // 更新按钮
        UpdateButton();
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    void ReStartGame()
    {
        GameLoader.puzzleGame.ReStart();
    }

    #endregion
}
