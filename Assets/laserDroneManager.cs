using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserDroneManager : MonoBehaviour
{
    public Transform droneChild;

    public Transform targetChild;

    private List<Transform> targets = new List<Transform>();

    private List<laserDroneScript> laserDrones = new List<laserDroneScript>();

    public int currentShape;

    public float rotationSpeed;

    public float rotationIncrease;

    public List<int> acceptableShape;

    public float shapeAmp;

    float currentHue;

    public float hueChangeValue;

    private bool oldHasReached;

    public float reformTime;

    private float reformTimer;

    public float timeBeforeLaserStart;

    private bool startLasers;

    private bool startLasersJustStarted;

    public int startStage2Count;

    private bool stage2;

    private Camera cam;
    bool secondStageJustStarted;

    public Transform player;

    public float secondStageTimeBeforeLaser;
    public float secondStageLaserTime;

    public float timeAfterLockOn;

    public float angularVelAcceleration;
    float currentAngVel;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        for (var i = 0; i < droneChild.childCount; i++)
        {
            laserDrones.Add(droneChild.GetChild(i).GetComponent<laserDroneScript>());
            targets.Add(targetChild.GetChild(i));
        }

        for (var i = 0; i < laserDrones.Count; i++)
        {
            laserDrones[i].target = targets[i];
        }
        CreateFormation(currentShape, 0f, shapeAmp);
    }

    void CreateFormation(int n, float rotOffset, float amp)
    {
        startLasers = false;
        for (var i = 0; i < laserDrones.Count; i++)
        {
            if (!laserDrones[i].Equals(null))
            {
                laserDrones[i].isSpinning = false;
                laserDrones[i].laser.gameObject.SetActive(false);
            }
            
        }
        Vector2[] points = new Vector2[laserDrones.Count];
        float[] angles = new float[laserDrones.Count];
        for(var i = 0;i < laserDrones.Count;i++)
        {
            float t = (2 * Mathf.PI / (float)laserDrones.Count) * (float)i;
            int side = Mathf.FloorToInt((float)((i + 1)/ (float)laserDrones.Count) * n);
            float t1 = (2 * Mathf.PI/n) * side + 0.001f;
            float t2 = (2 * Mathf.PI/n) * side - 0.001f;
            float distFromCenter = ShapeFunction(amp, n, t, rotOffset);
            float distt1 = ShapeFunction(amp, n, t1, rotOffset);
            float distt2 = ShapeFunction(amp, n, t2, rotOffset);
            if (distFromCenter > amp+1)
            {
                distFromCenter /= 2;
            }
            Vector2 dir = new Vector2(Mathf.Cos(t - rotOffset), Mathf.Sin(t - rotOffset));
            Vector2 dirt1 =  new Vector2(Mathf.Cos(t1 - rotOffset), Mathf.Sin(t1 - rotOffset));
            Vector2 dirt2 =  new Vector2(Mathf.Cos(t2 - rotOffset), Mathf.Sin(t2 - rotOffset));
            Vector2 deriv = (dirt2 * Mathf.Abs(distt2)) - (dirt1 * Mathf.Abs(distt1)).normalized;
            float angle = Mathf.Atan2(deriv.y, deriv.x) * Mathf.Rad2Deg;
            angles[i] = angle + 180f;
            points[i] = dir * Mathf.Abs(distFromCenter);
        }

        bool formationCreated = false;
        int[] pointPerTarget = new int[laserDrones.Count];
        List<int> openPoints = new List<int>();
        while (!formationCreated)
        {
            pointPerTarget = new int[laserDrones.Count];
            openPoints.Clear();
            for (var i = 0; i < laserDrones.Count; i++)
            {
                openPoints.Add(i);
            }
            for (var i = 0; i < laserDrones.Count; i++)
            {
                bool pointFound = false;
                List<int> potentialPoints = new List<int>();
                for (var j = 0; j < openPoints.Count; j++)
                {
                    potentialPoints.Add(openPoints[j]);
                }

                while (potentialPoints.Count > 0)
                {
                    int indexRand = Random.Range(0, potentialPoints.Count - 1);
                    if (Vector2.Distance(points[potentialPoints[indexRand]], targets[i].position) > laserDrones[i].slowDownDistance - 1f)
                    {
                        pointPerTarget[i] = potentialPoints[indexRand];
                        pointFound = true;
                        openPoints.Remove(potentialPoints[indexRand]);
                        break;
                    }
                    else
                    {
                        potentialPoints.Remove(potentialPoints[indexRand]);
                    }
                }

                if (potentialPoints.Count == 0 && !pointFound)
                {
                    break;
                }

                if (i == laserDrones.Count - 1)
                {
                    formationCreated = true;
                }
            }
        }

        for (var i = 0; i < pointPerTarget.Length; i++)
        {
            int ind = pointPerTarget[i];
            targets[i].position = points[ind];
            targets[i].rotation = Quaternion.Euler(0f,0f,angles[ind]);
        }
        
    }

    //RADIANS
    float ShapeFunction(float amplitude, int n, float t, float rotOffset)
    {
        return (amplitude * Mathf.Cos(Mathf.PI / n)) /
            Mathf.Cos((t - rotOffset) - ((2 * Mathf.PI / n) * Mathf.Floor((n * (t - rotOffset) + Mathf.PI) / (Mathf.PI * 2))));
    }

    float SideSlopeFunction(float amp, int n, float tOffset, float rotOffset, int i)
    {
        int side = Mathf.FloorToInt((float) i / (float) laserDrones.Count * (float)n);
        float returnVal =  (amp * Mathf.Cos(Mathf.PI / n)) / Mathf.Cos((((side * (2 * Mathf.PI/n))) - rotOffset) - (side * ((2 * Mathf.PI)/n)));
        return returnVal;
    }

    // Update is called once per frame
    void Update()
    {
        bool hasAllReached = true;
        int deadCount = 0;
        for (var i = 0; i < laserDrones.Count; i++)
        {
            if (!laserDrones[i].Equals(null))
            {
                laserDrones[i].hue = currentHue;
                if (!laserDrones[i].reachedTarget)
                {
                    hasAllReached = false;
                    break;
                }
            }
            else
            {
                deadCount += 1;
            }
           
        }

        if (deadCount == laserDrones.Count)
        {
            Destroy(gameObject);
        }

        if (laserDrones.Count - deadCount <= startStage2Count && !stage2)
        {
            stage2 = true;
            secondStageJustStarted = true;
        }

        if (!stage2)
        {
            if (hasAllReached)
            {

                if (!oldHasReached == hasAllReached)
                {
                    for (var i = 0; i < laserDrones.Count; i++)
                    {
                        if (!laserDrones[i].Equals(null))
                        {
                            laserDrones[i].preLaser.SetActive(true);
                        }
                    
                    }
                    StartCoroutine(startLaserPhase(timeBeforeLaserStart));
                }

                if (startLasers)
                {
                    if (startLasersJustStarted)
                    {
                        currentAngVel = 0f;
                        for (var i = 0; i < laserDrones.Count; i++)
                        {
                            if (!laserDrones[i].Equals(null))
                            {
                                laserDrones[i].isSpinning = true;
                                laserDrones[i].preLaser.SetActive(false);
                                laserDrones[i].laser.gameObject.SetActive(true);
                            }
                        
                        }

                        startLasersJustStarted = false;
                    }
                    currentHue += hueChangeValue * Time.deltaTime;
                    if (currentHue > 1f)
                    {
                        currentHue -= 1f;
                    }

                    float targetAngVel = (rotationSpeed + (rotationIncrease * deadCount));
                    if (Mathf.Abs(currentAngVel) < Mathf.Abs(targetAngVel))
                    {
                        currentAngVel += Mathf.Sign(targetAngVel - currentAngVel) * angularVelAcceleration * Time.deltaTime;
                    }
                   
                    transform.Rotate(0f, 0f, currentAngVel * Time.deltaTime);
                }
           
            } 
        }
        else
        {

            if (secondStageJustStarted)
            {
                for (var i = 0; i < laserDrones.Count; i++)
                {
                    if (!laserDrones[i].Equals(null))
                    {
                        laserDrones[i].gameObject.GetComponent<commonEnemyBehaviour>().healthGiveBackAmount =
                            0.1f;
                        laserDrones[i].isSpinning = false;
                        laserDrones[i].stage2 = true;
                        laserDrones[i].reachedTarget = false;
                        laserDrones[i].target.position = new Vector2(Random.Range(-cam.orthographicSize * (Screen.width/(float)Screen.height), cam.orthographicSize * (Screen.width/(float)Screen.height)), Random.Range(-cam.orthographicSize, cam.orthographicSize));
                    }
                }

                secondStageJustStarted = false;
            }

            for (var i = 0; i < laserDrones.Count; i++)
            {
                if (!laserDrones[i].Equals(null))
                {
                    if (laserDrones[i].DoTargetting)
                    {
                        Vector2 dir = (player.position - targets[i].position).normalized;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        laserDrones[i].target.rotation = Quaternion.Euler(0f,0f, angle);
                    }

                    if (laserDrones[i].reachedTarget && !laserDrones[i].secondStageSwitchTarget && !laserDrones[i].secondStageInCoroutine)
                    {
                        StartCoroutine(secondPhaseShootingCoroutine(secondStageTimeBeforeLaser, secondStageLaserTime, timeAfterLockOn,
                            laserDrones[i]));
                    }

                    if (!laserDrones[i].reachedTarget)
                    {
                        laserDrones[i].laser.gameObject.SetActive(false);
                        laserDrones[i].preLaser.SetActive(false);
                    }
                }
            }

        }


        if (!stage2)
        {
            reformTimer += Time.deltaTime;
            if (reformTimer >= reformTime)
            {
                reformTimer = 0f;
                int randInd = Random.Range(0, acceptableShape.Count - 1);
                currentShape = acceptableShape[randInd];
                CreateFormation(currentShape, 0f, shapeAmp);
            }

        }
       
        oldHasReached = hasAllReached;
    }

    IEnumerator startLaserPhase(float timeBeforeLaser)
    {
        float elapsed = 0.0f;
        while (elapsed < timeBeforeLaser)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        startLasers = true;
        startLasersJustStarted = true;
    }

    IEnumerator secondPhaseShootingCoroutine(float timeBeforeLaser, float timeLaser, float timeAfterLock, laserDroneScript laser)
    {
        if (!laser.Equals(null))
        {
            laser.preLaser.SetActive(true);
            laser.secondStageInCoroutine = true;
            laser.secondStageSwitchTarget = false;
            laser.DoTargetting = true;
        }

        float elapsed = 0.0f;
        yield return new WaitForSeconds(timeBeforeLaser);

        if (!laser.Equals(null))
        {
            laser.DoTargetting = false;
        }
        
        yield return new WaitForSeconds(timeAfterLockOn);

        if (!laser.Equals(null))
        {
            laser.laser.gameObject.SetActive(true);
            laser.preLaser.SetActive(false);
        }
        yield return new WaitForSeconds(timeLaser);

        if (!laser.Equals(null))
        {
            laser.laser.gameObject.SetActive(false);
            laser.target.position = new Vector2(Random.Range(-cam.orthographicSize * (Screen.width/(float)Screen.height), cam.orthographicSize * (Screen.width/(float)Screen.height)), Random.Range(-cam.orthographicSize, cam.orthographicSize));
            laser.reachedTarget = false;
            laser.secondStageInCoroutine = false;
        }
       
    }
}
