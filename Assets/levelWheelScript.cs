using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class levelWheelScript : MonoBehaviour
{
    public int maxIndex;

    public int currentIndex;

    public int targetIndex;

    private bool moving;

    public float moveDist;

    public AnimationCurve lerpCurve;

    public float moveTime;

    public List<Collider2D> colliders;

    public int currentLevel;

    public List<GameObject> disabledIndicators;

    public List<levelLoaderButton> buttons = new List<levelLoaderButton>();
    // Start is called before the first frame update
    void Start()
    {
        GetStatisticsLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetIndex != currentIndex && !moving)
        {
            StartCoroutine(MoveCoroutine(currentIndex, targetIndex));
            moving = true;
        }
    }

    IEnumerator MoveCoroutine(int startIndex, int endIndex)
    {
        float elapsed = 0.0f;
        for (var i = 0; i < colliders.Count; i++)
        {
            colliders[i].enabled = false;
        }
        Vector2 startPos = startIndex * new Vector2(-moveDist,0f);
        Vector2 endPos = endIndex * new Vector2(-moveDist, 0f);
        while (elapsed < moveTime)
        {
            float lerpVal = lerpCurve.Evaluate(elapsed / moveTime);
            transform.position = Vector2.Lerp(startPos, endPos, lerpVal);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;

        currentIndex = endIndex;
        if (currentIndex != targetIndex)
        {
            StartCoroutine(MoveCoroutine(currentIndex, targetIndex));
        }
        else
        {
            moving = false;
            for (var i = 0; i < colliders.Count; i++)
            {
                colliders[i].enabled = true;
            }
        }
    }
    
    void GetStatisticsLevel()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            GetStatsLevel,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void GetStatsLevel(GetPlayerStatisticsResult result)
    {
        foreach (var eachStat in result.Statistics)
        {
            if (eachStat.StatisticName == "level")
            {
                currentLevel = eachStat.Value;
                for (var i = currentLevel + 1; i < disabledIndicators.Count; i++)
                {
                    disabledIndicators[i].SetActive(true);
                    buttons[i].enabled = false;
                }
                break;
            }
        }
    }
}
