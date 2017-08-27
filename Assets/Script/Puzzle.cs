using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour {

    [Header("Piece")]
    public Vector2 pieceSize = new Vector2(6,4);
    public int pieceLayer = 31;
    public GameObject piecePrefab;


	// Use this for initialization
	void Start () {

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

        piece.layer = pieceLayer;
        piece.GetComponent<Piece>().planeCollider = GetComponent<Collider>();
        piece.GetComponent<Piece>().Init(x,y);
    }
}
