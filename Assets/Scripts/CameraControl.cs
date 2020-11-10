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
    public float minAimingPitch = -45f;
    public float maxAimingPitch = 45f;
    public Vector3 cameraOffset;
    public Vector3 aimingCameraOffset;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");

        if(animator.GetBool("aiming"))
            pitch = Mathf.Clamp(pitch, minAimingPitch, maxAimingPitch);
        else
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 camOffset = Vector3.Lerp(cameraOffset, aimingCameraOffset,
                                animator.GetLayerWeight(1));
        transform.position = player.position - transform.forward * distToTarget +
            transform.TransformVector(camOffset);
    }
}
