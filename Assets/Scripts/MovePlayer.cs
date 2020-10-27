using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float rotSpeed = 10f;
    public float jumpPower = 5f;
    Transform camTransform;
    Animator animator;
    Rigidbody rigidbody;
    Vector3 moveDir;
    public float groundedRaycastThreshold = 0.01f; 
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
        ComputeMoveDirection();
        ApplyRootRotation();
        ApplyRootMovement();
        ComputeAnimatorParams();
        HandleJump();
        HandleAttack();
    }
    private void HandleAttack()
    {
        if(Input.GetButtonDown("Fire1"))
            animator.SetTrigger("attack");
    }
    private void ComputeMoveDirection()
    {
        float h = Input.GetAxis("Horizontal");//-1 pentru tasta A, 1 pentru tasta D, 0 altfel
        float v = Input.GetAxis("Vertical"); //-1 pentru tasta S, 1 pentru tasta W, 0 altfel

        moveDir = h * camTransform.right + v * camTransform.forward;
        moveDir.y = 0f;
        moveDir = moveDir.normalized;
    }

    private void HandleJump()
    {
        Ray ray = new Ray(transform.position + Vector3.up * groundedRaycastThreshold, Vector3.down);
        if (Physics.Raycast(ray, 2 * groundedRaycastThreshold))
        {
            animator.SetBool("midair", false);
            animator.applyRootMotion = true;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpForce = (Vector3.up + moveDir) * jumpPower;
                rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
                animator.SetBool("midair", true);
            }
        }
        else
        {
            animator.applyRootMotion = false;
            animator.SetBool("midair", true);
        }
    }
    void ComputeAnimatorParams( )
    {
        Vector3 moveDirCharacterSpace = transform.InverseTransformDirection(moveDir);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirCharacterSpace *= 0.5f;
        }
        animator.SetFloat("forward", moveDirCharacterSpace.z, 0.2f, Time.deltaTime);
        animator.SetFloat("right", moveDirCharacterSpace.x, 0.2f, Time.deltaTime);
    }

    private void ApplyRootMovement()
    {
        if (animator.GetBool("midair"))
            return;
        //transform.position += moveDir * Time.deltaTime * movementSpeed; //hardcoded movement
        float velY = rigidbody.velocity.y;
        rigidbody.velocity = animator.deltaPosition / Time.deltaTime;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, velY, rigidbody.velocity.z);
    }

    void ApplyRootRotation( )
    {
        if (animator.GetBool("midair"))
            return;

        if (
            (transform.forward - moveDir).magnitude > 0.001f &&
            (transform.forward + moveDir).magnitude > 0.001f
           )
        {
            float theta = Mathf.Acos(Vector3.Dot(transform.forward, moveDir));
            theta *= Mathf.Rad2Deg;
            Vector3 axis = Vector3.Cross(transform.forward, moveDir);
            transform.rotation = Quaternion.AngleAxis(theta * Time.deltaTime * rotSpeed, axis) * transform.rotation;
        }
        if ((transform.forward + moveDir).magnitude < 0.001f)
        {
            transform.rotation = Quaternion.AngleAxis(2f, Vector3.up) * transform.rotation;
        }
    }
}
