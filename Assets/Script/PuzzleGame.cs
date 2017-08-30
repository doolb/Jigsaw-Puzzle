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

    bool pieceCreated;
    List<int> randomBuffer = new List<int>();
	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        Time.timeScale = 0f;
        tileBottomOrigin = transform.GetChild(1).localPosition;
        tileTopOrigin = transform.GetChild(2).localPosition;
    }
	

    #region public function
    public void StartGame()
    {
        if (pieceCreated) return;
        pieceCreated = true;

        MakePuzzle();
        startButtonLabel.text = "继续";
    }

    public void ShowAllPieceOrNot()
    {
        if (!pieceCreated) return;

        for (int i = firstPieceIndex; i < transform.childCount; i++)
        {
            //print(transform.GetChild(i).gameObject.GetComponent<Piece>());
            GameObject child = transform.GetChild(i).gameObject;
            if (!child.GetComponent<Piece>().pid.isAtEdge)
            {
                child.SetActive(UIToggle.current.value);
            }
        }
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

    #endregion

    #region function
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

    #endregion
}
