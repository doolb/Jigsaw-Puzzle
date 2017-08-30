using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个用于和UI元素交互
/// </summary>
public class PuzzleGame : Puzzle {

	// Use this for initialization
    protected override void Start()
    {
        base.Start();
	}
	
	// Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    #region public function
    public void StartGame()
    {
        MakePuzzle();
    }

    #endregion
}
