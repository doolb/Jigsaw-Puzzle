using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody))]
/// <summary>
/// 这个用于拼图的每个块的具体逻辑
/// </summary>
public class Piece : MonoBehaviour
{

    #region 变量

    #region 静态变量


    /// <summary>
    /// 当前顺序的最大值
    /// </summary>
    public static int maxDepth = 0;

    /// <summary>
    /// 拼图列表的缓存
    /// </summary>
    public static List<GameObject> pieceCache = new List<GameObject>();

    /// <summary>
    /// 当前所有拼图的 最上层的对象
    /// </summary>
    static GameObject topGameObject = null;
    #endregion


    /// <summary>
    /// 碰撞体对象
    /// </summary>
    new BoxCollider collider;

    /// <summary>
    /// 图像渲染对象
    /// </summary>
    SpriteRenderer sprite;


    /// <summary>
    /// 拼图 ID 值
    /// </summary>
    public PieceID pid;

    /// <summary>
    /// 保存所有连接的拼图
    /// </summary>
    public List<GameObject> connectedPieces = new List<GameObject>();

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

            // 遍历所有的连接的拼图，设置顺序
            foreach (GameObject go in connectedPieces)
                go.GetComponent<SpriteRenderer>().sortingOrder = value;
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

        // 设置渲染顺序
        sprite.sortingOrder = pid == null ? 0 : pid.order;

