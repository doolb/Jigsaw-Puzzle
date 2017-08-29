using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPiece
{
    void Init(int x, int y);

    int order
    {
        get;
        set;
    }
}


public class Piece : MonoBehaviour,IPiece {


    public static int maxDepth = 0;
    static GameObject topGameObject = null;
    static bool GameStarted = false;
    public static List<GameObject> pieceCache ;
    


    PieceID pid;
    SpriteRenderer sprite;

    public List<GameObject> connectedPieces ;

    public int order
    {
        get 
        {
            if (sprite != null) return sprite.sortingOrder;
            else return -1;
        }

        set 
        { 
            if (sprite != null) sprite.sortingOrder = value;
            foreach (GameObject go in connectedPieces)
                go.GetComponent<SpriteRenderer>().sortingOrder = value;
        }
    }


    #region unity callback
    
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sortingOrder = pid == null ? 0 : pid.order;
        
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == null) return;
        if (!GameStarted) return;

        // 只有当前选中的，或者和当前选中的相连的才能继续执行
        if (topGameObject == gameObject ||
            topGameObject.GetComponent<Piece>().IsConnected(gameObject))
            AddNeighbor(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == null) return;
        if (!GameStarted) return;

        // 只有当前选中的，或者和当前选中的相连的才能继续执行
        if (topGameObject == gameObject ||
            topGameObject.GetComponent<Piece>().IsConnected(gameObject))
            AddNeighbor(other.gameObject);
    }


    #endregion

    #region callback
    public void OnActive()
    {
        GameStarted = true;

        // 是否是最顶的对象
        if (topGameObject != gameObject)
        {
            topGameObject = gameObject;
            order = ++maxDepth;
        }
    }

    public void OnMove(Vector3 delta)
    {
        MoveConnectedPiece(delta);
    }


    #endregion

    #region public function
    public void Init(int x, int y)
    {
        pid = new PieceID(x, y);
        gameObject.name = ToString();

        // 设置 拼图 mark 材质
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetTextureOffset("_MarkTex", pid.markOffset);
    }

    public override string ToString()
    {
        return "Piece " + pid.ToString();
    }

    public bool IsConnected(GameObject piece)
    {
        return connectedPieces.Find(x => x == piece);
    }

    #endregion


    #region function

    bool AddNeighbor(GameObject other)
    {

        // 是否是相临的两块
        NeighborType type = pid.IsNeighbor(other.GetComponent<Piece>().pid);
        if (type == NeighborType.None) return false;
        // 是否足够接近
        if (!IsClosed(other, type)) return false;

        // 计算要移动的偏移,移动所有相连的块
        Vector3 offset = GetNeighborOffset(other, type);
        other.transform.localPosition += offset;
        other.GetComponent<Piece>().MoveConnectedPiece(offset);

        print("Neighbor is at " + type + "\nmy :" + gameObject.GetComponent<Piece>() + " other :" + other.GetComponent<Piece>());

        // 设置 order
        RebuildOrder(other);

        // 把所有块连接起来
        GetAllConnected(other);
        ConnectPiece();


        return true;
    }

    Vector3 GetNeighborOffset(GameObject other, NeighborType type)
    {
        Vector3 offsetY = new Vector3(0, Puzzle.instance.pieceSize.y, 0);
        Vector3 offsetX = new Vector3(Puzzle.instance.pieceSize.x, 0, 0);
        Vector3 pos = Vector3.zero;
        switch (type)
        {
            case NeighborType.Top:
                pos = gameObject.transform.localPosition + offsetY;
                break;
            case NeighborType.Bottom:
                pos = gameObject.transform.localPosition - offsetY;
                break;
            case NeighborType.Left:
                pos = gameObject.transform.localPosition - offsetX;
                break;
            case NeighborType.Right:
                pos = gameObject.transform.localPosition + offsetX;
                break;
            default:
                print("Neighbor type error: " + type);
                break;
        }
        Vector3 d = pos - other.gameObject.transform.localPosition ;

        return d;
   
    }

    bool IsClosed(GameObject other,NeighborType type)
    {
        Vector3 offsetY = new Vector3(0, Puzzle.instance.pieceSize.y / 2 , 0);
        Vector3 offsetX = new Vector3(Puzzle.instance.pieceSize.x / 2, 0, 0);
        Vector3 a, b;
        switch (type)
        {
            case NeighborType.Top:
                a = gameObject.transform.localPosition + offsetY;
                b = other.transform.localPosition - offsetY;
                break;
            case NeighborType.Bottom:
                a = gameObject.transform.localPosition - offsetY;
                b = other.transform.localPosition + offsetY;
                break;
            case NeighborType.Left:
                a = gameObject.transform.localPosition - offsetX;
                b = other.transform.localPosition + offsetX;
                break;
            case NeighborType.Right:
                a = gameObject.transform.localPosition + offsetX;
                b = other.transform.localPosition - offsetX;
                break;
            default:
                print("Neighbor type error: " + type);
                return false;
        }

        return Vector3.Distance(a, b) < Puzzle.instance.largestSize;
    }

    void MoveConnectedPiece(Vector3 offset)
    {
        foreach (GameObject piece in gameObject.GetComponent<Piece>().connectedPieces)
        {
            piece.transform.localPosition += offset;
        }
    }

    
    /// <summary>
    /// 把所有可以连接的对象添加到缓存中
    /// </summary>
    /// <param name="other"></param>
    void GetAllConnected(GameObject other)
    {
        pieceCache.Clear();

        pieceCache.Add(gameObject);
        pieceCache.Add(other);

        // 添加所有己连接的到缓存中
        foreach (GameObject piece in GetComponent<Piece>().connectedPieces)
            pieceCache.Add(piece);
        foreach (GameObject piece in other.GetComponent<Piece>().connectedPieces)
            pieceCache.Add(piece);

    }


    void ConnectPiece()
    {

        foreach(GameObject a in pieceCache)
        {
            foreach (GameObject b in pieceCache)
            {
                // 是否是同一个
                if (a == b) continue;
                // 是否已经加入
                Piece p = a.GetComponent<Piece>();
                if (a.GetComponent<Piece>().connectedPieces.Find(x => x == b)) continue;

                p.connectedPieces.Add(b);

            }
            
        }

        
    }

    /// <summary>
    /// 重建 raycast 的 order
    /// </summary>
    void RebuildOrder(GameObject other)
    {
        int order = other.GetComponent<Piece>().order;
        if (this.order > order)
            order = this.order;

        other.GetComponent<Piece>().order = order;
        

    }

    #endregion


}


