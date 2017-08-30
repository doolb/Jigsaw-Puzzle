using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个用于和UI元素交互
/// </summary>
public class PuzzleGame : Puzzle {

    
    public UIPlayAnimation menuButtonPlayAnimation;


	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        Time.timeScale = 0f;
    }
	

    #region public function
    public void StartGame()
    {
        if (Piece.GameStarted) return;
        MakePuzzle();
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
