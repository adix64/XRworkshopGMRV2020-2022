using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    MeshRenderer meshRenderer;
    Transform cameraTrasnform;
    public float power = 4f;
    // Start is called before the first frame update
    void Start()
    {
        cameraTrasnform = Camera.main.transform;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraToBox = transform.position - cameraTrasnform.position;
        float dotProd = Vector3.Dot(cameraTrasnform.forward, cameraToBox.normalized);
        float blendFactor = dotProd * 0.5f + 0.5f;
        blendFactor = Mathf.Pow(blendFactor,power);
        meshRenderer.material.color = Color.Lerp(Color.grey, Color.yellow, blendFactor);
    }
}
