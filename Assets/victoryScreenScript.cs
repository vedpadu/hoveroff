using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class victoryScreenScript : MonoBehaviour
{
    public AnimationCurve growCurve;

    public float growDur;

    [HideInInspector] public float currentTime;

    public List<TextMeshPro> namesText;

    public List<TextMeshPro> timesText;

    public TextMeshPro textTimeCurrent;

    public TextMeshPro textTimeHighScore;

    [HideInInspector] public string statName;

    public float conversionFactor = 1000000f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Grow(growDur));
        textTimeCurrent.text = "Time:" + FormatTime(currentTime);
        GetStatisticBestTime();
    }

    void GetStatisticBestTime()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            CheckStatsInit,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void CheckStatsInit(GetPlayerStatisticsResult result)
    {

        bool contains = false;
        float highScore = currentTime;
        foreach (var eachStat in result.Statistics)
        {
            if (eachStat.StatisticName.Equals(statName))
            {
                contains = true;
                highScore = ((float)eachStat.Value/conversionFactor) * -1;
            }
        }

        if (!contains)
        {
            PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest {
                    // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
                    Statistics = new List<StatisticUpdate> {
                        new StatisticUpdate { StatisticName = statName, Value = (int)(currentTime * conversionFactor * -1) },
                    }
                },
                result1 => { Debug.Log("User statistics updated init highscore"); },
                error => { Debug.LogError(error.GenerateErrorReport()); });
        }
        else
        {
            if (currentTime < highScore)
            {
                highScore = currentTime;
                PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest {
                        // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
                        Statistics = new List<StatisticUpdate> {
                            new StatisticUpdate { StatisticName = statName, Value = (int)(currentTime * conversionFactor * -1) },
                        }
                    },
                    result1 => { Debug.Log("User statistics updated new highscore"); },
                    error => { Debug.LogError(error.GenerateErrorReport()); });
            }
        }

        textTimeHighScore.text = "Best:" + FormatTime(highScore);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Grow(float duration)
    {
        float elapsed = 0.0f;
        float currentScale = 0.0f;
        while (elapsed < duration)
        {
            float lerpVal = growCurve.Evaluate(elapsed / duration);
            currentScale = Mathf.Lerp(0f, 1f, lerpVal);
            transform.localScale = new Vector3(currentScale, currentScale,1f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1f,1f,1f);
    }
    
    string FormatTime (float time){
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float fraction = time * 1000;
        fraction = (fraction % 1000);
        string timeText = String.Format ("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
        return timeText;
    }
}
