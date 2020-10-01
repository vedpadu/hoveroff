using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockRotationChildScript : MonoBehaviour
{
    public float rotationZ;

    public Vector2 position;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0f,0f,rotationZ);
        transform.position = transform.parent.position + (Vector3)position;
    }
}
