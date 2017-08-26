using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour {

    public Vector2 imageSize = new Vector2(640, 480);
    public Vector2 pieceSize = new Vector2(6,4);

    public GameObject piecePrefab;


    public static Puzzle instance;

	// Use this for initialization
	void Start () {
        if (instance == null)
            instance = this;
        if (instance != null && instance != this)
            DestroyObject(gameObject);

        DontDestroyOnLoad(gameObject);

        MakePuzzle();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


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
        GameObject piece = NGUITools.AddChild(gameObject, piecePrefab);
        piece.transform.localScale = new Vector3(100 / pieceSize.x, 100 / pieceSize.y, 1);
        piece.GetComponent<UIDragObject>().contentRect = gameObject.GetComponent<UIRect>();


        float scaleX = 1 / pieceSize.x;
        float scaleY = 1 / pieceSize.y;
        piece.GetComponent<Renderer>().material.mainTextureScale = 
            new Vector2(scaleX, scaleY);
        piece.GetComponent<Renderer>().material.mainTextureOffset = 
            new Vector2(x*scaleX,y*scaleY);
    }
}
