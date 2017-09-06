using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticControl : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {
        transform.Find("Button - Close").GetComponent<UIButton>().onClick.Add(new EventDelegate(Hide));
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        GameLoader.menuControl.Show();
    }
}
