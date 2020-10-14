using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class leaderBoardMenuScript : MonoBehaviour
{
    public List<string> options;

    public List<string> optionKeys;

    public TMP_Dropdown dropdown;

    private bool mouseInside;

    public float hoveredScale;

    private float initScale;
    float currentScale;

    public float lerpVal;

    public CanvasGroup cGroup;

    public float showGroupTime;

    public bool menuOpen;

    public List<TextMeshProUGUI> namesText;

    public List<TextMeshProUGUI> timesText;

    public TextMeshProUGUI bestTime;
    float conversionFactor = 1000000f;
    // Start is called before the first frame update
    void Start()
    {
        dropdown.ClearOptions();

        dropdown.AddOptions(options);
        dropdown.value = 0;
        dropdown.RefreshShownValue();
        currentScale = transform.localScale.x;
        initScale = currentScale;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (menuOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(CanvasDisappear(showGroupTime));
                menuOpen = false;
            }
        }
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
        if (mouseInside)
        {
            currentScale = Mathf.Lerp(currentScale, hoveredScale, lerpVal);
            if (Input.GetMouseButtonDown(0) && !menuOpen)
            {
                StartCoroutine(CanvasShow(showGroupTime));
                RefreshLeaderboard();
                menuOpen = true;
            }
        }else{
            currentScale = Mathf.Lerp(currentScale, initScale, lerpVal);
        }
    }

    private void OnMouseEnter()
    {
        mouseInside = true;
    }

    private void OnMouseExit()
    {
        mouseInside = false;
    }

    public void RefreshLeaderboard()
    {
        string statName = optionKeys[dropdown.value];
        GetStatisticBestTime();
        var requestLeaderboard = new GetLeaderboardRequest
            {StartPosition = 0, StatisticName = statName, MaxResultsCount = namesText.Count};
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderBoard, OnErrorLeaderBoard);
        
    }

    void OnGetLeaderBoard(GetLeaderboardResult result)
    {
        for (var i = 0; i < namesText.Count; i++)
        {
            if (result.Leaderboard.Count > i)
            {
                namesText[i].text = result.Leaderboard[i].DisplayName;
                timesText[i].text = FormatTime(((float)result.Leaderboard[i].StatValue/conversionFactor) * -1);
            }
            else
            {
                namesText[i].text = "----------------";
                timesText[i].text = "----------------";
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
    
    void GetStatisticBestTime()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            CheckStatsHighScore,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void CheckStatsHighScore(GetPlayerStatisticsResult result)
    {
        bool contains = false;
        string statName = optionKeys[dropdown.value];
        float highScore = 10000000000000f;
        foreach (var eachStat in result.Statistics)
        {
            if (eachStat.StatisticName.Equals(statName))
            {
                contains = true;
                highScore = ((float)eachStat.Value/conversionFactor) * -1;
                break;
            }
        }

        if (contains)
        {
            bestTime.text = "Time:" + FormatTime(highScore);
        }

       
    }
    
    IEnumerator CanvasShow(float duration)
    {
        float elapsed = 0.0f;
        cGroup.gameObject.SetActive(true);
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            cGroup.alpha = alpha;
            elapsed += Time.deltaTime;
            yield return null;
        }

        cGroup.alpha = 1f;
    }

    IEnumerator CanvasDisappear(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            cGroup.alpha = alpha;
            elapsed += Time.deltaTime;
            yield return null;
        }

        cGroup.alpha = 0f;
        cGroup.gameObject.SetActive(false);
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
