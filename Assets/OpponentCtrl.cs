using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentCtrl : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;
    public Transform player;
    Animator animator;
    public float attackThreshold = 1.1f;
    CapsuleCollider capsule;
    public float groundedRaycastThreshold = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.position);

        Vector3 characterSpacDir = transform.InverseTransformVector(agent.velocity);
        animator.SetFloat("forward", characterSpacDir.z, .5f, Time.deltaTime);
        animator.SetFloat("right", characterSpacDir.x, .5f, Time.deltaTime);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < attackThreshold)
        {
            animator.SetTrigger("attack");
        }
        HandleJump();
    }
    private void HandleJump()
    {
        bool grounded = false;

        for (float offsetX = -1f; offsetX <= 1f; offsetX += 1f)
        {
            for (float offsetZ = -1f; offsetZ <= 1f; offsetZ += 1f)
            {
                Vector3 rayOffset = new Vector3(offsetX, 0f, offsetZ).normalized * capsule.radius;
                Ray ray = new Ray(transform.position + rayOffset + Vector3.up * groundedRaycastThreshold, Vector3.down);
                if (Physics.Raycast(ray, 2 * groundedRaycastThreshold))
                {
                    grounded = true;
                    break;
                }
            }
        }
        if (grounded)
        {
            animator.SetBool("midair", false);
            animator.applyRootMotion = true;
            agent.speed = 0.7f;
        }
        else
        {
            agent.speed = 3.5f;
            animator.applyRootMotion = false;
            animator.SetBool("midair", true);
        }
    }
}
