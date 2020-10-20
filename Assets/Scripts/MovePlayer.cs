using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float movementSpeed = 1f;
    Transform camTransform;
    Animator animator;
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        camTransform = Camera.main.transform;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");//-1 pentru tasta A, 1 pentru tasta D, 0 altfel
        float v = Input.GetAxis("Vertical"); //-1 pentru tasta S, 1 pentru tasta W, 0 altfel

        Vector3 moveDir = h * camTransform.right + v * camTransform.forward;
        moveDir.y = 0f;
        moveDir = moveDir.normalized;
        //transform.position += moveDir * Time.deltaTime * movementSpeed;
        ApplyRootRotation(moveDir);
        ComputeAnimatorParams(moveDir);
    }

    void ComputeAnimatorParams(Vector3 moveDir)
    {
        Vector3 moveDirCharacterSpace = transform.InverseTransformDirection(moveDir);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirCharacterSpace *= 0.5f;
        }
        animator.SetFloat("forward", moveDirCharacterSpace.z, 0.2f, Time.deltaTime);
        animator.SetFloat("right", moveDirCharacterSpace.x, 0.2f, Time.deltaTime);

        rigidbody.velocity = animator.deltaPosition / Time.deltaTime;
    }
    void ApplyRootRotation(Vector3 moveDir)
    {
        if (
            (transform.forward - moveDir).magnitude > 0.001f &&
            (transform.forward + moveDir).magnitude > 0.001f
           )
        {
            float theta = Mathf.Acos(Vector3.Dot(transform.forward, moveDir));
            theta *= Mathf.Rad2Deg;
            Vector3 axis = Vector3.Cross(transform.forward, moveDir);
            transform.rotation = Quaternion.AngleAxis(theta * .15f, axis) * transform.rotation;
        }
        if ((transform.forward + moveDir).magnitude < 0.001f)
        {
            transform.rotation = Quaternion.AngleAxis(2f, Vector3.up) * transform.rotation;
        }
    }
}
