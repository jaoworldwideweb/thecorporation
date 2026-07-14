using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StarHumanoid : Character{
    [Header("References")]
    [SerializeField] private Transform player;
    private bool chasingPlayer;

    private void Start(){
        ResumeWandering();
    }

    private void FixedUpdate(){
        Vector3 direction = player.position - transform.position;

        if (Physics.Raycast(transform.position + Vector3.up * 2f, direction.normalized, out RaycastHit hit, Mathf.Infinity, 769, QueryTriggerInteraction.Ignore)){
            if (hit.transform.CompareTag("Player")){
                if (!chasingPlayer){
                    chasingPlayer = true;
                    TargetPlayer();
                }
                return;
            }
        }
        if (chasingPlayer){
            chasingPlayer = false;
            ResumeWandering();
        }
    }

    public void TargetPlayer(){
        StartRoutine(FollowRoutine(player));
    }

    public void ResumeWandering(){
        StartRoutine(WanderRoutine());
    }

    public void Hear(Vector3 soundPosition){
        StartRoutine(MoveToRoutine(soundPosition));
    }
}
