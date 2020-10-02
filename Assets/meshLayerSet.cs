using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshLayerSet : MonoBehaviour
{
    public string layerName;

    public int layerInt;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().sortingLayerName = layerName;
        GetComponent<MeshRenderer>().sortingOrder = layerInt;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
