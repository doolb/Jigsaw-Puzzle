using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个类控制拼图的逻辑
/// </summary>
public class Puzzle : DragablePlane
{

    /// <summary>
    /// 第一块 拼图 的索引
    /// </summary>
    public int firstPieceIndex = 2;

    [Header("Piece")]

    /// <summary>
    /// 拼图的个数
    /// </summary>
    public Vector2 pieceCount = new Vector2(6, 4);

    public int pieceTotalCount
    {
        get { return (int)pieceCount.x * (int)pieceCount.y; }
    }

    /// <summary>
    /// 拼图 预制体
    /// </summary>
    public GameObject piecePrefab;

    /// <summary>
    /// 拼图 图像
    /// </summary>
    public Sprite pieceImage;

    /// <summary>
    /// 拼图的 形状
    /// </summary>
    public Texture markImage;

    /// <summary>
    /// 自动连接 的最大 距离
    /// </summary>
    public float largestSize = 30.0f;


    [HideInInspector]

    /// <summary>
    /// 计算显示 的 标准尺寸
    /// </summary>
    public const float displayX = 640, displayY = 480;


    /// <summary>
    /// 拼图的标准缩放值
    /// </summary>
    public Vector2 pieceScale;

    /// <summary>
    /// 拼图和标准尺寸缩放比率
    /// </summary>
    public Vector2 displayRatio = new Vector2(1, 1);

    /// <summary>
    /// 每块拼图的像素大小
    /// </summary>
    public Vector2 pieceSize;

    /// <summary>
    /// 每块拼图的实际显示的大小
    /// </summary>
    public Vector2 displaySize;


    /// <summary>
    /// 拼图是否已经创建
    /// </summary>
    public bool pieceCreated;

    List<GameObject> neighbors = new List<GameObject>();

    [HideInInspector]
    /// <summary>
    /// 单一实例
    /// </summary>
    public static Puzzle instance;

    /// <summary>
    /// 碰撞测试的 缓存大小
    /// </summary>
    protected override int raycastHitCacheSize
    {
        // 总的 拼图个数
        get { return ((int)pieceCount.x) * ((int)pieceCount.y); }
    }


    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Start()
    {
        base.Start();


        // 启动单实例
        if (instance == null)
            instance = this;
        if (instance != null && instance != this)
            DestroyObject(gameObject);

        DontDestroyOnLoad(gameObject);

        // 加载拼图预制体
        piecePrefab = Resources.Load("Piece") as GameObject;
    }



    #region 重载基类函数
    /// <summary>
    /// 激活拼图对象
    /// </summary>
    /// <param name="go">激活的拼图对象</param>
    protected override void ActiveObject(GameObject go)
    {
        base.ActiveObject(go);

        // 通知 激活的拼图 
        go.GetComponent<Piece>().OnActive();
    }

    /// <summary>
    /// 取消激活对象
    /// </summary>
    /// <param name="go">取消激活拼图对象</param>
    protected override void DeactiveObject(GameObject go)
    {
        base.DeactiveObject(go);

        // 记录是否已经添加
        bool add = false;

        // 寻找最近的邻居

        GameObject nb = GetCloestNeighbor(go);

        // 如果找到 就 通知当前拼图块添加
        if (nb != null)
            add = go.GetComponent<Piece>().AddNeighbor(nb);

        // 如果添加成功，返回
        if (add) return;

        // 遍历 已经连接的 块
        foreach (GameObject obj in go.GetComponent<Piece>().connectedPieces)
        {
            // 寻找最近的邻居
            nb = GetCloestNeighbor(obj);

            // 如果找到 就 通知当前拼图块添加
            if (nb != null)
                add = obj.GetComponent<Piece>().AddNeighbor(nb);

            // 如果添加成功，返回
            if (add) return;
        }

    }

    /// <summary>
    /// 移动拼图
    /// </summary>
    /// <param name="go">拼图对象</param>
    /// <param name="delta">移动的距离</param>
    protected override void MoveObject(GameObject go, Vector3 delta)
    {
        // 通知 拼图 移动
        go.GetComponent<Piece>().OnMove(delta);
    }

    /// <summary>
    /// 碰撞测试的 优先值
    /// </summary>
    /// <param name="go">拼图对象</param>
    /// <returns>拼图对象的 优先值</returns>
    protected override int RaycastHitOrder(GameObject go)
    {
        // 返回 拼图对象的 优先值
        return go.GetComponent<Piece>().order;
    }



    #endregion

