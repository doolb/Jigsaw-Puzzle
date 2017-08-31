using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个用于和UI元素交互
/// </summary>
public class PuzzleGame : Puzzle {

    public int firstPieceIndex = 3;

    
    public UIPlayAnimation menuButtonPlayAnimation;
    public UILabel startButtonLabel;

    Vector3 tileBottomOrigin, tileTopOrigin;

    bool isShowAll;
    bool pieceCreated;
    List<int> randomBuffer = new List<int>();
    bool isPieceCountChange;
    Vector2 newPieceCount;

    int moveCount = 0;

	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        Time.timeScale = 0f;
        tileBottomOrigin = transform.GetChild(1).localPosition;
        tileTopOrigin = transform.GetChild(2).localPosition;
    }


    #region virtual function
    protected override void ActiveObject(GameObject go)
    {
        base.ActiveObject(go);

        moveCount++;
    }

    protected override void DeactiveObject(GameObject go)
    {
        // 是否和所有块相连
        Piece piece = go.GetComponent<Piece>();
        if(piece.connectedPieces.Count == (int)(pieceCount.x * pieceCount.y) - 1)
        {
            print("finish.");
        }
    }

    #endregion

    #region public function
    public void StartGame()
    {
        if (startButtonLabel.text == "继续")
        {
            return;
        }
        else
        {
            if (isPieceCountChange)
            {
                pieceCount = newPieceCount;
                ClearPiece();
            }
            MakePuzzle();
            ShowAllOrNot(isShowAll);

            pieceCreated = true;
            Piece.theFirstRun = false;
            startButtonLabel.text = "继续";
            moveCount = 0;
        }

    }

    public void ShowAllPieceOrNot()
    {
        if (!pieceCreated) return;

        isShowAll = UIToggle.current.value;
        ShowAllOrNot(isShowAll);
    }

    public void TilePiece()
    {
        if(!pieceCreated) return;

        BuildRandomBuffer(firstPieceIndex, transform.childCount);

        int maxVCount = (int)((tileTopOrigin.y - tileBottomOrigin.y) /
                            (pieceSize.y * .8f));
        int count = 0;
        for (int i = firstPieceIndex; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(randomBuffer[i - firstPieceIndex]).gameObject;
            if (!child.active) continue;
            if (child.GetComponent<Piece>().connectedPieces.Count == 0)
            {

                // 移动到新位置
                int x = count / maxVCount;
                int y = count % maxVCount;

                child.transform.localPosition = tileBottomOrigin +
                    new Vector3(x * pieceSize.x, y * pieceSize.y, 0);

                count++;
            }
        }
    }

    public void ShowControlPanel()
    {
        menuButtonPlayAnimation.enabled = false;
        Time.timeScale = 0f;
    }

    public void HideControlPanel()
    {
        menuButtonPlayAnimation.enabled = true;
        Time.timeScale = 1f;

    }

    public void SetPieceCount()
    {
        newPieceCount = GetPieceCount( int.Parse(UIPopupList.current.value.Trim()));

        // 游戏是否未开始
        if(!pieceCreated)
        {
            pieceCount = newPieceCount;
            return;
        }

        isPieceCountChange = newPieceCount != pieceCount;

        if (isPieceCountChange)
        {
            startButtonLabel.text = "开始";
        }
        else
        {
            startButtonLabel.text = "继续";
        }
    }

    public void SetPieceShape()
    {
        string markName = "Image/puzzle mark/" + GetShapeMarkFromName(UIPopupList.current.value);
        markImage = Resources.Load<Texture>(markName);
        if (pieceCreated) UpdatePieceMark();
    }

    #endregion

    #region function

    void ShowAllOrNot(bool show)
    {
        for (int i = firstPieceIndex; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (!child.GetComponent<Piece>().pid.isAtEdge)
            {
                child.SetActive(show);
            }
        }
    }

    void BuildRandomBuffer(int min, int max)
    {
        randomBuffer.Clear();

        // 初始化列表
        for (int i = min; i < max; i++)
            randomBuffer.Add(i);

        // 随机化列表
        for(int i=0; i < randomBuffer.Count; i++)
        {
            int index = Random.Range(0, randomBuffer.Count);
            int temp = randomBuffer[i];
            randomBuffer[i] = randomBuffer[index];
            randomBuffer[index] = temp;
        }
    }

    string GetShapeMarkFromName(string name)
    {
        // 标准 角型 弧型
        switch(name.Trim())
        {
            case "标准": return "normal";
            case "角型": return "angle";
            case "弧型": return "arc";
        }

        return "normal";
    }

    void UpdatePieceMark()
    {
        for (int i = firstPieceIndex; i < transform.childCount; i++)
        {
            Renderer rend = transform.GetChild(i).gameObject.GetComponent<Renderer>();
            rend.material.SetTexture("_MarkTex", markImage);
        }
    }

    Vector2 GetPieceCount(int count)
    {
        int x = 6, y = 4;
        switch(count)
        {
            case 24: x = 6; y = 4; break;
            case 48: x = 8; y = 6; break;
            case 63: x = 9; y = 7; break;
            case 108: x = 12; y = 9; break;
            case 192: x = 16; y = 12; break;
            case 300: x = 25; y = 12; break;
            case 520: x = 26; y = 20; break;
            case 768: x = 32; y = 24; break;
        }

        return new Vector2(x, y);
    }

    void  ClearPiece()
    {
        while(transform.childCount > firstPieceIndex)
        {
            GameObject go = transform.GetChild(firstPieceIndex).gameObject;
            go.transform.parent = null;
            Destroy(go);
        }
    }

    #endregion
}
