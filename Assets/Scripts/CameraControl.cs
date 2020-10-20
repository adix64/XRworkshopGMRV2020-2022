using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    float yaw = 0f, pitch = 0f;
    public Transform player;
    public float distToTarget = 4f;
    public float minPitch = -45f;
    public float maxPitch = 45f;
    public Vector3 cameraOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = player.position - transform.forward * distToTarget +
            transform.TransformVector(cameraOffset);
    }
}
