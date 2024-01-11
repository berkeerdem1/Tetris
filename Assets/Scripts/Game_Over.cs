using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Over : MonoBehaviour
{
    public static Game_Over Instance;
    public bool isClickedRetryButton = false;
    public Transform scoreEndTextPos, timeTextEndPos, startScorePos, startTimePos;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject);
    }
    public void Panel()
    {
        Time.timeScale = 1;
        isClickedRetryButton = true;
    }
    public void ResetPos(Text score, Text time)
    {
        score.transform.position = startScorePos.transform.position;
        time.transform.position = startTimePos.transform.position;
    }
    public void ChangePos(Text score, Text time)
    {
        score.transform.position = scoreEndTextPos.transform.position;
        time.transform.position = timeTextEndPos.transform.position;
    }
}
