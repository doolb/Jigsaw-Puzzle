using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 控制脚本
/// </summary>
public class UIControl : MonoBehaviour
{
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
    /// 初始化
    /// </summary>
    void Awake()
    {

        // 获取结束面板
        finish = transform.Find("Finish").gameObject;
        finish.SetActive(false);

        // 添加 动画结束 事件
        finish.GetComponent<TweenAlpha>().onFinished.Add(new EventDelegate(GameLoader.menuControl.Show));

        // 获取文本对象
        infoLabel = finish.transform.Find("Label - Info").GetComponent<UILabel>();

        // 获取 显示时间 对象
        time = transform.Find("Label - Time").GetComponent<UILabel>();

        // 注册 菜单控制 事件
        buttonMenu = transform.Find("Button - Menu").gameObject;
        buttonMenu.GetComponent<UIButton>().onClick.Add(new EventDelegate(GameLoader.menuControl.Show));

        // 注册 游戏结束 事件
        GameLoader.puzzleGame.onGameEnd.Add(new EventDelegate(ShowFinish));
    }

    /// <summary>
    /// 固定时间执行
    /// </summary>
    void FixedUpdate()
    {
        // 显示时间
        ShowTime(GameLoader.puzzleGame.gameTime);
    }

    /// <summary>
    /// 显示结束界面
    /// </summary>
    public void ShowFinish()
    {
        // 激活界面
        finish.SetActive(true);

        // 播放动画
        finish.GetComponent<TweenAlpha>().ResetToBeginning();
        finish.GetComponent<TweenAlpha>().PlayForward();

        // 显示信息
        infoLabel.text = GameLoader.puzzleGame.record.ToString();
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
}
