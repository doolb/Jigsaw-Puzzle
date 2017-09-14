using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// 这个用于拼图的每个块的显示
/// </summary>
public class Piece : MonoBehaviour
{

    #region 变量

    /// <summary>
    /// 拼图逻辑管理对象
    /// </summary>
    public Puzzle puzzle;

    /// <summary>
    /// 当前拼图 ID 值
    /// </summary>
    public Count id;

    /// <summary>
    /// 碰撞体对象
    /// </summary>
    new BoxCollider collider;

    /// <summary>
    /// 图像渲染对象
    /// </summary>
    SpriteRenderer sprite;


    /// <summary>
    /// 是否在边界上
    /// </summary>
    public bool isAtEdge;


    /// <summary>
    /// 当前拼图块在 连接列表的 索引
    /// </summary>
    public int connectedListID;

    public int connectedCount
    {
        get { return puzzle.connectedPieces[connectedListID].Count; }
    }

    /// <summary>
    /// 拼图对象的顺序
    /// </summary>
    public int order
    {
        get
        {
            // 返回图像渲染的 排序值
            if (sprite != null) return sprite.sortingOrder;
            else return -1;
        }

        set
        {
            // 设置图像渲染的 排序值
            if (sprite != null) sprite.sortingOrder = value;
        }
    }

    #endregion

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        // 获取图像渲染对象
        sprite = GetComponent<SpriteRenderer>();

        // 获取碰撞体对象
        collider = GetComponent<BoxCollider>();
    }


    /// <summary>
    /// 重载 ToString 函数
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "Piece " + id.ToString();
    }



    #region 公共函数
    /// <summary>
    /// 初始化拼图
    /// </summary>
    /// <param name="x">第 x 列</param>
    /// <param name="y">第 y 行</param>
    public void Init(int x, int y)
    {
        // 重置 id
        id.x = x;
        id.y = y;

        // 设置 顺序
        order = id.x * puzzle.count.y + id.y;

        // 重置 在 连接列表的 id
        connectedListID = order;

        // 更新名字
        gameObject.name = ToString();

        // 更新大小
        ReSize();
    }

    /// <summary>
    /// 更新拼图的大小
    /// </summary>
    public void ReSize()
    {
        // 设置显示大小
        transform.localScale = new Vector3(200 / puzzle.count.x * puzzle.display.x,
                                           200 / puzzle.count.y * puzzle.display.y, 1);

        // 设置 collider 大小
        collider.size = new Vector3(puzzle.imageX / 200.0f, puzzle.imageY / 200.0f, 1);
    }
    #endregion


}
