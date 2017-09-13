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

    public Puzzle puzzle;


    #region 静态变量


    /// <summary>
    /// 当前顺序的最大值
    /// </summary>
    public static int maxDepth = 0;
    
    /// <summary>
    /// 当前所有拼图的 最上层的对象
    /// </summary>
    static GameObject topGameObject = null;
    #endregion

    
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
    /// 拼图形状 贴图 的 uv 值
    /// </summary>
    public Vector2 markOffset;

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


    /// <summary>
    /// 计算拼图形状 贴图的 uv 偏移
    /// </summary>
    void CalcMarkOffset()
    {
        // 默认在边界上
        isAtEdge = true;

        // 当前的偏移值
        float offsetX = 0, offsetY = 0;

        // 左边界
        if (id.x == 0)
        {
            // 左下角, uv = (0,0)
            if (id.y == 0) goto _end_;

            // 左上角, uv = (0,0.25)
            if (id.y == puzzle.count.y - 1) { offsetY = 0.25f; goto _end_; };

            // 在贴图的第 2 列的位置
            offsetX = 0.25f;

            // 交替设置 y 的偏移
            offsetY = id.y % 2 == 1 ? 0.25f : 0.0f;
        }
        // 右边界
        else if (id.x == puzzle.count.x - 1)
        {

            // 右下角, uv = (0,0.5)
            if (id.y == 0) { offsetY = 0.5f; goto _end_; }

            // 右上角, uv = (0,0.75)
            if (id.y == puzzle.count.y - 1) { offsetY = 0.75f; goto _end_; };

            // 在贴图的第 2 列的位置
            offsetX = 0.25f;

            // 交替设置 y 的偏移
            offsetY = id.y % 2 == 1 ? 0.75f : 0.5f;
        }


        // 不用判断角落了
        // 下边界
        else if (id.y == 0)
        {
            // 在贴图的第 3 列的位置
            offsetX = 0.5f;

            // 交替设置 y 的偏移
            offsetY = id.x % 2 == 1 ? 0.25f : 0f;
        }
        // 上边界
        else if (id.y == puzzle.count.y - 1)
        {
            // 在贴图的第 3 列的位置
            offsetX = 0.5f;

            // 交替设置 y 的偏移
            offsetY = id.x % 2 == 1 ? 0.75f : 0.5f;
        }
        // 其它地方
        else
        {
            // 在贴图的第 4 列的位置
            offsetX = 0.75f;

            // 交替设置 y 的偏移
            offsetY = id.x % 2 == 1 ? 0.25f : 0f;
            offsetY += id.y % 2 == 1 ? 0f : 0.5f;

            // 当前不在边界上
            isAtEdge = false;
        }



    _end_:

        // 保存 uv 偏移
        markOffset = new Vector2(offsetX, offsetY);
    }


    #region 公共函数

    /// <summary>
    /// 当拼图被选中时调用
    /// </summary>
    public void OnSelect()
    {
        // 是否是最顶的对象
        if (topGameObject != gameObject)
        {
            // 设置当前对象为顶层对象
            topGameObject = gameObject;

            // 设置顺序
            order = ++maxDepth;
        }

        // 是否旋转，并是否按下 "Fire2" 按钮
        if (PuzzleGame.isRotate && Input.GetButton("Fire2"))
        {
            // 顺时针旋转 90 度
            transform.localEulerAngles -= new Vector3(0, 0, 90);
        }
    }

    /// <summary>
    /// 当拼图被移动时调用
    /// </summary>
    /// <param name="delta">移动的偏移</param>
    public void OnMove(Vector3 delta)
    {
        // 移动当前拼图块
        transform.position += delta;
    }

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


        // 贴图 的 uv 值
        CalcMarkOffset();

        // 设置 拼图 mark 材质
        sprite.material.SetTextureOffset("_MarkTex", markOffset);

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
        collider.size = new Vector3(puzzle.imageX / 200.0f,puzzle.imageY / 200.0f, 1);
    }
    #endregion


}
