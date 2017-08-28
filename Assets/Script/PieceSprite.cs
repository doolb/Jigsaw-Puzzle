using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSprite : MonoBehaviour {

    [Header("Corner")]
    public Sprite cornerLeftBottom;
    public Sprite cornerLeftTop;
    public Sprite cornerRightBottom;
    public Sprite cornerRightTop;

    [Header("Edge")]
    public Sprite edgeBottomA;
    public Sprite edgeTopA;
    public Sprite edgeBottomB;
    public Sprite edgeTopB;

    public Sprite edgeLeftA;
    public Sprite edgeLeftB;
    public Sprite edgeRightA;
    public Sprite edgeRightB;

    [Header("Center")]
    public Sprite left;
    public Sprite right;



    [HideInInspector]
	public static PieceSprite instance;

    void Start()
    {
        if (instance == null)
            instance = this;
        if (instance != null && instance != this)
            DestroyObject(gameObject);
    }

    /// <summary>
    /// 获取拼图的 sprite
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Sprite GetSprite(int x,int y)
    {
        // 左边界
        if(x==0)
        {
            // 角落
            if (y == 0) return cornerLeftBottom;
            if (y == (int)Puzzle.instance.pieceCount.y -1) return cornerLeftTop;

            return y % 2 == 1 ? edgeLeftA : edgeLeftB;

        }
        // 右边界
        else if(x == (int)Puzzle.instance.pieceCount.x -1 )
        {

            // 角落
            if (y == 0) return cornerRightBottom;
            if (y == (int)Puzzle.instance.pieceCount.y -1) return cornerRightTop;


            return y % 2 == 1 ? edgeRightA : edgeRightB;
        }

        // 下边界
        if(y==0)
            return x % 2 == 1 ? edgeBottomA : edgeBottomB;

        // 上边界
        if (y == 0)
            return x % 2 == 1 ? edgeTopA : edgeTopB;

        return left;
    }

}
