using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float movementSpeed = 1f;
    Transform camTransform;
    // Start is called before the first frame update
    void Start()
    {
        camTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");//-1 pentru tasta A, 1 pentru tasta D, 0 altfel
        float v = Input.GetAxis("Vertical"); //-1 pentru tasta S, 1 pentru tasta W, 0 altfel

        Vector3 moveDir = h * camTransform.right + v * camTransform.forward;
        moveDir.y = 0f;
        moveDir = moveDir.normalized;
        transform.position += moveDir * Time.deltaTime * movementSpeed;
    }
}
