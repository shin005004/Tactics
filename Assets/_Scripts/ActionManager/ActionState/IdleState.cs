using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// No Actions besides look to direction
// No enemy found - has response to damage tho
public class IdleState : ActionBaseState
{
    // ViewActionHanlding
    private Vector3 lastLookDir;
    private float lastLookTime = -1f;
    private ActionManager _actionManager;

    // Planning/Playing
    private Vector3 lastVelocity;

    public override void EnterState(ActionManager actionManager)
    {
        _actionManager = actionManager;
        
        GameManager.OnBeforeStateChanged += ChangeState;
        lastLookTime = -1f;
    }
    public override void UpdateState(ActionManager actionManager)
    {
        if(GameManager.Instance.State == GameState.Playing)
            MovePlayer(actionManager);
    }

    private void ChangeState(GameState newState)
    {
        if(newState == GameState.Planning)
        {
            lastVelocity = _actionManager.navMeshAgent.velocity;
            _actionManager.navMeshAgent.velocity = Vector3.zero;
            _actionManager.navMeshAgent.isStopped = true;
        }
        if(newState == GameState.Playing)
        {
            if(GameManager.Instance.State == GameState.Planning)
                _actionManager.navMeshAgent.velocity = lastVelocity;
            _actionManager.navMeshAgent.isStopped = false;
        }
    }

    private void MovePlayer(ActionManager actionManager)
    {
        // Action selection
        if (actionManager.pathPoints.Count != 0)
        {
            // View action = PlayerAction.Look
            PathInfo peekPath = actionManager.pathPoints.Peek();

            if (peekPath.action == PlayerAction.Look)
            {
                RotateTowardTarget(actionManager, peekPath.viewDir, 10f);
                lastLookTime = Time.time;
                lastLookDir = peekPath.viewDir;
            }
            else if (Time.time - lastLookTime < 1.5f)
                RotateTowardTarget(actionManager, lastLookDir, 10f);
            else
                if (actionManager.navMeshAgent.steeringTarget != null)
                    RotateTowardTarget(actionManager, actionManager.navMeshAgent.steeringTarget - actionManager.transform.position, 2f);
        }

        if (ShouldSetDestination(actionManager))
        {
            PathInfo peekPath = actionManager.pathPoints.Peek();
            actionManager.navMeshAgent.SetDestination(peekPath.point);
            ErasePassedPath(actionManager);
        }
    }

    private bool ShouldSetDestination(ActionManager actionManager)
    {
        if (actionManager.pathPoints.Count == 0)
            return false;

        if (actionManager.navMeshAgent.hasPath == false || actionManager.navMeshAgent.remainingDistance < 0.5f)
            return true;

        return false;
    }

    private PathInfo ErasePassedPath(ActionManager actionManager)
    {
        if (actionManager._pathMaker.points.Count != 0)
        {
            var tempInfo = actionManager._pathMaker.points[0];
            actionManager.pathPoints.Dequeue();
            actionManager._pathMaker.points.RemoveAt(0);

            actionManager._pathMaker.UpdatePath();
            return tempInfo;
        }
        return new PathInfo(Vector3.zero);
    }


    // NavMeshAgent Functions

    
    // targetVector = direction
    // Look At targetVector direction with given Speed
    public void RotateTowardTarget(ActionManager actionManager, Vector3 targetVector, float lookSpeed)
    {
        if (targetVector.magnitude == 0)
            return;
        Vector3 direction = (targetVector).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        actionManager.transform.rotation = Quaternion.Slerp(actionManager.transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
    }
}