        // 获取碰撞体对象
        collider = GetComponent<BoxCollider>();

    }


    #region 公共函数

    /// <summary>
    /// 当拼图被激活时调用
    /// </summary>
    public void OnActive()
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
        // 移动所有连接的拼图
        MoveConnectedPiece(delta);
    }

    /// <summary>
    /// 初始化拼图
    /// </summary>
    /// <param name="x">第 x 列</param>
    /// <param name="y">第 y 行</param>
    public void Init(int x, int y)
    {
        // 清空连接列表
        connectedPieces.Clear();

        // 初始化 id 值
        pid = new PieceID(x, y);

        // 设置 顺序
        order = pid.order;

        // 更新名字
        gameObject.name = ToString();

        // 设置 拼图 mark 材质
        sprite.material.SetTextureOffset("_MarkTex", pid.markOffset);

        // 更新大小
        ReSize();
    }

    /// <summary>
    /// 更新拼图的大小
    /// </summary>
    public void ReSize()
    {
        // 设置显示大小
        transform.localScale = new Vector3(200 / Puzzle.instance.pieceCount.x * Puzzle.instance.displayRatio.x,
                                           200 / Puzzle.instance.pieceCount.y * Puzzle.instance.displayRatio.y, 1);

        // 设置 collider 大小
        collider.size = new Vector3(Puzzle.instance.pieceImage.texture.width / 200.0f,
                                      Puzzle.instance.pieceImage.texture.height / 200.0f, 1);
    }

    /// <summary>
    /// 重载 ToString 函数
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "Piece " + pid.ToString();
    }

    /// <summary>
    /// 添加邻居
    /// </summary>
    /// <param name="other">要加入的拼图对象</param>
    /// <returns>是否添加成功</returns>
    public bool AddNeighbor(GameObject other, NeighborType type)
    {
        // 旋转角度 是否 都为 0 
        if (System.Math.Abs(transform.localEulerAngles.z) > 1 ||
            System.Math.Abs(other.transform.localEulerAngles.z) > 1)
            return false;
        

        // 计算要移动的偏移
        Vector3 offset = GetNeighborOffset(other, type);

        // 移动另个拼图对象
        other.transform.position += offset;

        // 移动所有相连的拼图
        other.GetComponent<Piece>().MoveConnectedPiece(offset);

        // 打印邻居信息
        print("Neighbor is at " + type + "\nmy :" + gameObject.GetComponent<Piece>() + " other :" + other.GetComponent<Piece>());

        // 设置 邻居 的顺序
        other.GetComponent<Piece>().order = order;

        // 把所有块连接起来
        ConnectPiece(other);

        // 添加成功
        return true;
    }

    #endregion


    #region 其它函数

    /// <summary>
    /// 获取当前邻居需要移动的偏移
    /// </summary>
    /// <param name="other">另一个邻居</param>
    /// <param name="type">邻居的类型</param>
    /// <returns>需要移动的偏移</returns>
    Vector3 GetNeighborOffset(GameObject other, NeighborType type)
    {
        // 邻居在 Y 轴上的偏移
        Vector3 offsetY = new Vector3(0, Puzzle.instance.displaySize.y, 0);

        // 邻居在 X 轴上的偏移
        Vector3 offsetX = new Vector3(Puzzle.instance.displaySize.x, 0, 0);

        // 邻居要移动到的位置
        Vector3 pos = Vector3.zero;

        // 判断邻居的类型，计算偏移
        switch (type)
        {
            // 邻居在上面
            case NeighborType.Top:
                // 在 Y 轴上增加偏移
                pos = gameObject.transform.position + offsetY;
                break;

            // 邻居在下面
            case NeighborType.Bottom:
                // 在 Y 轴上减少偏移
                pos = gameObject.transform.position - offsetY;
                break;

            // 邻居在左边
            case NeighborType.Left:
                // 在 X 轴上减少偏移
                pos = gameObject.transform.position - offsetX;
                break;

            // 邻居在右边
            case NeighborType.Right:
                // 在 X 轴上增加偏移
                pos = gameObject.transform.position + offsetX;
                break;

            // 不是邻居
            default:
                print("Neighbor type error: " + type);
                break;
        }

        // 返回要移动的偏移
        return pos - other.gameObject.transform.position;

    }

    

    /// <summary>
    /// 移动所有相邻的拼图
    /// </summary>
    /// <param name="offset">移动的偏移</param>
    void MoveConnectedPiece(Vector3 offset)
    {
        // 遍历所有的连接的拼图
        foreach (GameObject piece in gameObject.GetComponent<Piece>().connectedPieces)
        {
            // 移动拼图
            piece.transform.position += offset;
        }
    }


    /// <summary>
    /// 把所有可以连接的对象添加到缓存中
    /// </summary>
    /// <param name="other"></param>
    void GetAllConnected(GameObject other)
    {
        // 清空缓存列表
        pieceCache.Clear();

        // 添加当前拼图
        pieceCache.Add(gameObject);

        // 添加邻居
        pieceCache.Add(other);

        // 添加 所有和 自己相连的拼图
        foreach (GameObject piece in GetComponent<Piece>().connectedPieces)
            pieceCache.Add(piece);

        // 添加 所有和 邻居相连的拼图
        foreach (GameObject piece in other.GetComponent<Piece>().connectedPieces)
            pieceCache.Add(piece);

    }

    /// <summary>
    /// 把邻居加入以及和邻居的拼图加入到连接中
    /// </summary>
    /// <param name="other">要加入的邻居</param>
    void ConnectPiece(GameObject other)
    {

        // 把所有需要连接的拼图 保存到缓存中
        GetAllConnected(other);

        // 嵌套遍历 缓存，一 一 相连
        foreach (GameObject a in pieceCache)
        {
            foreach (GameObject b in pieceCache)
            {
                // 是否是同一个
                if (a == b) continue;

                // 是否已经加入
                Piece p = a.GetComponent<Piece>();
                if (a.GetComponent<Piece>().connectedPieces.Contains(b)) continue;

                // 连接拼图
                p.connectedPieces.Add(b);

            }

        }


    }
    #endregion


}

/// <summary>
/// 邻居的类型
/// </summary>
public enum NeighborType
{
    /// <summary>
    /// 不是邻居
    /// </summary>
    None,

    /// <summary>
    /// 上面
    /// </summary>
    Top,

    /// <summary>
    /// 下面
    /// </summary>
    Bottom,

    /// <summary>
    /// 左边
    /// </summary>
    Left,

    /// <summary>
    /// 右边
    /// </summary>
    Right,

    /// <summary>
    /// 枚举最大值
    /// </summary>
    Max
};


/// <summary>
/// 拼图的 ID ，表示拼图的位置
/// </summary>
public class PieceID
{
    /// <summary>
    /// 第 x 列，从 0 开始计数
    /// </summary>
    public int x;

    /// <summary>
    /// 第 y 行，从 0 开始计数
    /// </summary>
    public int y;

