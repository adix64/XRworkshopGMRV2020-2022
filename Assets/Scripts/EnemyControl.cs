using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;
    public Transform player;
    Animator animator;
    public float attackDistanceThreshold = 1f;
    public float rotSpeed = 10f;
    Vector3 targetOffset;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        StartCoroutine(SeedMoveDirection(1f));
    }
    IEnumerator SeedMoveDirection(float t) 
    {
        yield return new WaitForSeconds(t);

        int randN = UnityEngine.Random.Range(0, 10);

        float offsetLength = UnityEngine.Random.Range(1f, 2f);
        switch (randN)
        {
            case 0:
                targetOffset = Vector3.zero;
                break;
            case 2: case 3: case 4:
                targetOffset = player.right * offsetLength;
                break;
            case 5:  case 6: 
                targetOffset = -player.right * offsetLength;
                break;
           case 7: case 8: case 9: case 1:
                targetOffset = player.forward * offsetLength * 3f;
                break;
        }

        float newWaitTime = UnityEngine.Random.Range(0.5f, 1.5f);
        yield return StartCoroutine(SeedMoveDirection(newWaitTime));
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
        if (agent.enabled)
        {
            if (stateNfo.IsName("Grounded"))
                agent.SetDestination(player.position + targetOffset);
            else
                agent.velocity = animator.deltaPosition / Time.deltaTime;
        }


        Quaternion newRot = Quaternion.LookRotation((player.position - transform.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * rotSpeed);
    }
}
