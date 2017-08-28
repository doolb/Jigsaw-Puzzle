using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : DragablePlane {

    [Header("Piece")]
    public Vector2 pieceCount = new Vector2(6,4);
    public GameObject piecePrefab;
    public Texture image;

    public float largestSize = 10.0f;


    [HideInInspector]
    public Vector2 pieceSize;
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
    
    protected override void ActiveObject(GameObject go)
    {
        go.GetComponent<Piece>().OnActive();
    }

    protected override void DeactiveObject(GameObject go)
    {

    }

    protected override void MoveObject(GameObject go,Vector3 delta)
    {
        go.GetComponent<Piece>().OnMove(delta);
        
    }

    
    #endregion




    public void MakePuzzle()
    {


        int x = (int)pieceCount.x;
        int y = (int)pieceCount.y;
        pieceSize = new Vector2(image.width / x, image.height / y);

        Piece.pieceCache = new List<GameObject>(x * y);

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
        piece.transform.localScale = new Vector3(100 / pieceCount.x, 100 / pieceCount.y, 1);
        
        // 设置材质
        float scaleX = 1 / pieceCount.x;
        float scaleY = 1 / pieceCount.y;
        piece.GetComponent<Renderer>().material.mainTextureScale = 
            new Vector2(scaleX, scaleY);
        piece.GetComponent<Renderer>().material.mainTextureOffset = 
            new Vector2(x*scaleX,y*scaleY);

        piece.layer = childLayer;
        piece.GetComponent<Piece>().connectedPieces = new List<GameObject>(x * y);
        piece.GetComponent<Piece>().Init(x,y);
    }
}