    /// <summary>
    /// 是否在边界上
    /// </summary>
    public bool isAtEdge;

    /// <summary>
    /// 拼图形状 贴图 的 uv 值
    /// </summary>
    public Vector2 markOffset;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="x">第 x 列，从 0 开始计数</param>
    /// <param name="y">第 y 行，从 0 开始计数</param>
    public PieceID(int x, int y)
    {
        // 第 x 列
        this.x = x;

        // 第 y 行
        this.y = y;

        // 贴图 的 uv 值
        CalcMarkOffset();
    }

    /// <summary>
    /// 初始的顺序 值
    /// </summary>
    public int order
    {
        // 返回 当前拼图的 顺序
        get { return x * (int)Puzzle.instance.pieceCount.y + y; }
    }

    /// <summary>
    /// 重载 ToString 函数
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        // 返回 当前的 行和列
        return x + ":" + y;
    }

    /// <summary>
    /// 判断 两个拼图是否是邻居
    /// </summary>
    /// <param name="other">另一个拼图</param>
    /// <returns>邻居的类型</returns>
    public NeighborType IsNeighbor(PieceID other)
    {
        NeighborType type = NeighborType.None;

        // 如果另一拼图对象为 null， 直接返回
        if (other == null) return type;

        // 同一列
        if (x == other.x)
        {
            // 上面
            if (y + 1 == other.y) type = NeighborType.Top;

            // 下面
            if (y - 1 == other.y) type = NeighborType.Bottom;

        }
        // 同一行
        else if (y == other.y)
        {
            // 右边
            if (x + 1 == other.x) type = NeighborType.Right;

            // 左边
            if (x - 1 == other.x) type = NeighborType.Left;
        }

        // 返回邻居的类型
        return type;
    }

    /// <summary>
    /// 计算拼图形状 贴图的 uv 偏移
    /// </summary>
    void CalcMarkOffset()
    {
        // 默认在边界上
        isAtEdge = true;

        // 记录当前拼图的块数
        int countX = (int)Puzzle.instance.pieceCount.x;
        int countY = (int)Puzzle.instance.pieceCount.y;

        // 当前的偏移值
        float offsetX = 0, offsetY = 0;

        // 左边界
        if (x == 0)
        {
            // 左下角, uv = (0,0)
            if (y == 0) goto _end_;

            // 左上角, uv = (0,0.25)
            if (y == countY - 1) { offsetY = 0.25f; goto _end_; };

            // 在贴图的第 2 列的位置
            offsetX = 0.25f;

            // 交替设置 y 的偏移
            offsetY = y % 2 == 1 ? 0.25f : 0.0f;
        }
        // 右边界
        else if (x == countX - 1)
        {

            // 右下角, uv = (0,0.5)
            if (y == 0) { offsetY = 0.5f; goto _end_; }

            // 右上角, uv = (0,0.75)
            if (y == countY - 1) { offsetY = 0.75f; goto _end_; };

            // 在贴图的第 2 列的位置
            offsetX = 0.25f;

            // 交替设置 y 的偏移
            offsetY = y % 2 == 1 ? 0.75f : 0.5f;
        }


        // 不用判断角落了
        // 下边界
        else if (y == 0)
        {
            // 在贴图的第 3 列的位置
            offsetX = 0.5f;

            // 交替设置 y 的偏移
            offsetY = x % 2 == 1 ? 0.25f : 0f;
        }
        // 上边界
        else if (y == countY - 1)
        {
            // 在贴图的第 3 列的位置
            offsetX = 0.5f;

            // 交替设置 y 的偏移
            offsetY = x % 2 == 1 ? 0.75f : 0.5f;
        }
        // 其它地方
        else
        {
            // 在贴图的第 4 列的位置
            offsetX = 0.75f;

            // 交替设置 y 的偏移
            offsetY = x % 2 == 1 ? 0.25f : 0f;
            offsetY += y % 2 == 1 ? 0f : 0.5f;

            // 当前不在边界上
            isAtEdge = false;
        }



    _end_:

        // 保存 uv 偏移
        markOffset = new Vector2(offsetX, offsetY);
    }
}