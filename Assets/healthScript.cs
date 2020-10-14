using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthScript : MonoBehaviour
{
    public GameObject deathEffect;
    [HideInInspector]public float health;

    public float maxHealth;

    public Transform HealthBarTransform;

    public List<SpriteRenderer> HealthBarRenderers = new List<SpriteRenderer>();

    private float timeSinceDamaged;

    public float barAnimationTime;

    public AnimationCurve barAnimationOpacity;
    
    public float deathShakeDur;
    
    public float deathShakeMag;

    private cameraShake camerShake;
    
    public bool destroyThisObject = true;

    public GameObject gameObjectToDestroy;

    public bool shakeFadeAfterDeath;

    public bool actuallyKill = true;

    public GameObject instantiateOnDeath;
    // Start is called before the first frame update
    void Start()
    {
        camerShake = Camera.main.GetComponent<cameraShake>();
        health = maxHealth;
        timeSinceDamaged = barAnimationTime + 1f;
    }

    // Update is called once per frame
    void Update()
    {
       
            if (health <= 0f)
            {
                if (actuallyKill)
                {
                    if (!instantiateOnDeath.Equals(null))
                    {
                        Destroy(GameObject.Instantiate(instantiateOnDeath, transform.position, transform.rotation),1.5f);
                    }
                    Destroy(GameObject.Instantiate(deathEffect, transform.position, transform.rotation), 3f);
                    if (shakeFadeAfterDeath)
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<shipScript>().StopShakeCamOverTime(deathShakeDur, deathShakeMag);
                    }
                    else
                    {
                        camerShake.shakes.Add(new Shake(deathShakeDur, deathShakeMag));
                    }
            
            
                    if (destroyThisObject)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        Destroy(gameObjectToDestroy);
                    }
                }
                else
                {
                    health = 0f;
                }
                
           
            }else if (health > maxHealth)
            {
                health = maxHealth;
            }

            HealthBarTransform.transform.localScale = new Vector3(Mathf.Clamp(health/maxHealth,0f, 1f), 1f,1f);

        if (timeSinceDamaged <= barAnimationTime)
        {
            float fraction = timeSinceDamaged / barAnimationTime;
            float opacity = barAnimationOpacity.Evaluate(fraction);
            foreach (SpriteRenderer healthBarRenderer in HealthBarRenderers)
            {
                healthBarRenderer.color = new Color(healthBarRenderer.color.r, healthBarRenderer.color.g, healthBarRenderer.color.b, opacity);
            }
            timeSinceDamaged += Time.deltaTime;
        }
        else
        {
            foreach (SpriteRenderer healthBarRenderer in HealthBarRenderers)
            {
                healthBarRenderer.color = new Color(healthBarRenderer.color.r, healthBarRenderer.color.g, healthBarRenderer.color.b, 0f);
            }
        }
    }

    public void DecreaseHealth(float amount)
    {
        health -= amount;
        if (!Mathf.Approximately(amount, 0f))
        {
            timeSinceDamaged = 0f;
        }
        
    }
    
    
}
