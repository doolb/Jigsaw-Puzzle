using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 记录界面控制脚本
/// </summary>
public class RecordControl : MonoBehaviour
{
    /// <summary>
    /// 游戏数据
    /// </summary>
    GameData data
    {
        get { return GameLoader.instance.dataManager.gameData; }
    }

    /// <summary>
    /// 界面隐藏 事件
    /// </summary>
    public List<EventDelegate> onHide = new List<EventDelegate>();

    /// <summary>
    /// 纪录对象预制体
    /// </summary>
    public GameObject recordPrefab;

    /// <summary>
    /// 保存纪录的对象
    /// </summary>
    GameObject recordsView;

    /// <summary>
    /// 当前纪录的个数
    /// </summary>
    int recordCount = 0;


    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {

        // 隐藏界面
        gameObject.SetActive(false);

        // 寻找关闭按钮，注册事件
        transform.Find("Button - Close").GetComponent<UIButton>().onClick.Add(new EventDelegate(Hide));

        // 加载纪录对象预制体
        recordPrefab = Resources.Load<GameObject>("Record");

        // 获取保存纪录的对象
        recordsView = transform.Find("Scroll View - Scores").gameObject;

        // 加载纪录
        Add<string>(data.records);
    }


    /// <summary>
    /// 显示界面
    /// </summary>
    public void Show()
    {
        // 显示界面
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 添加纪录并且显示界面
    /// </summary>
    /// <typeparam name="T">要添加的对象的类型</typeparam>
    /// <param name="items">要添加的对象的列表</param>
    public void Show<T>(List<T> items)
    {

        // 显示界面
        Show();

        // 添加新的纪录
        Add<T>(items);

    }


    /// <summary>
    /// 添加一个纪录并且显示界面
    /// </summary>
    /// <typeparam name="T">要添加的对象的类型</typeparam>
    /// <param name="item">要添加的对象的</param>
    public void Show<T>(T item)
    {

        // 显示界面
        Show();

        // 添加新的纪录
        Add<T>(item);

    }

    /// <summary>
    /// 隐藏界面
    /// </summary>
    public void Hide()
    {
        // 隐藏界面
        gameObject.SetActive(false);

        // 发送 隐藏 事件
        for (int i = 0; i < onHide.Count; i++)
            onHide[i].Execute();
    }

    /// <summary>
    /// 添加新的纪录
    /// </summary>
    /// <typeparam name="T">要添加的对象的类型</typeparam>
    /// <param name="item">要添加的对象</param>
    public void Add<T>(T item)
    {
        // 获取一个纪录对象
        GameObject child = null;

        // 如果纪录数大于 己有的个对象数
        if (++recordCount > recordsView.transform.childCount)
            // 实例化一个新的对象
            child = NGUITools.AddChild(recordsView, recordPrefab);

        // 否者直接使用现有的对象
        else child = recordsView.transform.GetChild(recordCount - 1).gameObject;

        // 激活对象
        child.SetActive(true);

        // 更新 Num 文本
        child.transform.Find("Label - Num").GetComponent<UILabel>().text = recordCount.ToString();

        // 更新 纪录 文本
        child.transform.Find("Label - Info").GetComponent<UILabel>().text = item.ToString();

        // 更新 所有纪录的显示
        recordsView.GetComponent<UIGrid>().Reposition();

    }

    /// <summary>
    /// 添加新的纪录
    /// </summary>
    /// <typeparam name="T">要添加的对象的类型</typeparam>
    /// <param name="items">要添加的对象的列表</param>
    public void Add<T>(List<T> items)
    {
        // 遍历列表
        foreach (T item in items)
        {
            Add<T>(item);
        }
    }

    /// <summary>
    /// 清空纪录
    /// </summary>
    public void Clear()
    {
        // 遍历所有纪录
        for (int i = 0; i < recordCount; i++)
        {
            // 隐藏对象
            recordsView.transform.GetChild(i).gameObject.SetActive(false);
        }

        // 重置纪录数
        recordCount = 0;
    }
}
