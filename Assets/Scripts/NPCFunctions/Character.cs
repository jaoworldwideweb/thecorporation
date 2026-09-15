using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour{
#region Inspector
    [Header("Navigation")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected AILocationSelectorScript wanderer;
    protected Coroutine currentRoutine;
#endregion

#region MainFunctions
    protected virtual void Awake(){
        if (!agent){
            agent = GetComponent<NavMeshAgent>();			
		}
    }
#endregion
	
#region API
	protected virtual void MoveTo(Vector3 destination){
		StartRoutine(MoveToRoutine(destination));
	}

	protected virtual void Follow(Transform target){
		StartRoutine(FollowRoutine(target));
	}

	protected virtual void StopMovement(){
		StopRoutine();
		
		if (agent != null){
			agent.ResetPath();
			agent.isStopped = true;
		}
	}

	protected virtual void ResumeMovement(){
		if (agent != null){
			agent.isStopped = false;			
		}
	}
#endregion

#region Helpers
	protected bool HasReachedDestination(){
		if (agent.pathPending){
			return false;			
		}
		if (agent.remainingDistance > agent.stoppingDistance){
			return false;			
		}
		
		return !agent.hasPath || agent.velocity.sqrMagnitude < 0.01f;
	}

	protected bool IsMoving(){
		return agent.hasPath && agent.velocity.sqrMagnitude > 0.01f;
	}
#endregion


#region Coroutines
	protected virtual IEnumerator WanderRoutine(){
		while (true){
			ResumeMovement();
			agent.SetDestination(wanderer.GetNewTarget());
			
			yield return new WaitUntil(HasReachedDestination);
			
			OnDestinationReached();
			
			yield return null;
		}
	}

	protected virtual IEnumerator MoveToRoutine(Vector3 destination){
		ResumeMovement();
		agent.SetDestination(destination);
		
		yield return new WaitUntil(HasReachedDestination);
		
		OnDestinationReached();
	}

	protected virtual IEnumerator FollowRoutine(Transform target){
		ResumeMovement();
		while (target != null){
			agent.SetDestination(target.position);
			yield return null;
		}

		OnTargetLost();
	}
#endregion

#region RoutineHandler
	protected void StartRoutine(IEnumerator routine){
		StopRoutine();
		currentRoutine = StartCoroutine(routine);
	}

	protected void StopRoutine(){
		if (currentRoutine == null)
			return;

		StopCoroutine(currentRoutine);
		currentRoutine = null;
	}
#endregion

#region Virtual Callbacks
	/// <summary>
	/// Called whenever MoveTo() or Wander() reaches its destination.
	/// </summary>
	protected virtual void OnDestinationReached(){}

	/// <summary>
	/// Called if a Follow() target disappears.
	/// </summary>
	protected virtual void OnTargetLost(){}
#endregion
}