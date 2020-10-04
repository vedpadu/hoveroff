using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserScript : MonoBehaviour
{
    public int reflective;
    public int enemyLayer;
    public int enemyLayer2;
    public int enemyLayer3;
    public float damage;
    public LayerMask hittable;

    public int maxReflections;

    private List<Vector3> laserPoints = new List<Vector3>();
    private LineRenderer lR;

    public float maxLaserDist;

    public float currentLaserDist;

    public float laserDistChangeValue;

    private lineRendererShadows shadows;

    private healthScript shipHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        if (enemyLayer3 == 0)
        {
            enemyLayer3 = enemyLayer2;
        }
        shipHealth = transform.parent.GetComponent<healthScript>();
        lR = GetComponent<LineRenderer>();
        //shadows = GetComponent<lineRendererShadows>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (currentLaserDist < maxLaserDist)
        {
            currentLaserDist += laserDistChangeValue * Time.deltaTime;
        }
        else
        {
            currentLaserDist = maxLaserDist;
        }
        CalculateLaser(maxReflections, true, transform.position, Vector2.zero);
        if (!lR.enabled)
        {
            lR.enabled = true;
            /*for (var i = 0; i < shadows.shadows.Length;i++)
            {
                shadows.shadows[i].enabled = true;
            }*/
        }
        lR.positionCount = laserPoints.Count;
        lR.SetPositions(laserPoints.ToArray());
    }

    public void OnDisable()
    {
        if (lR != null)
        {
            lR.enabled = false;
            currentLaserDist = 0f;
        }
        
       /* for (var i = 0; i < shadows.shadows.Length;i++)
        {
            shadows.shadows[i].enabled = false;
        }*/
    }


    public void CalculateLaser(int iterations, bool original, Vector2 start, Vector2 dir)
    {
        if (iterations == 0)
        {
            return;
        }
        if (original)
        {
            laserPoints.Clear();
            laserPoints.Add((Vector2)transform.position);
            float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        RaycastHit2D ray = Physics2D.Raycast(start, dir, currentLaserDist, hittable);
        if (ray)
        {
            if (ray.collider.gameObject.layer == enemyLayer || ray.collider.gameObject.layer == enemyLayer2 || ray.collider.gameObject.layer == enemyLayer3)
            {
                healthScript otherHealth = ray.collider.gameObject.GetComponent<healthScript>();
                otherHealth.DecreaseHealth(damage * Time.deltaTime);
                if (otherHealth.health <= 0f)
                {
                    shipHealth.DecreaseHealth((-otherHealth.maxHealth * otherHealth.gameObject.GetComponent<commonEnemyBehaviour>().healthGiveBackAmount));
                }
                
                
            }
            if (ray.collider.gameObject.layer == reflective)
            {
                Vector2 newDir = Vector2.Reflect(dir, ray.normal);
                laserPoints.Add(ray.point);
                CalculateLaser(iterations - 1, false, ray.point + (newDir * 0.02f), newDir);
            }
            else
            {
                laserPoints.Add(ray.point + (dir * 0.05f));
                currentLaserDist = ray.distance;
                return;
            }
        }
        else
        {
            laserPoints.Add((Vector2)start + (dir * currentLaserDist));
            return;
        }
    }
}
