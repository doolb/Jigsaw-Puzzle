using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(UITexture))]
/// <summary>
/// 这个用于拼图的每个块的显示
/// </summary>
public class Piece : MonoBehaviour
{

    #region 变量

    /// <summary>
    /// 拼图逻辑管理对象
    /// </summary>
    public Puzzle puzzle
    {
        get { return manager.puzzle; }
    }

    /// <summary>
    /// 拼图管理对象
    /// </summary>
    public PuzzleManager manager;


    /// <summary>
    /// 图像渲染对象
    /// </summary>
    public UITexture rend;

    /// <summary>
    /// 当前拼图 ID 值
    /// </summary>
    public Count id;



    /// <summary>
    /// 是否在边界上
    /// </summary>
    public bool isAtEdge;

    /// <summary>
    /// 拼图形状偏移
    /// </summary>
    public Vector2 markOffset;

    /// <summary>
    /// 当前拼图块在 连接列表的 索引
    /// </summary>
    public int connectedListID;

    /// <summary>
    /// 已经连接的拼图个数
    /// </summary>
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
            // 返回 显示的深度
            return rend.depth;
        }

        set
        {
            // 设置 显示的深度
            rend.depth = value;
        }
    }

    #endregion

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        // 获取图像渲染对象
        rend = GetComponent<UITexture>();

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


        if(rend.material == null)
            rend.material = Instantiate<Material>(manager.materialPrefab);

        SetDisplay(x, y);

        // 更新大小
        ReSize();
    }

    /// <summary>
    /// 更新拼图的大小
    /// </summary>
    public void ReSize()
    {
        // 设置拼图大小
        transform.localScale = new Vector3(puzzle.size.x,puzzle.size.y, 1);
    }

    public void Flush()
    {
        rend.enabled = false;
        rend.enabled = true;
    }
    #endregion


    void SetDisplay(int x,int y)
    {
        // 设置拼图图像
        rend.mainTexture = manager.pieceImage;

        // 设置图像 uv
        rend.material.mainTextureScale = manager.pieceScale * 2f;

        rend.material.mainTextureOffset = new Vector2(x * manager.pieceScale.x, y * manager.pieceScale.y);



        // 设置拼图形状
        rend.material.SetTexture("_MarkTex", manager.markImage);

        rend.material.SetTextureScale("_MarkTex", new Vector2(0.25f, 0.25f));

        // 计算拼图形状
        CalcMarkOffset();
        // 设置拼图偏移
        rend.material.SetTextureOffset("_MarkTex", markOffset);


        // 刷新显示
        Flush();
    }


    /// <summary>
    /// 计算拼图形状 贴图的 uv 偏移
    /// </summary>
    void CalcMarkOffset()
    {
        // 设置默认值
        markOffset = Vector2.zero;

        isAtEdge = true;

        // 左边界
        if (id.x == 0)
        {
            // 左下角, uv = (0,0)
            if (id.y == 0) goto _end_;

            // 左上角, uv = (0,0.25)
            if (id.y == puzzle.count.y - 1) { markOffset.y = 0.25f; goto _end_; };

            // 在贴图的第 2 列的位置
            markOffset.x = 0.25f;

            // 交替设置 y 的偏移
            markOffset.y = id.y % 2 == 1 ? 0.25f : 0.0f;
        }
        // 右边界
        else if (id.x == puzzle.count.x - 1)
        {

            // 右下角, uv = (0,0.5)
            if (id.y == 0) { markOffset.y = 0.5f; goto _end_; }

            // 右上角, uv = (0,0.75)
            if (id.y == puzzle.count.y - 1) { markOffset.y = 0.75f; goto _end_; };

            // 在贴图的第 2 列的位置
            markOffset.x = 0.25f;

            // 交替设置 y 的偏移
            markOffset.y = id.y % 2 == 1 ? 0.75f : 0.5f;
        }


        // 不用判断角落了
        // 下边界
        else if (id.y == 0)
        {
            // 在贴图的第 3 列的位置
            markOffset.x = 0.5f;

            // 交替设置 y 的偏移
            markOffset.y = id.x % 2 == 1 ? 0.25f : 0f;
        }
        // 上边界
        else if (id.y == puzzle.count.y - 1)
        {
            // 在贴图的第 3 列的位置
            markOffset.x = 0.5f;

            // 交替设置 y 的偏移
            markOffset.y = id.x % 2 == 1 ? 0.75f : 0.5f;
        }
        // 其它地方
        else
        {
            // 在贴图的第 4 列的位置
            markOffset.x = 0.75f;

            // 交替设置 y 的偏移
            markOffset.y = id.x % 2 == 1 ? 0.25f : 0f;
            markOffset.y += id.y % 2 == 1 ? 0f : 0.5f;

            // 当前不在边界上
            isAtEdge = false;
        }



    _end_:
        return;
    }

}
