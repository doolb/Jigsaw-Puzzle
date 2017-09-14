using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 控制脚本
/// </summary>
public class UIControl : MonoBehaviour
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
    /// 结束面板
    /// </summary>
    GameObject finish;

    /// <summary>
    /// 显示信息 对象
    /// </summary>
    UILabel infoLabel;

    /// <summary>
    /// 显示时间 对象
    /// </summary>
    UILabel time;

    /// <summary>
    /// 菜单按钮
    /// </summary>
    GameObject buttonMenu;


    /// <summary>
    /// 视口显示窗口
    /// </summary>
    GameObject viewWindow;

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {

        // 获取 结束面板
        finish = transform.Find("Finish").gameObject;
        finish.SetActive(false);



        // 获取文本对象
        infoLabel = finish.transform.Find("Label - Info").GetComponent<UILabel>();

        // 获取 显示时间 对象
        time = transform.Find("Label - Time").GetComponent<UILabel>();

        // 菜单 按钮
        buttonMenu = transform.Find("Button - Menu").gameObject;

        // 默认隐藏菜单按钮
        buttonMenu.SetActive(false);


        // 获取 视口显示窗口
        viewWindow = transform.Find("View Window").gameObject;



    }

    public void Init()
    {
        // 点击菜单事件
        buttonMenu.GetComponent<UIButton>().onClick.Add(new EventDelegate(game.Pause));


        // 添加 动画结束 事件
        finish.GetComponent<TweenAlpha>().onFinished.Add(new EventDelegate(() =>
        {
            // 结束界面 显示结束后，显示 纪录
            GameLoader.instance.recordControl.Show<Record>(game.record);

            // 关闭纪录界面后显示菜单
            GameLoader.instance.recordControl.onHide.Add(new EventDelegate(GameLoader.instance.menuControl.Show));
        }));



        // 关联进行游戏事件
        game.onGameRun.Add(new EventDelegate(Show));

        // 关联暂停游戏事件
        game.onGamePause.Add(new EventDelegate(Hide));

        // 注册 游戏结束 事件
        game.onGameEnd.Add(new EventDelegate(() =>
        {
            string msg = game.record.ToString();

            // 显示结束界面
            ShowFinish(msg);

            // 保存纪录
            data.records.Add(msg);
        }));

    }


    #endregion

    void FixedUpdate()
    {
        // 显示时间
        ShowTime(game.gameTime);
    }

    #region 公共函数
    /// <summary>
    /// 显示结束界面
    /// </summary>
    public void ShowFinish(string info)
    {
        // 隐藏菜单按钮
        buttonMenu.SetActive(false);

        // 激活界面
        finish.SetActive(true);

        // 播放动画
        finish.GetComponent<TweenAlpha>().ResetToBeginning();
        finish.GetComponent<TweenAlpha>().PlayForward();

        // 显示信息
        infoLabel.text = info;
    }



    /// <summary>
    /// 显示时间
    /// </summary>
    /// <param name="t">时间</param>
    public void ShowTime(float t = 0)
    {
        // 如果 时间 小于 0.1， 不显示
        if (t < 0.1)
            time.text = "";
        else
            // 显示时间
            time.text = t.ToString("F1");
    }

    // 显示或 隐藏  视口
    public void ToggleView(bool show)
    {
        // 显示或 隐藏  视口
        viewWindow.SetActive(show);
    }

    /// <summary>
    /// 显示菜单按钮
    /// </summary>
    public void Show()
    {
        // 激活菜单按钮
        buttonMenu.SetActive(true);
    }

    /// <summary>
    /// 隐藏菜单按钮
    /// </summary>
    public void Hide()
    {
        // 取消激活菜单按钮
        buttonMenu.SetActive(false);
    }
    #endregion

}
