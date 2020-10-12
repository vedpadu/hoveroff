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

    private float highScore;

    private string displayName;
    // Start is called before the first frame update
    void Start()
    {
       
        StartCoroutine(Grow(growDur));
        textTimeCurrent.text = "Time:" + FormatTime(currentTime);
        //starts chain of getting times
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest{PlayFabId = PlayerPrefs.GetString("PlayFabId")}, OnGetDisplayName, OnGetDisplayNameFail);
       
    }

    void OnGetDisplayName(GetPlayerProfileResult result)
    {
        displayName = result.PlayerProfile.DisplayName;
        GetStatisticBestTime();
    }

    void OnGetDisplayNameFail(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
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
        highScore = currentTime;
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
                OnUpdateStats,
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
                    OnUpdateStats,
                    error => { Debug.LogError(error.GenerateErrorReport()); });
            }
            else
            {
                GetLeaderBoard();
            }
        }

        textTimeHighScore.text = "Best:" + FormatTime(highScore);
    }

    void OnUpdateStats(UpdatePlayerStatisticsResult result)
    {
        print("stats Updated");
        StartCoroutine(GetLBCoroutine(0.7f));
    }

    IEnumerator GetLBCoroutine(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        GetLeaderBoard();
    }

    void GetLeaderBoard()
    {
        var requestLeaderboard = new GetLeaderboardRequest
            {StartPosition = 0, StatisticName = statName, MaxResultsCount = namesText.Count};
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderBoard, OnErrorLeaderBoard);
    }

    void OnGetLeaderBoard(GetLeaderboardResult result)
    {
        for (var i = 0; i < result.Leaderboard.Count; i++)
        {
            if (result.Leaderboard[i].DisplayName.Equals(displayName))
            {
                namesText[i].text = displayName;
                timesText[i].text = FormatTime(highScore);
            }
            else
            {
                namesText[i].text = result.Leaderboard[i].DisplayName;
                timesText[i].text = FormatTime(((float)result.Leaderboard[i].StatValue/conversionFactor) * -1);
            }
           
        }
    }

    void OnErrorLeaderBoard(PlayFabError error)
    {
        for (var i = 0; i < namesText.Count; i++)
        {
            namesText[i].text = "Error";
            timesText[i].text = "Error";
        }
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
