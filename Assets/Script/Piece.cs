using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {


    static int maxDepth = 0;
    static GameObject topGameObject = null;
    static bool GameStarted = false;
    public static List<GameObject> pieceCache ;


    PieceID pid;

    SpriteRenderer sprite;

    public List<GameObject> connectedPieces ;

    #region unity callback
    
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == null) return;
        if (!GameStarted) return;

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
            sprite.sortingOrder = ++maxDepth;
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
    }

    public override string ToString()
    {
        return "Piece " + pid.ToString();
    }

    #endregion


    #region function

    

    Vector3 GetNeighborOffset(GameObject other, NeighborType type)
    {
        float sizeY = Puzzle.instance.pieceSize.y;
        float sizeX = Puzzle.instance.pieceSize.x;
        Vector3 pos = Vector3.zero;
        switch (type)
        {
            case NeighborType.Top:
                pos = gameObject.transform.localPosition +
                    new Vector3(0, sizeY, 0);
                break;
            case NeighborType.Bottom:
                pos = gameObject.transform.localPosition -
                    new Vector3(0, sizeY, 0);
                break;
            case NeighborType.Left:
                pos = gameObject.transform.localPosition -
                    new Vector3(sizeX, 0, 0);
                break;
            case NeighborType.Right:
                pos = gameObject.transform.localPosition +
                    new Vector3(sizeX, 0, 0);
                break;
            default:
                print("Neighbor type error: " + type);
                break;
        }
        Vector3 d = pos - other.gameObject.transform.localPosition ;

        return d;
   
    }

    void MoveConnectedPiece(Vector3 offset)
    {
        foreach (GameObject piece in gameObject.GetComponent<Piece>().connectedPieces)
        {
            piece.transform.localPosition += offset;
        }
    }

    void AddNeighbor(GameObject other)
    {
        //connectedPieces.Add(other.gameObject);

        // 是否是相临的两块
        NeighborType type = pid.IsNeighbor(other.GetComponent<Piece>().pid);
        if (type == NeighborType.None) return;

        print("Neighbor is at " + type);


        GetAllConnected(other);
        ConnectPiece();

        if(topGameObject == gameObject)
        {
            Vector3 offset = GetNeighborOffset(other, type);
            MoveConnectedPiece(offset);
        }


    }

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
    public PieceID(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return x + ":" + y;
    }

    public NeighborType IsNeighbor(PieceID other)
    {
        NeighborType type = NeighborType.None;

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
}