    /// <summary>
    /// 生成拼图
    /// </summary>
    protected void MakePuzzle()
    {
        // 设置 拼图 的最大 优先值
        Piece.maxDepth = (int)pieceCount.x * (int)pieceCount.y + 1;

        // 生成 （x,y) 个 拼图块
        for (int i = 0; i < pieceCount.x; i++)
        {
            for (int j = 0; j < pieceCount.y; j++)
            {
                // 创建 第 （ x,y ) 个拼图
                CreatePiece(i, j);
            }
        }

        // 检查 拼图的 显示
        CheckPiece();

        // 拼图已经创建
        pieceCreated = true;
    }

    /// <summary>
    /// 获取 第 (x,y) 个拼图，下标从 0 开始
    /// 原点在 左下角
    /// </summary>
    /// <param name="x">第 x 列 </param>
    /// <param name="y">第 y 列 </param>
    /// <returns></returns>
    public GameObject GetPiece(int x, int y)
    {
        // 找到 拼图 的索引
        int index = firstPieceIndex + x * (int)pieceCount.y + y;

        // 如果 小于 当前 已有的拼图个数，直接返回
        if (index < transform.childCount)
            return transform.GetChild(index).gameObject;

        // 创建一个新的拼图实例
        return Instantiate(piecePrefab, transform);

    }

    /// <summary>
    /// 获取 第 (x,y) 个拼图，下标从 0 开始
    /// 原点在 左下角
    /// </summary>
    /// <param name="x">第 x 列 </param>
    /// <param name="y">第 y 列 </param>
    /// <returns></returns>
    public GameObject GetPiece(float x, float y)
    {
        // 获取 第 (x,y) 个拼图
        return GetPiece((int)x, (int)y);
    }


    /// <summary>
    /// 清理现有的拼图
    /// </summary>
    public void ClearPiece()
    {
        // 如果没有生成拼图，返回
        if (!pieceCreated) return;

        // 遍历所有拼图对象
        for (int i = 0; i < pieceCount.x; i++)
            for (int j = 0; j < pieceCount.y; j++)
            {
                // 隐藏 拼图对象
                GetPiece(i, j).SetActive(false);
            }

        // 拼图已经清理
        pieceCreated = false;
    }

    /// <summary>
    /// 重设拼图的大小
    /// </summary>
    protected void ReSize()
    {
        // 如果 拼图对象为空，返回
        if (pieceImage == null) return;

        // 适配 图像 的 宽高比
        transform.localScale = new Vector3(
            pieceImage.texture.width / (pieceImage.texture.height / displayY) / displayX,
            1, 1);

        // 更新 图像大小
        pieceSize = new Vector2(pieceImage.texture.width / pieceCount.x, pieceImage.texture.height / pieceCount.y);

        // 更新 拼图缩放比率
        displayRatio.x = displayX / pieceImage.texture.width;
        displayRatio.y = displayY / pieceImage.texture.height;

        // 更新 拼图显示大小
        displaySize.x = displayRatio.x * pieceSize.x * transform.localScale.x;
        displaySize.y = displayRatio.y * pieceSize.y;

        // 更新 拼图标准缩放值
        pieceScale.x = 1 / pieceCount.x;
        pieceScale.y = 1 / pieceCount.y;
    }

    /// <summary>
    /// 创建 第 (x,y) 个拼图
    /// </summary>
    /// <param name="x">第 x 列</param>
    /// <param name="y">第 y 行</param>
    void CreatePiece(int x, int y)
    {
        // 获取 当前 位置的 拼图对象
        GameObject child = GetPiece(x, y);

        // 激活对象
        child.SetActive(true);

        // 初始化 拼图
        Piece piece = child.GetComponent<Piece>();
        if (piece == null) piece = child.AddComponent<Piece>();
        piece.Init(x, y);


        // 设置材质
        Vector2 offset = pieceScale / 2f;

        // 获取图像渲染对象
        SpriteRenderer rend = child.GetComponent<SpriteRenderer>();

        // 设置图像 uv
        rend.material.mainTextureScale = pieceScale * 2;
        rend.material.mainTextureOffset =
            new Vector2(x * pieceScale.x - offset.x, y * pieceScale.y - offset.y);

        // 设置拼图图像
        rend.sprite = pieceImage;

        // 设置拼图形状
        rend.material.SetTexture("_MarkTex", markImage);

        // 设置 层
        child.layer = childLayer;

        // 随机位置
        child.transform.position = new Vector3(
                                        Random.Range(-0.15f, 0.15f) * collider.size.x,
                                        Random.Range(-0.15f, 0.15f) * collider.size.y,
                                        0);
    }

    /// <summary>
    /// 检查 拼图的 显示
    /// </summary>
    void CheckPiece()
    {
        // 上边界
        // 如果 拼图个数 y 是奇数，反转  
        if ((int)pieceCount.y % 2 == 1)
        {
            print("fix puzzle at top edge.");

            // 左上角
            Transform leftTop = GetPiece(0, (int)pieceCount.y - 1).transform;

            // 反转 Y 轴显示
            leftTop.localScale = new Vector3(leftTop.localScale.x, -leftTop.localScale.y, 1);

            // 设置成 左下角的形状
            leftTop.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0, 0));


