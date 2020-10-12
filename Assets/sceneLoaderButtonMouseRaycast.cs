using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneLoaderButtonMouseRaycast : MonoBehaviour
{
    private Collider2D col;

    private bool mouseInside;
    
    public float scaleHovered;

    public float lerpValueButton;
    private float currentScale;

    private float initScale;

    public int scene;

    public int sceneTransition;

    private levelManagerScript levelLoader;
    // Start is called before the first frame update
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
        currentScale = transform.localScale.x;
        initScale = currentScale;
        levelLoader = GameObject.FindGameObjectWithTag("levelSelect").GetComponent<levelManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            if(hit.collider.Equals(col))
            {
                mouseInside = true;
            }
            else
            {
                mouseInside = false;
            }
        }
        else
        {
            mouseInside = false;
        }
        
        transform.localScale = new Vector3(currentScale, currentScale, 1f);
        if (mouseInside)
        {
            currentScale = Mathf.Lerp(currentScale, scaleHovered, lerpValueButton);
            if (Input.GetMouseButtonDown(0))
            {
                levelLoader.LoadNextLevel(scene, sceneTransition);
            }
        }
        else
        {
            currentScale = Mathf.Lerp(currentScale, initScale, lerpValueButton);
        }
        
    }
}
