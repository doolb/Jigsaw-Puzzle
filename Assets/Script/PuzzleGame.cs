using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个用于和UI元素交互
/// </summary>
public class PuzzleGame : Puzzle {

    
    public UIPlayAnimation menuButtonPlayAnimation;
    public UILabel startButtonLabel;

    bool pieceCreateed;
	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        Time.timeScale = 0f;
    }
	

    #region public function
    public void StartGame()
    {
        if (pieceCreateed) return;
        pieceCreateed = true;

        MakePuzzle();
        startButtonLabel.text = "继续";
    }

    public void ShowAllPieceOrNot()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            //print(transform.GetChild(i).gameObject.GetComponent<Piece>());
            GameObject child = transform.GetChild(i).gameObject;
            if (!child.GetComponent<Piece>().pid.isAtEdge)
            {
                child.SetActive(UIToggle.current.value);
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
}
