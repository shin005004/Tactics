using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Proto
{
    public class NavAgentManager : MonoBehaviour
    {
        [HideInInspector] public NavMeshAgent navMeshAgent;

        // PathInfo
        public Vector3 pathNextPosition;
        public Vector3 pathNextRotation;

        // EnemyFound Info
        public Vector3 enemyNextRotation;

        // Common things
        public float rotationSpeed;
        public bool isEnemy = false;
        public bool isPath = true;

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            pathNextPosition = transform.position;
            pathNextRotation = transform.position + transform.forward;
        }
        void Update()
        {
            // follow path and do actions
            if (isPath)
            {
                if (pathNextPosition != navMeshAgent.nextPosition)
                {
                    navMeshAgent.SetDestination(pathNextPosition);
                }
                RotateTowardTarget(pathNextRotation, rotationSpeed);
            }
            // enemyFound
            if (isEnemy)
            {
                RotateTowardTarget(enemyNextRotation, rotationSpeed);
            }
            else // idle state
            {

            }
        }

        // targetVector = world point
        // Look At targetVector point with chosen speed
        public void RotateTowardTarget(Vector3 targetVector, float lookSpeed)
        {
            Vector3 direction = (targetVector - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
        }
    }

}