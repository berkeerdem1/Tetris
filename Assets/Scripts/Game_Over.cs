using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Over : MonoBehaviour
{
    public static Game_Over Instance;
    public bool isClickedRetryButton, hasScaled = false;
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
        //Time.timeScale = 1;
        isClickedRetryButton = true;
    }
    public void ResetPos(Text score, Text time)
    {
        score.transform.position = startScorePos.transform.position;
        time.transform.position = startTimePos.transform.position;

        ScaleReduceObject(score.transform, time.transform, 1.5f);

        hasScaled = false;
    }
    public void ChangePos(Text score, Text time)
    {
        score.transform.position = scoreEndTextPos.transform.position;
        time.transform.position = timeTextEndPos.transform.position;

        if (!hasScaled)
        {
            ScaleGrowObject(score.transform, time.transform, 1.5f); // Sadece bir kez calistir
            hasScaled = true;
        }
    }
    public void ScaleGrowObject(Transform objTransform, Transform objTransform2, float scale)//Objeyi buyut
    {
        objTransform.localScale = objTransform.localScale * scale;
        objTransform2.localScale = objTransform2.localScale * scale;
    }
    public void ScaleReduceObject(Transform objTransform, Transform objTransform2, float scale)//Objeyi kucult
    {
        objTransform.localScale = objTransform.localScale / scale;
        objTransform2.localScale = objTransform2.localScale / scale;
    }
}
