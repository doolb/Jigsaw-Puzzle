using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : DragablePlane {

    [Header("Piece")]
    public Vector2 pieceSize = new Vector2(6,4);
    public GameObject piecePrefab;


    public static Puzzle instance;

	// Use this for initialization
	protected void Start () {

        // 启动单实例
        if (instance == null)
            instance = this;
        if (instance != null && instance != this)
            DestroyObject(gameObject);

        DontDestroyOnLoad(gameObject);

        base.Start();

        MakePuzzle();
	}
	
	// Update is called once per frame
    protected void Update()
    {
        base.Update();
	}


    #region base callback
    /*
    protected override void ActiveObject()
    {
        GameStarted = true;

        // 是否是最顶的对象
        if (topGameObject != gameObject)
        {
            topGameObject = gameObject;
            topGameObject.GetComponent<SpriteRenderer>().sortingOrder = ++maxDepth;
        }
    }

    protected override void DeactiveObject()
    {

    }

    protected override void MoveObject(Vector3 delta)
    {
        print("move " + pid);
        foreach (GameObject nb in neighbors)
            nb.transform.localPosition += delta;
    }

    */
    #endregion




    public void MakePuzzle()
    {

        int x = (int)pieceSize.x;
        int y = (int)pieceSize.y;

        for(int i=0;i< x; i++)
        {
            for(int j=0;j< y; j++)
            {
                CreatePiece(i,j);
            }
        }

    }

    void CreatePiece(int x,int y)
    {
        GameObject piece = Instantiate(piecePrefab, gameObject.transform);

        // 设置大小
        piece.transform.localScale = new Vector3(100 / pieceSize.x, 100 / pieceSize.y, 1);
        //piece.transform.GetComponent<
        // 设置材质
        float scaleX = 1 / pieceSize.x;
        float scaleY = 1 / pieceSize.y;
        piece.GetComponent<Renderer>().material.mainTextureScale = 
            new Vector2(scaleX, scaleY);
        piece.GetComponent<Renderer>().material.mainTextureOffset = 
            new Vector2(x*scaleX,y*scaleY);

        piece.layer = childLayer;
        piece.GetComponent<Piece>().Init(x,y);
    }
}
