using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splitBulletScript : MonoBehaviour
{
    public GameObject bulletToSplitTo;

    public int splitCount;

    public float splitTime;

    private float splitTimer;

    public float bulletForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        splitTimer += Time.deltaTime;
        if (splitTimer >= splitTime)
        {
            float angleInterval = 360f / splitCount;
            for (var i = 0; i < splitCount; i++)
            {
                float currAngle = angleInterval * i;
                currAngle *= Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
                Vector2 instantiationPos = transform.position;
                GameObject bulletObj = GameObject.Instantiate(bulletToSplitTo, instantiationPos, Quaternion.Euler(0f, 0f, currAngle * Mathf.Rad2Deg));
                bulletObj.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(bulletForce,0f));
            }
            Destroy(gameObject);
        }
    }
}
