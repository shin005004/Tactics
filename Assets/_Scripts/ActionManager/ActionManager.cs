using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionManager : MonoBehaviour
{
    // Finite State Machine
    ActionBaseState currentState;
    public IdleState idleState = new IdleState();
    public CombatState combatState = new CombatState();
    public ActionState actionState = new ActionState();

    // Other Scripts
    [HideInInspector] public PathMaker _pathMaker;
    [HideInInspector] public ActionMaker _actionMaker;
    [HideInInspector] public NavMeshAgent navMeshAgent;

    // Moving Options
    public Queue<PathInfo> pathPoints = new Queue<PathInfo>();
    public float movementSpeed;
    public float rotationSpeed;

    // UI Options 
    public Queue<GameObject> lookTransforms = new Queue<GameObject>();
    public GameObject arrowPrefab;

    private void Start()
    {
        // System Settings
        _pathMaker = GetComponent<PathMaker>();
        _actionMaker = GetComponent<ActionMaker>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;

        // Init
        lookTransforms.Clear();

        // State Machine
        currentState = idleState;
        currentState.EnterState(this);
    }

    private void LateUpdate()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(ActionBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }


    // Mover Options 
    public void UpdateMoverPath()
    {
        for(int i = 0; i < lookTransforms.Count; i++)
            Destroy(lookTransforms.Dequeue());
        pathPoints.Clear();

        for (int i = 0; i < _pathMaker.points.Count; i++)
        {
            switch(_pathMaker.points[i].action)
            {
                case PlayerAction.Look:
                    var direction = _pathMaker.points[i].viewDir.normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    var tempArrow = Instantiate(arrowPrefab, _pathMaker.points[i].point + new Vector3(0, 0.1f, 0), lookRotation) as GameObject;
                    lookTransforms.Enqueue(tempArrow);
                    break;
            }
            pathPoints.Enqueue(_pathMaker.points[i]);
        }
    }
}