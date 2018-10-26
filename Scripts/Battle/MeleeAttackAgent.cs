﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FaceController))]
[RequireComponent(typeof(BoxCollider2D))]
public class MeleeAttackAgent : AttackAgent {
    
    public AttackDirection attackDirection;

    FaceController faceController;
    BoxCollider2D ctrlCollider;
    AttackArea attackArea = new AttackArea();

    protected override void Awake()
    {
        base.Awake();
        faceController = GetComponent<FaceController>();
        ctrlCollider = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();

        if (isAttacking)
        {
            RecalculateAttackArea();

            Collider2D[] hits = Physics2D.OverlapAreaAll(attackArea.leadingPoing, attackArea.trailingPoint, enemyMask);

            for(int i = 0; i < hits.Length; i++)
            {
                Vector2 heading = hits[i].transform.position - transform.position;
                RaycastHit2D obstacle = Physics2D.Raycast(transform.position, heading.normalized, heading.magnitude, obstacleMask);

                if(!obstacle && !hitTargets.Contains(hits[i]))
                {
                    Character enemyCharacter = hits[i].GetComponent<Character>();

                    if(enemyCharacter != null)
                    {
                        enemyCharacter.TakeDamage(character.attackStats.GetAttacks(), this);
                        hitTargets.Add(hits[i]);
                    }
                }
            }
        }
    }

    protected override void OnAttack()
    {
        RecalculateAttackArea();
    }

    void RecalculateAttackArea()
    {
        Vector2 fDir = faceController.GetFaceDirection();
        Vector2 fDirNormalLeft = Vector2.Perpendicular(fDir);
        Vector2 fDirNormalRight = -fDirNormalLeft;
        Vector2 bottomRight = (Vector2)transform.position + fDir * ctrlCollider.size / 2 + fDirNormalRight * range / 2;
        Vector2 topLeft = bottomRight + fDir * range + fDirNormalLeft * range;

        switch (attackDirection)
        {
            case AttackDirection.Static:
                attackArea.leadingPoing = topLeft;
                attackArea.trailingPoint = bottomRight;
                break;
            case AttackDirection.LeftToRight:
                attackArea.leadingPoing = topLeft - fDir * range + fDirNormalRight * range * (1 - timeToAttackComplete / attackDuration);
                attackArea.trailingPoint = topLeft;
                break;
            case AttackDirection.RightToLeft:
                attackArea.leadingPoing = bottomRight + fDirNormalLeft * range * (1 - timeToAttackComplete / attackDuration); 
                attackArea.trailingPoint = bottomRight + fDir * range;
                break;
            case AttackDirection.Outward:
                attackArea.leadingPoing = topLeft - fDir * range + fDir * range * (1 - timeToAttackComplete / attackDuration);
                attackArea.trailingPoint = bottomRight;
                break;
        }
    }

    void OnDrawGizmos()
    {
        if (isAttacking)
        {
            Gizmos.color = Color.green;
            Vector2 p1 = new Vector2(attackArea.leadingPoing.x, attackArea.trailingPoint.y);
            Vector2 p2 = new Vector2(attackArea.trailingPoint.x, attackArea.leadingPoing.y);

            Gizmos.DrawLine(attackArea.leadingPoing, p1);
            Gizmos.DrawLine(p1, attackArea.trailingPoint);
            Gizmos.DrawLine(attackArea.trailingPoint, p2);
            Gizmos.DrawLine(p2, attackArea.leadingPoing);
        }
    }

    public enum AttackDirection
    {
        Static,
        LeftToRight,
        RightToLeft,
        Outward
    }

    struct AttackArea
    {
        public Vector2 leadingPoing;
        public Vector2 trailingPoint;
    }
}
