﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class AttackAgent : MonoBehaviour
{

    public float range;

    public Stat knockback;
    public Stat knockbackSpeed;

    public Stat attackRate;
    public float attackDuration;

    protected LayerMask enemyMask;
    protected LayerMask obstacleMask;

    public delegate void OnAttackStart();
    public delegate void OnAttackComplete();

    public OnAttackStart onAttackStart;
    public OnAttackComplete onAttackComplete;

    protected float timeToNextAttack;
    protected float timeToAttackComplete;
    protected bool isAttacking;

    protected Transform target;
    protected Character character;
    protected List<Collider2D> hitTargets;

    protected virtual void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Init(LayerMask enemies, LayerMask obstacles)
    {
        enemyMask = enemies;
        obstacleMask = obstacles;
    }


    protected virtual void Update()
    {
        if (timeToNextAttack > 0)
        {
            timeToNextAttack -= Time.deltaTime;
        }
        else
        {
            timeToNextAttack = 0;
        }

        if (timeToAttackComplete > 0)
        {
            timeToAttackComplete -= Time.deltaTime;
        }
        else
        {
            if (isAttacking)
            {
                timeToAttackComplete = 0;
                isAttacking = false;

                if (onAttackComplete != null)
                {
                    onAttackComplete();
                }
            }
        }
    }

    public void Attack()
    {
        Attack(null);
    }

    public void Attack(Transform newTarget)
    {
        if(enemyMask == 0)
        {
            Debug.LogWarning("AttackAgent's EnemyMask is not being set. No damage will be done.");
        }
        if (timeToNextAttack > 0)
            return;

        timeToNextAttack = 1 / attackRate.GetValue();

        target = newTarget;
        timeToAttackComplete = attackDuration;
        isAttacking = true;
        hitTargets = new List<Collider2D>();

        if (onAttackStart != null) {
            onAttackStart();
        }

        OnAttack();
    }

    protected virtual void OnAttack()
    {

    }
}
