using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 保存 整形 二维大小的结构体
/// </summary>
public struct Count
{
    /// <summary>
    /// x 
    /// </summary>
    public int x;

    /// <summary>
    /// y
    /// </summary>
    public int y;


    /// <summary>
    /// 重载 ToString 函数
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return x + ":" + y;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="_x">x</param>
    /// <param name="_y">y</param>
    public Count(int _x, int _y)
    {
        x = _x;
        y = _y;
    }


    public static bool operator !=(Count lhs, Count rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator ==(Count lhs, Count rhs)
    {
        return (lhs.x == rhs.x) && (lhs.y == rhs.y);
    }
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
/// 这个类控制拼图的逻辑
/// </summary>
public class Puzzle
{
    /// <summary>
    /// 拼图显示管理对象
    /// </summary>
    PuzzleManager manager;


    /// <summary>
    /// 拼图的个数
    /// </summary>
    public Count count = new Count(6, 4);

    /// <summary>
    /// 拼图总个数
    /// </summary>
    public int totalCount
    {
        get { return count.x * count.y; }
    }

    /// <summary>
    /// 拼图的大小
    /// </summary>
    public Vector2 size
    {
        get { return manager.pieceSize; }
    }


    /// <summary>
    /// 拼图图像 宽度
    /// </summary>
    public float imageX { get { return manager.pieceImage.width; } }

    /// <summary>
    /// 拼图图像 高度
    /// </summary>
    public float imageY { get { return manager.pieceImage.height; } }

    /// <summary>
    /// 是否完成拼图
    /// </summary>
    public bool finish;

    /// <summary>
    /// 缓存邻居拼图
    /// </summary>
    List<GameObject> neighborCache = new List<GameObject>();

    /// <summary>
    /// 缓存邻居拼图类型
    /// </summary>
    List<NeighborType> neighborTypes = new List<NeighborType>();

    /// <summary>
    /// 保存所有拼图的连接表的列表
    /// </summary>
    public List<List<GameObject>> connectedPieces = new List<List<GameObject>>();


    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="m">管理拼图显示的对象</param>
    public Puzzle(PuzzleManager m)
    {
        manager = m;
    }

    #region 公共函数

    /// <summary>
    /// 重置拼图
    /// </summary>
    public void Reset()
    {
        // 设置拼图为未完成
        finish = false;

        // 遍历所有拼图
        for (int i = 0; i < count.x; i++)
        {
            for (int j = 0; j < count.y; j++)
            {
                // 获取 第 (x,y)个拼图
                GameObject child = GetPiece(i, j);

                int id = i * count.y + j;

                // 判断列表个数是否足够
                List<GameObject> list = null;
                if (id >= connectedPieces.Count)
                {
                    // 新建一个列表
                    list = new List<GameObject>();

                    // 加入列表中
                    connectedPieces.Add(list);
                }
                // 直接获取列表
                else
                    list = connectedPieces[id];

                // 清空列表
                list.Clear();

                // 添加当前拼图
                list.Add(child);
            }
        }
    }


    /// <summary>
    /// 移动拼图，包括相连的其它拼图,使用的是世界坐标
    /// </summary>
    /// <param name="piece">要移动的拼图</param>
    /// <param name="delta">要移动的距离，世界坐标</param>
    public void Move(GameObject piece, Vector3 delta)
    {
        // 获取当前拼图 在 连接表 中索引
        int index = piece.GetComponent<Piece>().connectedListID;

        // 移动所有连接的拼图
        foreach (GameObject go in connectedPieces[index])
            go.transform.position += delta;
    }

    /// <summary>
    /// 移动拼图，包括相连的其它拼图,使用的是局部坐标
    /// </summary>
    /// <param name="piece">要移动的拼图</param>
    /// <param name="delta">要移动的距离，局部坐标</param>
    public void LocalMove(GameObject piece, Vector3 delta)
    {
        // 获取当前拼图 在 连接表 中索引
        int index = piece.GetComponent<Piece>().connectedListID;

        // 移动所有连接的拼图
        foreach (GameObject go in connectedPieces[index])
            go.transform.localPosition += delta;

        Debug.Log("move " + delta);
    }

    /// <summary>
    /// 移动一个拼图结束
    /// </summary>
    /// <param name="piece">移动的拼图</param>
    public void MoveEnd(GameObject piece)
    {
        // 获取当前拼图 在 连接表 中索引
        int index = piece.GetComponent<Piece>().connectedListID;

        // 拼图是否已经完成
        if (connectedPieces[index].Count == totalCount)
            return;

        // 遍历所有连接的拼图
        foreach (GameObject go in connectedPieces[index])
        {
            // 如果添加邻居成功，返回
            if (AddNeighbor(go))
                break;
        }

        // 拼图完成
        if (connectedPieces[piece.GetComponent<Piece>().connectedListID].Count == totalCount)
        {
            finish = true;
        }
    }
    #endregion


    #region 拼图函数


    /// <summary>
    /// 获取第( x,y ) 块拼图
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    GameObject GetPiece(int x, int y)
    {
        return manager.GetPiece(x, y);
    }



    /// <summary>
    /// 获取最近的邻居
    /// </summary>
    /// <param name="go">要获取邻居的对象</param>
    /// <param name="type">邻居的类型</param>
    /// <returns>找到的邻居</returns>
    GameObject GetCloestNeighbor(GameObject go, out NeighborType type)
    {
        type = NeighborType.None;
        // 获取所有邻居 和邻居的类型
        CheckNeighbros(go);

        for (int i = 0; i < neighborCache.Count; i++)
        {
            type = neighborTypes[i];
            // 如果可以连接，返回
            if (IsClosedToConnect(go.transform.localPosition, neighborCache[i].transform.localPosition, type))
            {
                return neighborCache[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 检查四个邻居
    /// </summary>
    /// <param name="go">要检查的对象</param>
    void CheckNeighbros(GameObject go)
    {
        // 清空缓存列表
        neighborCache.Clear();
        neighborTypes.Clear();

        // 获取拼图块控制脚本
        Piece piece = go.GetComponent<Piece>();

        // 获取邻居表
        List<GameObject> neighbors = connectedPieces[piece.connectedListID];

        // 左，跳过最左边的一列
        if (piece.id.x > 0)
        {
            // 获取 左边的拼图
            GameObject nb = GetPiece(piece.id.x - 1, piece.id.y);

            // 检查是否已经连接，并且没有隐藏
            if (!neighbors.Contains(nb) && nb.activeSelf)
            {
                neighborCache.Add(nb);
                neighborTypes.Add(NeighborType.Left);
            }
        }

        // 右，跳过最右边的一列
        if (piece.id.x < count.x - 1)
        {
            // 获取 右边的拼图
            GameObject nb = GetPiece(piece.id.x + 1, piece.id.y);

            // 检查是否已经连接，并且没有隐藏
            if (!neighbors.Contains(nb) && nb.activeSelf)
            {
                neighborCache.Add(nb);
                neighborTypes.Add(NeighborType.Right);
            }
        }

        // 下，跳过最下的一行
        if (piece.id.y > 0)
        {
            // 获取 下边的拼图
            GameObject nb = GetPiece(piece.id.x, piece.id.y - 1);

            // 检查是否已经连接，并且没有隐藏
            if (!neighbors.Contains(nb) && nb.activeSelf)
            {
                neighborCache.Add(nb);
                neighborTypes.Add(NeighborType.Bottom);
            }
        }

        // 上，跳过最上的一行
        if (piece.id.y < count.y - 1)
        {
            // 获取 上边的拼图
            GameObject nb = GetPiece(piece.id.x, piece.id.y + 1);

            // 检查是否已经连接，并且没有隐藏
            if (!neighbors.Contains(nb) && nb.activeSelf)
            {
                neighborCache.Add(nb);
                neighborTypes.Add(NeighborType.Top);
            }
        }
    }



    /// <summary>
    /// 添加邻居
    /// </summary>
    /// <param name="piece">当前选中的拼图</param>
    /// <param name="type">邻居的类型</param>
    /// <returns>是否添加成功</returns>
    public bool AddNeighbor(GameObject piece)
    {
        // 寻找最近的邻居
        NeighborType type;
        GameObject neighbor = GetCloestNeighbor(piece, out type);

        // 如果没有找到，返回
        if (neighbor == null) return false;

        // 旋转角度 是否 都为 0 
        if (System.Math.Abs(piece.transform.localEulerAngles.z) > 1 ||
            System.Math.Abs(neighbor.transform.localEulerAngles.z) > 1)
            return false;


        // 计算要移动的偏移
        Vector3 offset = GetNeighborOffset(piece.transform.localPosition, neighbor.transform.localPosition, type);

        // 移动拼图
        LocalMove(neighbor, offset);

        // 打印邻居信息
        Debug.Log("Neighbor is at " + type + "\nmy :" + piece.GetComponent<Piece>() + " other :" + neighbor.GetComponent<Piece>());


        // 连接邻居
        ConnectNeighbor(piece, neighbor);


        // 添加成功
        return true;
    }


    /// <summary>
    /// 连接邻居
    /// </summary>
    /// <param name="master">当前选中的拼图</param>
    /// <param name="neighbor">要加入的邻居</param>
    void ConnectNeighbor(GameObject master, GameObject neighbor)
    {
        int index = master.GetComponent<Piece>().connectedListID;

        // 当前拼图 在连接表
        List<GameObject> pieces = connectedPieces[index];

        // 当前拼图的顺序
        int order = master.GetComponent<Piece>().order;

        // 保存要加入的邻居们的连接表
        List<GameObject> neighbors = connectedPieces[neighbor.GetComponent<Piece>().connectedListID];

        // 设置 邻居 的顺序,并把所有块连接起来
        foreach (GameObject go in neighbors)
        {
            Piece nb = go.GetComponent<Piece>();

            // 设置顺序
            nb.order = order;

            // 加入到新的表中
            pieces.Add(go);

            // 更新邻居在表中索引
            nb.connectedListID = index;
        }

        // 清空原来的表
        neighbors.Clear();
    }


    /// <summary>
    /// 获取两个拼图需要移动的偏移
    /// </summary>
    /// <param name="A">当前选中的拼图的位置</param>
    /// <param name="B">需要移动的另一个拼图的位置</param>
    /// <param name="type">第二个拼图关于第一个拼图的位置</param>
    /// <returns></returns>
    Vector3 GetNeighborOffset(Vector3 A, Vector3 B, NeighborType type)
    {
        // 邻居在 Y 轴上的偏移
        Vector3 Y = new Vector3(0, size.y, 0);

        // 邻居在 X 轴上的偏移
        Vector3 X = new Vector3(size.x, 0, 0);

        // 邻居要移动到的位置
        Vector3 pos = Vector3.zero;

        // 判断邻居的类型，计算偏移
        switch (type)
        {
            // 邻居在上面
            case NeighborType.Top:
                // 在 Y 轴上增加偏移
                pos = A + Y;
                break;

            // 邻居在下面
            case NeighborType.Bottom:
                // 在 Y 轴上减少偏移
                pos = A - Y;
                break;

            // 邻居在左边
            case NeighborType.Left:
                // 在 X 轴上减少偏移
                pos = A - X;
                break;

            // 邻居在右边
            case NeighborType.Right:
                // 在 X 轴上增加偏移
                pos = A + X;
                break;

            // 不是邻居
            default:
                break;
        }

        // 返回要移动的偏移
        return pos - B;
    }



    /// <summary>
    /// 两个相邻的拼图是否足够接近
    /// </summary>
    /// <param name="A">当前选中的拼图的位置</param>
    /// <param name="B">需要移动的另一个拼图的位置</param>
    /// <param name="type">邻居类型</param>
    /// <returns>是否足够接近</returns>
    bool IsClosedToConnect(Vector3 A, Vector3 B, NeighborType type)
    {
        // 保存当前拼图在 Y 轴大小的一半
        Vector3 Y = new Vector3(0, size.y / 2, 0);

        // 保存当前拼图在 X 轴大小的一半
        Vector3 X = new Vector3(size.x / 2, 0, 0);

        Vector3 a, b;
        switch (type)
        {
            // 邻居在上面
            case NeighborType.Top:

                // 当前对象 加上 Y 轴的偏移的一半
                a = A + Y;

                // 邻居 减去 Y 轴的偏移的一半
                b = B - Y;
                break;

            // 邻居在下面
            case NeighborType.Bottom:

                // 当前对象 减去 Y 轴的偏移的一半
                a = A - Y;

                // 邻居 加上 Y 轴的偏移的一半
                b = B + Y;
                break;

            // 邻居在左边
            case NeighborType.Left:

                // 当前对象 减去 X 轴的偏移的一半
                a = A - X;

                // 邻居 加上 X 轴的偏移的一半
                b = B + X;
                break;

            // 邻居在右边
            case NeighborType.Right:

                // 当前对象 加上 X 轴的偏移的一半
                a = A + X;

                // 邻居 减去 X 轴的偏移的一半
                b = B - X;
                break;

            // 不是邻居，返回 false
            default:
                return false;
        }

        // 测试当前 距离是否小于 设置的大小，并返回结果
        return Vector3.Distance(a, b) < manager.largestSize;
    }





    #endregion

}