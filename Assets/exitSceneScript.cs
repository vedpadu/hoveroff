using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitSceneScript : MonoBehaviour
{
    public int sceneToExitTo;
    public int sceneFadeIndex;

    private levelManagerScript lMS;
    // Start is called before the first frame update
    void Start()
    {
        lMS = GameObject.FindGameObjectWithTag("levelSelect").GetComponent<levelManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lMS.LoadNextLevel(sceneToExitTo, sceneFadeIndex);
        }
    }
}
