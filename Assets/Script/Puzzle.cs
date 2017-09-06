using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生成 每块 拼图
/// </summary>
public class Puzzle : DragablePlane
{

    [Header("Piece")]

    /// <summary>
    /// 拼图的个数
    /// </summary>
    public Vector2 pieceCount = new Vector2(6, 4);

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

        piecePrefab = Resources.Load("Piece") as GameObject;
    }



    #region base callback
    /// <summary>
    /// 激活拼图
    /// </summary>
    /// <param name="go">拼图对象</param>
    protected override void ActiveObject(GameObject go)
    {
        // 通知 激活的拼图 
        go.GetComponent<Piece>().OnActive();
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
                CreatePiece(i, j);
            }
        }

        // 检查 拼图的 显示
        CheckPiece();
    }


    /// <summary>
    /// 重设拼图的大小
    /// </summary>
    protected void ReSize()
    {

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
        // 实例化一个拼图对象
        GameObject piece = Instantiate(piecePrefab, gameObject.transform);

        // 初始化 拼图
        piece.AddComponent<Piece>().Init(x, y);


        // 设置材质
        Vector2 offset = pieceScale / 2f;

        SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();

        // 设置图像偏移
        rend.material.mainTextureScale = pieceScale * 2;
        rend.material.mainTextureOffset =
            new Vector2(x * pieceScale.x - offset.x, y * pieceScale.y - offset.y);

        // 设置拼图图像
        rend.sprite = pieceImage;

        // 设置拼图形状
        rend.material.SetTexture("_MarkTex", markImage);


        // 设置 层
        piece.layer = childLayer;



        // 随机位置
        piece.transform.position = new Vector3(
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
            Transform leftTop = transform.FindChild("Piece 0:" + ((int)pieceCount.y - 1));
            leftTop.localScale = new Vector3(leftTop.localScale.x, -leftTop.localScale.y, 1);
            leftTop.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0, 0));

            // 其它的
            for (int i = 1; i < pieceCount.x - 1; i++)
            {
                Transform piece = transform.FindChild("Piece " + i + ":" + ((int)pieceCount.y - 1));
                piece.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0.5f, i % 2 == 1 ? 0.5f : 0.75f));
            }
        }

        // 右边界
        // 如果 拼图个数 x 是奇数，反转  
        if ((int)pieceCount.x % 2 == 1)
        {
            print("fix puzzle at right edge.");


            // 左上角
            int rightBottomIndex = transform.childCount - (int)pieceCount.y;
            Transform rightBottom = transform.GetChild(rightBottomIndex);
            rightBottom.localScale = new Vector3(rightBottom.localScale.x, -rightBottom.localScale.y, 1);
            rightBottom.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0, 0.75f));

            // 其它的
            for (int i = 1; i < pieceCount.y - 1; i++)
            {
                Transform piece = transform.GetChild(rightBottomIndex + i);
                piece.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0.25f, i % 2 == 1 ? 0.5f : 0.75f));
            }

        }

        // 右上角
        // 只有 x, 和 y 其中一个是奇数时，才反转
        if (((int)pieceCount.x + (int)pieceCount.y) % 2 == 1)
        {
            print("fix right top corner");

            // 右上角 在最后一个位置
            Transform piece = transform.GetChild(transform.childCount - 1);


            piece.localScale = new Vector3(piece.localScale.x, -piece.localScale.y, 1);
            piece.GetComponent<Renderer>().material.SetTextureOffset("_MarkTex", new Vector2(0, 0.5f));

        }
    }
}
