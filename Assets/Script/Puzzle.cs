using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Puzzle : DragablePlane {

    [Header("Piece")]
    public Vector2 pieceCount = new Vector2(6,4);
    public GameObject piecePrefab;
    public Sprite  pieceImage;
    public Texture markImage;

    public float largestSize = 10.0f;



    // 显示信息
    [HideInInspector]
    public float displayX = 640, displayY = 480;
    public Vector2 displayRatio = new Vector2(1, 1); // 自缩放的比率
    public Vector2 pieceSize;   // 原始的像素大小
    public Vector2 displaySize; // 实际显示的大小



    [HideInInspector]
    public static Puzzle instance;

    protected override int raycastHitCacheSize
    {
        get { return ((int)pieceCount.x) * ((int)pieceCount.y); }
    }


	// Use this for initialization
	protected override void Start () {

        // 启动单实例
        if (instance == null)
            instance = this;
        if (instance != null && instance != this)
            DestroyObject(gameObject);

        DontDestroyOnLoad(gameObject);

        base.Start();

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

    protected override int RaycastHitOrder(GameObject go)
    {
        return go.GetComponent<Piece>().order;
    }


    
    #endregion


    


    protected void MakePuzzle()
    {


        int x = (int)pieceCount.x;
        int y = (int)pieceCount.y;


        Piece.maxDepth = (int)pieceCount.x * (int)pieceCount.y + 1;
        Piece.pieceCache = new List<GameObject>(x * y);

        for(int i=0;i< x; i++)
        {
            for(int j=0;j< y; j++)
            {
                CreatePiece(i,j);
            }
        }

    }

    protected void ReSize()
    {


        if (pieceImage == null) return;

        transform.localScale = new Vector3(
            pieceImage.texture.width / (pieceImage.texture.height / displayY) / displayX,
            1, 1);

        pieceSize = new Vector2(pieceImage.texture.width / pieceCount.x, pieceImage.texture.height / pieceCount.y);
        
        displayRatio.x = displayX / pieceImage.texture.width;
        displayRatio.y = displayY / pieceImage.texture.height;

        displaySize.x = displayRatio.x * pieceSize.x * transform.localScale.x;
        displaySize.y = displayRatio.y * pieceSize.y;


        
    }
    
    void CreatePiece(int x,int y)
    {
        GameObject piece = Instantiate(piecePrefab, gameObject.transform);

        
        
        


        // 设置材质
        float scaleX = 1 / pieceCount.x;
        float scaleY = 1 / pieceCount.y;
        float offsetX = scaleX / 2f;
        float offsetY = scaleY / 2f;
        SpriteRenderer rend = piece.GetComponent<SpriteRenderer>();
        rend.material.mainTextureScale = 
            new Vector2(scaleX * 2, scaleY * 2);
        rend.material.mainTextureOffset =
            new Vector2(x * scaleX - offsetX, y * scaleY - offsetY);

        rend.sprite = pieceImage;

        rend.material.SetTexture("_MarkTex", markImage);



        piece.layer = childLayer;
        piece.GetComponent<Piece>().Init(x,y);
        
        // 随机位置
        piece.transform.position = new Vector3(
                                        Random.Range(-0.15f,0.15f) * collider.size.x ,
                                        Random.Range(-0.15f,0.15f) * collider.size.y ,
                                        0);
    }


}
