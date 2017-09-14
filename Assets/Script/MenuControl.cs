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
    public bool show = true;

    /// <summary>
    /// 开始按钮 文本 对象
    /// </summary>
    public UILabel startLabel;

    /// <summary>
    /// 开始按钮 对象
    /// </summary>
    public UIButton startButton;

    /// <summary>
    /// 重新开始游戏的按钮
    /// </summary>
    public GameObject restartButton;

    /// <summary>
    /// 背景列表
    /// </summary>
    public UIPopupList backgroundList;

    /// <summary>
    /// 个数列表
    /// </summary>
    public UIPopupList countList;

    /// <summary>
    /// 图像列表
    /// </summary>
    public UIPopupList imageList;

    /// <summary>
    /// 形状列表
    /// </summary>
    public UIPopupList shapeList;

    /// <summary>
    /// 风格列表
    /// </summary>
    public UIPopupList styleList;

    /// <summary>
    /// 是否显示原图
    /// </summary>
    public UIToggle imageToggle;


    /// <summary>
    /// 拼图旋转转换
    /// </summary>
    public UIToggle rotateToggle;

    /// <summary>
    /// 是否显示所有
    /// </summary>
    public UIToggle showAllToggle;

    /// <summary>
    /// 平铺拼图按钮
    /// </summary>
    public UIButton tileButton;

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
        startLabel = start.Find("Label").GetComponent<UILabel>();


        // 获取 重新开始按钮
        restartButton = transform.Find("Button - ReStart").gameObject;


        // 获取 背景 列表
        backgroundList = transform.Find("Pop Up List - Background").GetComponent<UIPopupList>();

        // 寻找 是否 显示原始图像
        imageToggle = transform.Find("Check Box - Show Original Image").GetComponent<UIToggle>();

        // 寻找 是否 旋转拼图  单选框
        rotateToggle = transform.Find("Check Box - Rotate Piece").GetComponent<UIToggle>();

        // 寻找 拼图个数 列表
        countList = transform.Find("Pop Up List -  Piece Count").GetComponent<UIPopupList>();

        // 寻找 拼图图像 列表
        imageList = transform.Find("Pop Up List - Piece Image").GetComponent<UIPopupList>();

        // 寻找 拼图形状 列表, 并注册 事件
        shapeList = transform.Find("Pop Up List - Piece Shape").GetComponent<UIPopupList>();

        // 寻找 拼图风格 列表, 并注册 事件
        styleList = transform.Find("Pop Up List - Piece Style").GetComponent<UIPopupList>();

        // 寻找 是否显示所有拼图 按钮, 并注册 事件
        showAllToggle = transform.Find("Toggle Button - Show All").GetComponent<UIToggle>();

        // 寻找 平铺拼图 按钮, 并注册 事件
        tileButton = transform.Find("Button - Tile Piece").GetComponent<UIButton>();


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

        // 暂停时间更新
        Time.timeScale = 0;

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

        // 恢复时间更新
        Time.timeScale = 1;
    }


    /// <summary>
    /// 根据游戏状态，更新按钮文本
    /// </summary>
    public void UpdateButton(bool canContinue)
    {
        // 是否可以继续游戏
        if (canContinue)
        {
            // 更新按钮 为 "继续"
            startLabel.text = "继续";

            // 隐藏重新开始 按钮
            restartButton.SetActive(true);
        }
        else
        {
            // 更新按钮 为 "开始"
            startLabel.text = "开始";

            // 显示重新开始 按钮
            restartButton.SetActive(false);
        }
    }

}
