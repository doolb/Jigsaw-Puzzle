using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{

    GameObject finish;

    UILabel infoLabel;

    UILabel time;

    StatisticControl statistic;


    // Use this for initialization
    void Awake()
    {
        statistic = transform.Find("Panel - Statistic").gameObject.AddComponent<StatisticControl>();
        


        finish = transform.Find("Finish").gameObject;
        finish.SetActive(false);
        finish.GetComponent<TweenAlpha>().onFinished.Add(new EventDelegate(GameLoader.menuControl.Show));

        infoLabel = finish.transform.Find("Label - Info").GetComponent<UILabel>();

        time = transform.Find("Label - Time").GetComponent<UILabel>();


        transform.Find("Button - Menu").GetComponent<UIButton>().onClick.Add(new EventDelegate(GameLoader.menuControl.Show));

        GameLoader.puzzleGame.onGameEnd.Add(new EventDelegate(ShowFinish));
    }


    void FixedUpdate()
    {
        ShowTime(GameLoader.puzzleGame.gameTime);
    }


    public void ShowFinish()
    {
        finish.SetActive(true);
        finish.GetComponent<TweenAlpha>().ResetToBeginning();
        finish.GetComponent<TweenAlpha>().PlayForward();

        infoLabel.text = GameLoader.puzzleGame.records[GameLoader.puzzleGame.records.Count - 1].ToString();
    }

    public void ShowTime(float t = 0)
    {
        time.text = t.ToString("F1");
    }

}
