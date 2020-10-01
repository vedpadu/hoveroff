using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineRendererShadows : MonoBehaviour
{
    public int shadowCount;

    public float shadowDist;

    private shadowManager _shadowManager;

    [HideInInspector]
    public LineRenderer[] shadows;

    private LineRenderer lR;
    public GameObject shadowPref;

    public Material shadowMat;

    private bool hasInitialized = false;

    public string shadowLayer;

    public int shadowSortingOrder;
    // Start is called before the first frame update
    void Start()
    {
        lR = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!hasInitialized)
        {
            Material lRMat = lR.material;

            _shadowManager = GameObject.FindGameObjectWithTag("shadowManager").GetComponent<shadowManager>();
            shadows = new LineRenderer[shadowCount];
            for (var i = 0; i < shadowCount; i++)
            {
                GameObject gO = GameObject.Instantiate(shadowPref, transform.position, transform.rotation, transform);
                shadows[i] = gO.GetComponent<LineRenderer>();
                shadows[i].widthCurve = lR.widthCurve;
                shadows[i].startColor = _shadowManager.shadowColor;
                shadows[i].endColor = _shadowManager.shadowColor;
                shadows[i].material = shadowMat;
                shadows[i].material.SetColor("_Color", _shadowManager.shadowColor);
                shadows[i].material.SetTexture("_MainTex", lRMat.GetTexture("_MainTex"));
                /*shadows[i].material.SetVector("_Offset", lRMat.GetVector("_Offset"));
                shadows[i].material.SetVector("_Tiling", lRMat.GetVector("_Tiling"));*/
                shadows[i].sortingLayerName = shadowLayer;
                shadows[i].sortingOrder = shadowSortingOrder;
            }
            hasInitialized = true;
        }
        Vector3[] positions = new Vector3[lR.positionCount];
        lR.GetPositions(positions);
        for (var i = 0; i < shadows.Length; i++)
        {
            shadows[i].widthCurve = lR.widthCurve;
            shadows[i].positionCount = lR.positionCount;
            for (var j = 0; j < positions.Length; j++)
            {
                positions[j] += (Vector3)(_shadowManager.direction * (shadowDist * (i + 1)));
            }
            shadows[i].SetPositions(positions);
            //shadows[i].transform.position = new Vector3(pos.x, pos.y, transform.position.z + 0.01f);
        }
    }
}
