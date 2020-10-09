using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxScriptX : MonoBehaviour
{
    public List<Transform> parallaxObjects;

    public List<float> parallaxXValues;

    private float mouseX;

    public float parallaxMultiplier;

    public float power;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        for (var i = 0; i < parallaxObjects.Count; i++)
        {
            float dist = parallaxXValues[i] - mouseX;
            
            parallaxObjects[i].position = new Vector2(Mathf.Pow(Mathf.Abs(dist),power) * Mathf.Sign(dist) * parallaxMultiplier, 0f);
        }
    }
}
