using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteShadowsRealtime : MonoBehaviour
{
    public int shadowCount;
    private SpriteRenderer sR;
    public float shadowDist;

    private shadowManager _shadowManager;

    [HideInInspector]
    public Transform[] shadows;
    public Material shadowMat;

    public string shadowLayer;

    public int shadowLayerInt;
    // Start is called before the first frame update
    void Start()
    {
        shadows = new Transform[shadowCount];
        sR = GetComponent<SpriteRenderer>();
        _shadowManager = GameObject.FindGameObjectWithTag("shadowManager").GetComponent<shadowManager>();
        for (var i = 0; i < shadowCount; i++)
        {
           
            GameObject shadow = new GameObject();
            //shadow.layer = 16;
            shadows[i] = shadow.transform;
            shadows[i].parent = transform;
            shadows[i].position = transform.position;
            shadows[i].localScale = new Vector3(1f,1f,1f);
            SpriteRenderer sR1 = shadow.AddComponent<SpriteRenderer>();
            sR1.material = shadowMat;
            sR1.material.SetColor("_Color", _shadowManager.shadowColor);
            sR1.sprite = sR.sprite;
            sR1.sortingOrder = shadowLayerInt;
            sR1.sortingLayerName = shadowLayer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < shadowCount; i++)
        {
            Vector2 pos = (Vector2)transform.position + (_shadowManager.direction * (shadowDist * (i + 1)));
            shadows[i].position = new Vector3(pos.x, pos.y, transform.position.z + 0.01f);
            shadows[i].rotation = transform.rotation;
        }   
    }
}
