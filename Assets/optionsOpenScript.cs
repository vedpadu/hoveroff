using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class optionsOpenScript : MonoBehaviour
{
    public optionsButtonScript optionsMenu;
    public leaderBoardMenuScript leaderboard;

    private Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (optionsMenu.menuOpen || leaderboard.menuOpen)
        {
            col.enabled = false;
        }
        else
        {
            col.enabled = true;
        }
    }
}
