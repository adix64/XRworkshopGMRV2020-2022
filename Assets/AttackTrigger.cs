using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public string compareTo;
    public string hitType;
    public int damage = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(compareTo))
        {
            Animator opponentAnimator = other.GetComponentInParent<Animator>();
            if (opponentAnimator.GetFloat("timeSinceLastHit") > .25f)
            {
                opponentAnimator.SetTrigger(hitType);
                opponentAnimator.SetInteger("takenDamage", damage);
            }
        }
    }
}
