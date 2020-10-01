using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();

    private GameObject player;

    private bool init;
    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        player = GameObject.FindGameObjectWithTag("Player");
        
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.SetActive(true);
            for (var i = 0; i < enemies.Count; i++)
            {
                enemies[i].SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
}
