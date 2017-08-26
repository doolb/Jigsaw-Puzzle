using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : UIDragDropItem {


    static int maxDepth = 0;
    static GameObject topGameObject;
    static bool GameStarted = false;


    PieceID pid;

    SpriteRenderer sprite;

    List<GameObject> neighbors = new List<GameObject>();

    #region unity callback
    
    protected override void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == null) return;
        if (!GameStarted) return;


        print("OnTriggerEnter " + other.gameObject.GetComponent<Piece>());

        AddNeighbor(other.gameObject);
        
    }

    #endregion

    #region ngui callback
    protected override void OnPress(bool isPressed)
    {
        base.OnPress(isPressed);

        GameStarted = true;

        // 是否是最顶的对象
        if(topGameObject != gameObject)
        {
            topGameObject = gameObject;
            sprite.sortingOrder = ++maxDepth;
        }
    }




    #endregion

    #region function

    public void Init(int x,int y)
    {
        SetID(x, y);
        gameObject.name = ToString();
    }
    void SetID(int x,int y)
    {
        pid = new PieceID(x, y);
    }

    public override string ToString()
    {
        return "Piece " + pid.ToString();
    }

    void AddNeighbor(GameObject other)
    {
        // 是否是相临的两块
        NeighborType type = pid.IsNeighbor(other.GetComponent<Piece>().pid);
        if (type == NeighborType.None) return;

        print("Neighbor is at " + type);

        neighbors.Add(other.gameObject);
        
        // 移动到当前选择
        if (topGameObject == gameObject)
        {
            Vector3 pos = Vector3.zero;
            float sizeY = 120, sizeX = 108;
            switch(type)
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
            other.gameObject.transform.localPosition = pos;
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
            type = y < other.y ? NeighborType.Top :
                                 NeighborType.Bottom;
        }
        // 同一行
        else if(y == other.y)
        {
            type = x < other.x ? NeighborType.Right :
                                 NeighborType.Left;
        }

        return type;
    }
}