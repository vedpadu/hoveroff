using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hiveShockwaveScript : MonoBehaviour
{
    private float scale;

    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scale += speed * Time.deltaTime;
        transform.localScale = new Vector3(scale,scale,1f);
    }
}
