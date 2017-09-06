using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    UIPlayAnimation playAnim;
    bool show = true;

    UILabel startLabel;

    // Use this for initialization
    void Awake()
    {
        playAnim = gameObject.AddComponent<UIPlayAnimation>();
        playAnim.target = GetComponent<Animation>();


        RegistryCallback();

        GameLoader.puzzleGame.onGameEnd.Add(new EventDelegate(GameEnd));
    }

    public void Show()
    {
        if (show) return;
        show = true;

        playAnim.Play(false);

        GameLoader.puzzleGame.Pause();
    }

    public void Hide()
    {
        if (!show) return;
        show = false;

        playAnim.Play(true);

        GameLoader.puzzleGame.Continue();
    }

    void GameEnd()
    {
        UpdateButton();
    }

    #region ngui callback

    void RegistryCallback()
    {
        Transform startButton = transform.Find("Button - Start");
        startButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(StartGame));
        startLabel = startButton.Find("Label").GetComponent<UILabel>();

        transform.Find("Pop Up List - Background").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeBackground));

        transform.Find("Check Box - Show Original Image").GetComponent<UIToggle>().onChange.Add(new EventDelegate(ToggleImage));

        transform.Find("Check Box - Rotate Piece").GetComponent<UIToggle>().onChange.Add(new EventDelegate(ToggleRotate));

        transform.Find("Pop Up List -  Piece Count").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeCount));

        transform.Find("Pop Up List - Piece Image").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeImage));

        transform.Find("Pop Up List - Piece Shape").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeShape));

        transform.Find("Pop Up List - Piece Style").GetComponent<UIPopupList>().onChange.Add(new EventDelegate(ChangeStyle));

        transform.Find("Toggle Button - Show All").GetComponent<UIToggle>().onChange.Add(new EventDelegate(ToggleShow));

        transform.Find("Button - Tile Piece").GetComponent<UIButton>().onClick.Add(new EventDelegate(TilePiece));
    }


    void UpdateButton()
    {
        if (GameLoader.puzzleGame.needRestart ||
            !GameLoader.puzzleGame.pieceCreated)
        {
            startLabel.text = "开始";
        }
        else
            startLabel.text = "继续";
    }

    void StartGame()
    {
        Hide();

        GameLoader.puzzleGame.StartGame();

        UpdateButton();
    }

    void ChangeBackground()
    {
        GameLoader.background.ChangeBackground(UIPopupList.current.value);
    }

    void ChangeCount()
    {
        GameLoader.puzzleGame.SetPieceCount(GetPieceCount(int.Parse(UIPopupList.current.value.Trim())));

        UpdateButton();
    }

    Vector2 GetPieceCount(int count)
    {
        int x = 6, y = 4;
        switch (count)
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

    void ChangeImage()
    {
        GameLoader.puzzleGame.SetPieceImage(UIPopupList.current.value);
    }

    void ChangeShape()
    {
        GameLoader.puzzleGame.SetPieceShape(UIPopupList.current.value);
    }



    void ChangeStyle()
    {
        GameLoader.puzzleGame.SetPieceStyle(UIPopupList.current.value);

    }

    void ToggleShow()
    {
        GameLoader.puzzleGame.ShowAllOrNot(UIToggle.current.value);
    }

    void TilePiece()
    {
        GameLoader.puzzleGame.TilePiece();
    }

    void ToggleImage()
    {
        GameLoader.puzzleGame.ToggleImage(UIToggle.current.value);
    }

    void ToggleRotate()
    {
        GameLoader.puzzleGame.ToggleRotate(UIToggle.current.value);

        UpdateButton();
    }

    #endregion


}
