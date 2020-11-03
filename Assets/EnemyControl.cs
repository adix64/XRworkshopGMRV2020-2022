using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;
    public Transform player;
    Animator animator;
    public float attackDistanceThreshold = 1f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {


        Vector3 characterSpaceDir = transform.InverseTransformVector(agent.velocity);
        animator.SetFloat("forward", characterSpaceDir.z, 0.5f, Time.deltaTime);
        animator.SetFloat("right", characterSpaceDir.x, 0.5f, Time.deltaTime);

        attackDistanceThreshold = 1f + Mathf.Sin(Time.time) * .5f;
        if (Vector3.Distance(transform.position, player.position) < attackDistanceThreshold)
        {
            animator.SetTrigger("attack");
        }

        var stateNfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateNfo.IsName("Dead"))
        {
            agent.enabled = false;
        }
        if(agent.enabled)
            agent.SetDestination(player.position);
    }
}
