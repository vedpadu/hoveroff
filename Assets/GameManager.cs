using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();

    private GameObject player;

    private bool init;

    private bool hasStarted;

    private levelManagerScript lMS;

    public GameObject promptUI;

    public TextMeshProUGUI timerText;

    private float timer;
    public GameObject gameObjectToVictoryWhenDead;

    private bool victory;

    public string statisticName;

    public victoryScreenScript victoryScreen;

    public int levelNumber;

    private int currentLevel;

    public GameObject pauseMenu;

    private pauseMenuScript pauseMenScript;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenScript = pauseMenu.GetComponent<pauseMenuScript>();
        lMS = GameObject.FindGameObjectWithTag("levelSelect").GetComponent<levelManagerScript>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        player = GameObject.FindGameObjectWithTag("Player");
        if (PlayerPrefs.GetInt("startGameImmediate") == 1)
        {
            promptUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            for (var i = 0; i < enemies.Count; i++)
            {
                enemies[i].SetActive(false);
            }
            player.SetActive(false);
            init = true;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Destroy(gameObjectToVictoryWhenDead);
            timer = 50f;
        }

        if (!victory)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!pauseMenu.activeSelf)
                {
                    Time.timeScale = 0f;
                    pauseMenu.transform.localScale = new Vector3(0f,0f,1f);
                    pauseMenu.SetActive(true);
                    pauseMenScript.StartGrow();
                }
            }
        }
        
        if (gameObjectToVictoryWhenDead.Equals(null))
        {
            pauseMenu.SetActive(false);
            timerText.gameObject.SetActive(false);
            victory = true;
            timerText.gameObject.SetActive(false);
            victoryScreen.gameObject.SetActive(true);
            victoryScreen.currentTime = timer;
            victoryScreen.statName = statisticName;
            if (currentLevel == levelNumber)
            {
                currentLevel += 1;
                UpdatePlayerLevel();
            }
        }

        if (Input.anyKeyDown && !hasStarted)
        {
            PlayerPrefs.SetInt("startGameImmediate", 1);
        }

        if (PlayerPrefs.GetInt("startGameImmediate") == 1 && hasStarted == false)
        {
            hasStarted = true;
            player.SetActive(true);
            for (var i = 0; i < enemies.Count; i++)
            {
                enemies[i].SetActive(true);
            }
            promptUI.SetActive(false);
        }

        if (hasStarted && !victory)
        {
            timer += Time.deltaTime;
        }

        timerText.text = FormatTime(timer);

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.SetInt("startGameImmediate", 1);
            lMS.LoadNextLevel(SceneManager.GetActiveScene().buildIndex, 0);
            
        }
    }

    void UpdatePlayerLevel()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
                // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
                Statistics = new List<StatisticUpdate> {
                    new StatisticUpdate { StatisticName = "level", Value = currentLevel },
                }
            },
            result1 => { Debug.Log("User statistics updated"); },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    void GetStatsLevel()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatsLevel,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatsLevel(GetPlayerStatisticsResult result)
    {
        foreach (var eachStat in result.Statistics)
        {
            if (eachStat.StatisticName.Equals("level"))
            {
                currentLevel = eachStat.Value;
            }
        }
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
