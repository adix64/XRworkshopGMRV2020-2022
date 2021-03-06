﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float rotSpeed = 10f;
    public float jumpPower = 5f;
    public float weaponToggleAnimSpeed = 10f;
    public float aimingResponsiveness = 10f;
    Transform camTransform;
    Transform rightHand;
    Transform chestBone;
    public Transform weaponHandle;
    public Transform projectilePrefab;
    Transform weapon;
    Animator animator;
    Rigidbody rigidbody;
    Vector3 moveDir;
    public float groundedRaycastThreshold = 0.01f;
    public Transform enemiesContainer;
    List<Transform> enemies;
    // Start is called before the first frame update
    void Start()
    {
        camTransform = Camera.main.transform;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        enemies = new List<Transform>();
        for (int i = 0; i < enemiesContainer.childCount; i++)
            enemies.Add(enemiesContainer.GetChild(i));

        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        chestBone = animator.GetBoneTransform(HumanBodyBones.Chest);
        weapon = weaponHandle.GetChild(0);
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
    private void LateUpdate()
    {
        HandleWeaponBehaviour();
        
    }
    private void HandleWeaponBehaviour()
    {
        float armsLayerWeight = animator.GetLayerWeight(1);
        float newLayerWeight;
        if (Input.GetButton("Fire2"))
            newLayerWeight = Mathf.Lerp(armsLayerWeight, 1f, Time.deltaTime * weaponToggleAnimSpeed);
        else
            newLayerWeight = Mathf.Lerp(armsLayerWeight, 0f, Time.deltaTime * weaponToggleAnimSpeed);

        animator.SetLayerWeight(1, newLayerWeight);
        animator.SetBool("aiming", newLayerWeight > .9f);
        if (animator.GetBool("aiming"))
        {
            Quaternion alignWeapon2camFwd = Quaternion.FromToRotation(rightHand.right, camTransform.forward);
            chestBone.rotation = alignWeapon2camFwd * chestBone.rotation;

            weaponHandle.gameObject.SetActive(true);
            weaponHandle.transform.position = rightHand.position;
            weaponHandle.transform.rotation = rightHand.rotation;
            if (Input.GetButtonDown("Fire1"))
            {
                GameObject go = GameObject.Instantiate(projectilePrefab.gameObject);
                go.transform.rotation = weapon.rotation;
                go.transform.position = weapon.position + weapon.transform.right * .4f;
            }
        }
        else
        {
            weaponHandle.gameObject.SetActive(false);
        }
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
        bool midair = true;
        for (float rayOffsetX = -1f; rayOffsetX <= 1f; rayOffsetX += 1f)
        {
            for (float rayOffsetZ = -1f; rayOffsetZ <= 1f; rayOffsetZ += 1f)
            {
                Vector3 rayOffset = new Vector3(rayOffsetX, 0, rayOffsetZ).normalized * 0.5f;
                Ray ray = new Ray(transform.position + rayOffset + Vector3.up * groundedRaycastThreshold, Vector3.down);
                if (Physics.Raycast(ray, 2 * groundedRaycastThreshold))
                {
                    midair = false;
                    break;
                }
            }
        }

        if (!midair)
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
        Vector3 deltaPosition = animator.deltaPosition.magnitude * moveDir;
        rigidbody.velocity = deltaPosition / Time.deltaTime;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, velY, rigidbody.velocity.z);
    }

    void ApplyRootRotation()
    {
        if (animator.GetBool("midair"))
            return;

        Vector3 D = GetClosestEnemyOrientation();
        D = GetAimingOrientation(ref D);

        if (
            (transform.forward - D).magnitude > 0.001f &&
            (transform.forward + D).magnitude > 0.001f
           )
        {
            float theta = Mathf.Acos(Vector3.Dot(transform.forward, D));
            theta *= Mathf.Rad2Deg;
            Vector3 axis = Vector3.Cross(transform.forward, D);
            transform.rotation = Quaternion.AngleAxis(theta * Time.deltaTime * rotSpeed, axis) * transform.rotation;
        }
        if ((transform.forward + D).magnitude < 0.001f)
        {
            transform.rotation = Quaternion.AngleAxis(2f, Vector3.up) * transform.rotation;
        }
    }

    private Vector3 GetAimingOrientation(ref Vector3 D)
    {
        if (animator.GetBool("aiming"))
        {
            D = camTransform.forward;
            D.y = 0f;
            D = D.normalized;
        }
        return D;
    }
    private Vector3 GetClosestEnemyOrientation()
    {
        Vector3 D = moveDir;
        float minDist = 999999f;
        int closestEnemyIndex = -1;
        for (int i = 0; i < enemies.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, enemies[i].position);
            if (dist < 4f && dist < minDist)
            {
                minDist = dist;
                closestEnemyIndex = i;
            }
        }

        if (closestEnemyIndex != -1)
        {
            D = enemies[closestEnemyIndex].position - transform.position;
            D.y = 0;
            D = D.normalized;
        }

        return D;
    }
}
