using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{

    public GameObject uiRoot;
    public GameObject uiRoot3D;

    public static Background background;
    public static UIControl uiControl;
    public static MenuControl menuControl;
    public static PuzzleGame puzzleGame;



    // Use this for initialization
    void Awake()
    {
        LoadScene();
    }

    void LoadScene()
    {
        LoadBackground();

        LoadGame();

        LoadUI();

    }


    void LoadBackground()
    {
        GameObject bg = Instantiate(Resources.Load("Background")) as GameObject;
        background = bg.AddComponent<Background>();
    }


    void LoadUI()
    {
        menuControl = NGUITools.AddChild(uiRoot3D, Resources.Load<GameObject>("Panel - Menu")).AddComponent<MenuControl>();


        GameObject ui = NGUITools.AddChild(uiRoot, Resources.Load<GameObject>("Panel - UI"));
        uiControl = ui.AddComponent<UIControl>();
    }

    void LoadGame()
    {
        puzzleGame = Instantiate<GameObject>(Resources.Load("Puzzle") as GameObject).AddComponent<PuzzleGame>();
    }
}
