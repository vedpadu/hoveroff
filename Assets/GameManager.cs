using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // Start is called before the first frame update
    void Start()
    {
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

        if (gameObjectToVictoryWhenDead.Equals(null))
        {
            victory = true;
            timerText.gameObject.SetActive(false);
            victoryScreen.gameObject.SetActive(true);
            victoryScreen.currentTime = timer;
            victoryScreen.statName = statisticName;
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
