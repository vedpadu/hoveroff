using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class commonEnemyBehaviour : MonoBehaviour
{
    public float enemyAvoidanceDistance;

    public AnimationCurve enemyAvoidanceForceFalloff;

    public LayerMask enemyLayerMask;

    private Rigidbody2D rb;

    public healthScript hS;

    public float deathShakeDur;

    public float deathShakeMag;

    public float avoidanceLerp;

    public float hitEnemyDamage = 100f;

    public float hitPlayerDamage = 100f;

    public float hitDamageToPlayer = 75f;

    private bool hit;

    public bool healthScriptOnObject = true;

    private float hitTime = 0.5f;

    private float hitTimer;

    public float avoidanceLerpStartAfterTime = 0f;

    private float avoidanceTimer;

    [Range(0f,1f)]
    public float healthGiveBackAmount = 1f / 5f;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Rigidbody2D>() != null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        if (healthScriptOnObject)
        {
            hS = GetComponent<healthScript>();
        }
        
        hS.deathShakeDur = deathShakeDur;
        hS.deathShakeMag = deathShakeMag;
    }

    // Update is called once per frame
    void Update()
    {
        avoidanceTimer += Time.deltaTime;
        if (hit)
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= hitTime)
            {
                hitTimer = 0f;
                hit = false;
            }
        }

        if (avoidanceTimer >= avoidanceLerpStartAfterTime)
        {
            Collider2D[] enemiesInRange =
                Physics2D.OverlapCircleAll(transform.position, enemyAvoidanceDistance, enemyLayerMask);
            List<Rigidbody2D> enemiesInRangeRBs = new List<Rigidbody2D>();
            for (var i = 0; i < enemiesInRange.Length; i++)
            {
                if (!enemiesInRangeRBs.Contains(enemiesInRange[i].attachedRigidbody))
                {
                    enemiesInRangeRBs.Add(enemiesInRange[i].attachedRigidbody);
                }
            }

            for (var i = 0; i < enemiesInRangeRBs.Count; i++)
            {
                if (!enemiesInRangeRBs[i].Equals(rb))
                {
                    Rigidbody2D otherRb = enemiesInRangeRBs[i];
                    Vector2 dir = -(otherRb.position - rb.position);
                    float dist = dir.magnitude;
                    float forceMag = enemyAvoidanceForceFalloff.Evaluate(dist / enemyAvoidanceDistance);
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, avoidanceLerp * forceMag * Time.deltaTime));
                }
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hit)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                hS.DecreaseHealth(hitPlayerDamage);
                hit = true;
                other.gameObject.GetComponent<healthScript>().DecreaseHealth(hitDamageToPlayer);
            }else if (other.gameObject.CompareTag("Enemy"))
            {
                hit = true;
                hS.DecreaseHealth(hitEnemyDamage);
            }
        }

        if (other.gameObject.CompareTag("hiveShockwave"))
        {
            hS.DecreaseHealth(1000000000000000f);
        }
       
    }
}
