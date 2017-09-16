using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制拼图对象生成和显示
/// </summary>
public class PuzzleManager : DragablePlane
{
    /// <summary>
    /// 拼图逻辑对象
    /// </summary>
    public Puzzle puzzle;

    /// <summary>
    /// 当前顺序的最大值
    /// </summary>
    int maxDepth = 0;

    /// <summary>
    /// 第一块 拼图 的索引
    /// </summary>
    public int firstPieceIndex = 3;


    /// <summary>
    /// 拼图是否已经创建
    /// </summary>
    public bool pieceCreated;


    /// <summary>
    /// 拼图 预制体
    /// </summary>
    public GameObject piecePrefab;

    /// <summary>
    /// 拼图材质预制体
    /// </summary>
    public Material materialPrefab;

    /// <summary>
    /// 拼图 图像
    /// </summary>
    public Texture pieceImage;

    /// <summary>
    /// 拼图的 形状
    /// </summary>
    public Texture markImage;


    [Header("大小")]
    /// <summary>
    /// 可以 连接 的 最大 距离
    /// </summary>
    public float largestSize = 30.0f;

    /// <summary>
    /// 每块拼图的大小,和像素一样
    /// </summary>
    public Vector2 pieceSize;

    /// <summary>
    /// 拼图图像显示缩放
    /// </summary>
    public Vector2 pieceScale;

    [Header("旋转")]
    /// <summary>
    /// 拼图最少的旋转个数
    /// </summary>
    public int minCount = 1;

    /// <summary>
    /// 拼图最少的旋转个数
    /// </summary>
    public int maxCount = 5;

    /// <summary>
    /// 是否旋转
    /// </summary>
    public static bool isRotate;


    /// <summary>
    /// 摄像机大小
    /// </summary>
    public float camSize;

    /// <summary>
    /// 碰撞测试的 缓存大小
    /// </summary>
    protected override int raycastHitCacheSize
    {
        // 总的 拼图个数
        get { return puzzle.totalCount; }
    }

    protected override void Awake()
    {

        // 加载拼图预制体
        if (piecePrefab == null)
            piecePrefab = Resources.Load("Piece") as GameObject;

        // 加载材质预制体
        if (materialPrefab == null)
            materialPrefab = Resources.Load<Material>("piece");

        childLayer = piecePrefab.layer;

        if (puzzle == null)
            puzzle = new Puzzle(this);

        base.Awake();
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
        int index = firstPieceIndex + x * puzzle.count.y + y;

        // 如果 小于 当前 已有的拼图个数，直接返回
        if (index < transform.childCount)
            return transform.GetChild(index).gameObject;

        // 创建一个新的拼图实例
        return Instantiate(piecePrefab, transform);
    }




    /// <summary>
    /// 清理现有的拼图
    /// </summary>
    public void ClearPiece()
    {
        // 如果没有生成拼图，返回
        if (!pieceCreated) return;

        // 遍历所有拼图对象
        for (int i = 0; i < puzzle.count.x; i++)
            for (int j = 0; j < puzzle.count.y; j++)
            {
                // 隐藏 拼图对象
                GetPiece(i, j).SetActive(false);
            }

        // 拼图已经清理
        pieceCreated = false;
    }


    #region 重载基类函数
    /// <summary>
    /// 激活拼图对象
    /// </summary>
    /// <param name="go">激活的拼图对象</param>
    protected override void ActiveObject(GameObject go)
    {
        Piece piece = go.GetComponent<Piece>();

        // 选中的不是最上层的对象
        if (piece.order != maxDepth)
        {
            maxDepth++;

            // 更新所有连接拼图的 顺序
            foreach (GameObject obj in puzzle.connectedPieces[piece.connectedListID])
            {
                obj.GetComponent<Piece>().order = maxDepth;
            }
        }

        // 是否旋转，并是否按下 "Fire2" 按钮
        if (isRotate && Input.GetButton("Fire2"))
        {
            // 顺时针旋转 90 度
            go.transform.localEulerAngles -= new Vector3(0, 0, 90);
        }
    }

    /// <summary>
    /// 取消激活对象
    /// </summary>
    /// <param name="go">取消激活拼图对象</param>
    protected override void DeactiveObject(GameObject go)
    {
        // 拼图移动结束
        puzzle.MoveEnd(go);
    }

    /// <summary>
    /// 移动拼图
    /// </summary>
    /// <param name="go">拼图对象</param>
    /// <param name="delta">移动的距离</param>
    protected override void MoveObject(GameObject go, Vector3 delta)
    {
        // 通知 拼图 移动
        puzzle.Move(go, delta);
    }

    /// <summary>
    /// 碰撞测试的 优先值
    /// </summary>
    /// <param name="go">拼图对象</param>
    /// <returns>拼图对象的 优先值</returns>
    protected override int RaycastHitOrder(GameObject go)
    {
        Piece piece = go.GetComponent<Piece>();
        if (piece == null) return -1;
        return piece.order;
    }



    #endregion


    #region 管理拼图函数