            // 遍历 上边界的其它拼图，不包括 右上角
            for (int i = 1; i < pieceCount.x - 1; i++)
            {
                // 获取拼图对象
                GameObject piece = GetPiece(i, (int)pieceCount.y - 1);

                // 反转 形状
                piece.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0.5f, i % 2 == 1 ? 0.5f : 0.75f));
            }
        }

        // 右边界
        // 如果 拼图个数 x 是奇数，反转  
        if ((int)pieceCount.x % 2 == 1)
        {
            print("fix puzzle at right edge.");

            // 右下角
            Transform rightBottom = GetPiece(pieceCount.x - 1, 0).transform;

            // 反转 Y 轴显示
            rightBottom.localScale = new Vector3(rightBottom.localScale.x, -rightBottom.localScale.y, 1);

            // 设置成 右上角的形状
            rightBottom.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0, 0.75f));

            // 遍历 右边界的其它拼图，不包括 右上角
            for (int i = 1; i < pieceCount.y - 1; i++)
            {
                // 获取拼图对象
                GameObject piece = GetPiece(pieceCount.x - 1, i);

                // 反转 形状
                piece.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0.25f, i % 2 == 1 ? 0.5f : 0.75f));
            }

        }

        // 右上角
        // 只有 x, 和 y 其中一个是奇数时，才反转
        if (((int)pieceCount.x + (int)pieceCount.y) % 2 == 1)
        {
            print("fix right top corner");

            // 获取拼图对象
            Transform piece = GetPiece(pieceCount.x - 1, pieceCount.y - 1).transform;

            // 反转 Y 轴显示
            piece.localScale = new Vector3(piece.localScale.x, -piece.localScale.y, 1);

            // 设置成 右下角的形状
            piece.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0, 0.5f));

        }
    }




    /// <summary>
    /// 获取最近的邻居
    /// </summary>
    /// <param name="go">要获取邻居的对象</param>
    /// <returns></returns>
    GameObject GetCloestNeighbor(GameObject go)
    {
        // 查察所有邻居
        CheckNeighbros(go);

        // 如果可以添加的邻居数为 0，返回 null
        if (neighbors.Count == 0) return null;

        // 记录第一个邻居
        GameObject neighbor = neighbors[0];

        // 第一个邻居的距离
        float dis = Vector3.Distance(neighbors[0].transform.position, go.transform.position);

        // 测试其它邻居
        for (int i = 1; i < neighbors.Count; i++)
        {
            // 获取当前的距离
            float d = Vector3.Distance(neighbors[i].transform.position, go.transform.position);

            // 如果更近，就替换成当前的邻居
            if (d < dis)
            {
                // 更新距离
                dis = d;

                // 更新邻居
                neighbor = neighbors[i];
            }
        }


        // 返回最近的邻居
        return neighbor;
    }

    /// <summary>
    /// 检查四个邻居
    /// </summary>
    /// <param name="go">要检查的对象</param>
    void CheckNeighbros(GameObject go)
    {
        // 清空列表
        neighbors.Clear();

        // 获取拼图块控制脚本
        Piece piece = go.GetComponent<Piece>();


        // 左，跳过最左边的一列
        if (piece.pid.x > 0)
        {
            // 获取 左边的拼图
            GameObject nb = GetPiece(piece.pid.x - 1, piece.pid.y);

            // 检查是否已经连接，并且没有隐藏
            if (!piece.connectedPieces.Contains(nb) && nb.activeSelf) neighbors.Add(nb);
        }

        // 右，跳过最右边的一列
        if (piece.pid.x < pieceCount.x - 1)
        {
            // 获取 右边的拼图
            GameObject nb = GetPiece(piece.pid.x + 1, piece.pid.y);

            // 检查是否已经连接，并且没有隐藏
            if (!piece.connectedPieces.Contains(nb) && nb.activeSelf) neighbors.Add(nb);
        }

        // 下，跳过最下的一行
        if (piece.pid.y > 0)
        {
            // 获取 下边的拼图
            GameObject nb = GetPiece(piece.pid.x, piece.pid.y - 1);

            // 检查是否已经连接，并且没有隐藏
            if (!piece.connectedPieces.Contains(nb) && nb.activeSelf) neighbors.Add(nb);
        }

        // 上，跳过最上的一行
        if (piece.pid.y < pieceCount.y - 1)
        {
            // 获取 上边的拼图
            GameObject nb = GetPiece(piece.pid.x, piece.pid.y + 1);

            // 检查是否已经连接，并且没有隐藏
            if (!piece.connectedPieces.Contains(nb) && nb.activeSelf) neighbors.Add(nb);
        }
    }
}
