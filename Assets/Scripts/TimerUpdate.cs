﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUpdate : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("timeSinceLastHit", animator.GetFloat("timeSinceLastHit") + Time.deltaTime);
    }
}