    /// <summary>
    /// 生成拼图
    /// </summary>
    protected void MakePuzzle()
    {

        // 设置 拼图 的最大 优先值
        maxDepth = puzzle.totalCount + 1;

        ReSize();

        // 生成 （x,y) 个 拼图块
        for (int i = 0; i < puzzle.count.x; i++)
        {
            for (int j = 0; j < puzzle.count.y; j++)
            {
                // 创建 第 （ x,y ) 个拼图
                CreatePiece(i, j);
            }
        }

        // 检查 拼图的 显示
        CheckPiece();


        // 重置拼图
        puzzle.Reset();

        // 拼图已经创建
        pieceCreated = true;
    }






    /// <summary>
    /// 重设拼图的大小
    /// </summary>
    protected virtual void ReSize()
    {
        // 如果 拼图对象为空，返回
        if (pieceImage == null) return;

        // 更新 图像大小
        pieceSize = new Vector2(pieceImage.width / puzzle.count.x, pieceImage.height / puzzle.count.y);

        // 更新 拼图标准缩放值
        pieceScale.x = 1 / (float)puzzle.count.x;
        pieceScale.y = 1 / (float)puzzle.count.y;


        // 新的摄像机大小
        camSize = pieceImage.width / 810.0f;

        // 判断摄像机大小是否更改
        if (cam.orthographicSize != camSize)
        {
            // 获取更新偏移
            float offset = camSize / cam.orthographicSize;

            // 设置新的大小
            cam.orthographicSize = camSize;

            // 更新拼图的显示
            for (int x = 0; x < puzzle.count.x; x++)
                for (int y = 0; y < puzzle.count.y; y++)
                {
                    GetPiece(x, y).transform.localPosition *= offset;
                }
        }
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

        // 随机位置
        child.transform.localPosition = new Vector3(
                                        (Random.value - 0.5f) * pieceImage.width * 1.2f,
                                        (Random.value - 0.5f) * pieceImage.height * 1.2f,
                                        0);




        // 初始化 拼图
        Piece piece = child.GetComponent<Piece>();
        if (piece == null) piece = child.AddComponent<Piece>();

        // 设置参数
        piece.manager = this;

        // 初始化拼图
        piece.Init(x, y);
    }



    /// <summary>
    /// 检查 拼图的 显示
    /// </summary>
    void CheckPiece()
    {
        // 上边界
        // 如果 拼图个数 y 是奇数，反转  
        if (puzzle.count.y % 2 == 1)
        {
            Debug.Log("fix puzzle at top edge.");

            // 左上角
            Transform leftTop = GetPiece(0, puzzle.count.y - 1).transform;

            // 反转 Y 轴显示
            leftTop.localScale = new Vector3(leftTop.localScale.x, -leftTop.localScale.y, 1);

            // 设置成 左下角的形状
            leftTop.GetComponent<Piece>().rend.material.SetTextureOffset("_MarkTex", new Vector2(0, 0));


            // 遍历 上边界的其它拼图，不包括 右上角
            for (int i = 1; i < puzzle.count.x - 1; i++)
            {
                // 获取拼图对象
                GameObject piece = GetPiece(i, puzzle.count.y - 1);

                // 反转 形状
                piece.GetComponent<Piece>().rend.material.SetTextureOffset("_MarkTex", new Vector2(0.5f, i % 2 == 1 ? 0.5f : 0.75f));
            }
        }

        // 右边界
        // 如果 拼图个数 x 是奇数，反转  
        if (puzzle.count.x % 2 == 1)
        {
            Debug.Log("fix puzzle at right edge.");

            // 右下角
            Transform rightBottom = GetPiece(puzzle.count.x - 1, 0).transform;

            // 反转 Y 轴显示
            rightBottom.localScale = new Vector3(rightBottom.localScale.x, -rightBottom.localScale.y, 1);

            // 设置成 右上角的形状
            rightBottom.GetComponent<Piece>().rend.material.SetTextureOffset("_MarkTex", new Vector2(0, 0.75f));

            // 遍历 右边界的其它拼图，不包括 右上角
            for (int i = 1; i < puzzle.count.y - 1; i++)
            {
                // 获取拼图对象
                GameObject piece = GetPiece(puzzle.count.x - 1, i);

                // 反转 形状
                piece.GetComponent<Piece>().rend.material.SetTextureOffset("_MarkTex", new Vector2(0.25f, i % 2 == 1 ? 0.5f : 0.75f));
            }

        }

        // 右上角
        // 只有 x, 和 y 其中一个是奇数时，才反转
        if ((puzzle.count.x + puzzle.count.y) % 2 == 1)
        {
            Debug.Log("fix right top corner");

            // 获取拼图对象
            Transform piece = GetPiece(puzzle.count.x - 1, puzzle.count.y - 1).transform;

            // 反转 Y 轴显示
            piece.localScale = new Vector3(piece.localScale.x, -piece.localScale.y, 1);

            // 设置成 右下角的形状
            piece.GetComponent<Piece>().rend.material.SetTextureOffset("_MarkTex", new Vector2(0, 0.5f));

        }
    }

    #endregion


    IEnumerator SmoothCameraSize(float target)
    {
        float delta = target - cam.orthographicSize;
        float once = delta / 10;

        for (int i = 0; i < 10; i++)
        {
            cam.orthographicSize += once;

            yield return new WaitForSeconds(0.1f);
        }
    }
}
