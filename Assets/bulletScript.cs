using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public float damage;

    public GameObject deathParts;

    public float shakeDur;

    public float shakeMag;

    private shipScript shaker;

    public float deathTime;

    private float deathTimer;

    [HideInInspector]public healthScript shooter;

    private bool hit;

    private cameraShake camShake;
    // Start is called before the first frame update
    void Start()
    {
        camShake = Camera.main.GetComponent<cameraShake>();
        // shaker = GameObject.FindGameObjectWithTag("Player").GetComponent<shipScript>();
    }

    // Update is called once per frame
    void Update()
    {
        deathTimer += Time.deltaTime;
        if (deathTimer >= deathTime)
        {
            Destroy(GameObject.Instantiate(deathParts, transform.position, transform.rotation), 2f);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<healthScript>() != null && !hit)
        {
            if (!other.GetComponent<healthScript>().Equals(shooter))
            {
                other.GetComponent<healthScript>().DecreaseHealth(damage);
                Destroy(GameObject.Instantiate(deathParts, transform.position, transform.rotation), 2f);
                camShake.shakes.Add(new Shake(shakeDur, shakeMag));
                Destroy(gameObject);
                hit = true;
            }
            
        }
        else
        {
            Destroy(GameObject.Instantiate(deathParts, transform.position, transform.rotation), 2f);
            Destroy(gameObject);
            hit = true;
        }
        
    }
}