enum NeighborType
{
    None,
    Top,
    Bottom,
    Left,
    Right,
    Max
};


class PieceID
{
    int x;
    int y;


    bool isAtEdge;
    public Vector2 markOffset;

    public PieceID(int x, int y)
    {
        this.x = x;
        this.y = y;

        CalcMarkOffset();
    }

    public int order
    {
        get { return x * (int)Puzzle.instance.pieceCount.y + y; }
    }

    public override string ToString()
    {
        return x + ":" + y;
    }

    

    public NeighborType IsNeighbor(PieceID other)
    {
        NeighborType type = NeighborType.None;
        if (other == null) return NeighborType.None;
        // 同一列
        if(x == other.x)
        {
            if (y + 1 == other.y) type = NeighborType.Top;
            if (y - 1 == other.y) type = NeighborType.Bottom;
            
        }
        // 同一行
        else if(y == other.y)
        {
            if (x + 1 == other.x) type = NeighborType.Right;
            if (x - 1 == other.x) type = NeighborType.Left;
        }

        return type;
    }

    void CalcMarkOffset()
    {
        isAtEdge = true;

        int countX = (int)Puzzle.instance.pieceCount.x;
        int countY = (int)Puzzle.instance.pieceCount.y;

        float offsetX= 0,offsetY = 0;
        // 左边界
        if(x==0)
        {
            // 左下角
            if (y == 0) goto _end_;
            // 左上角
            if (y == countY -1 ) { offsetY = 0.25f; goto _end_; };

            offsetX = 0.25f;
            offsetY = y % 2 == 1 ? 0.25f : 0.0f;
        }
        // 右边界
        else if(x== countX -1 )
        {

            // 右下角
            if (y == 0) { offsetY = 0.5f; goto _end_; }
            // 右上角
            if (y == countY - 1) { offsetY = 0.75f; goto _end_; };

            offsetX = 0.25f;
            offsetY = y % 2 == 1 ? 0.75f : 0.5f;
        }
        // 不用判断角落了
        // 下边界
        else if(y==0)
        {
            offsetX = 0.5f;
            offsetY = x % 2 == 1 ? 0.25f : 0f;
        }
        // 上边界
        else if(y== countY -1)
        {
            offsetX = 0.5f;
            offsetY = x % 2 == 1 ? 0.75f : 0.5f;
        }
        // 其它地方
        else 
        {
            offsetX = 0.75f;
            offsetY = x % 2 == 1 ? 0.25f : 0f;
            offsetY += y % 2 == 1 ? 0f : 0.5f;

            isAtEdge = false;
        }



        _end_:
            markOffset = new Vector2(offsetX,offsetY);
    }
}