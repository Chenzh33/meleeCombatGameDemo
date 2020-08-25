using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace meleeDemo {

    public class PathFindingAgent : MonoBehaviour {
        public Transform Target;
        NavMeshAgent navMeshAgent;

        public void Init() {
            Target = null;
            navMeshAgent.isStopped = true;

        }
        void Awake () {
            navMeshAgent = GetComponent<NavMeshAgent> ();

        }

        void Update () {
            // if (Input.GetKey (KeyCode.Space)) {
            //     GoToTarget();

            // }

        }

        
        public void SetTarget (Transform target) {
            Target = target;

        }
        public void GoToTarget () {
            navMeshAgent.SetDestination (Target.position);
        }

        public bool IsStopped()
        {
            return navMeshAgent.isStopped;
        }
        public void Stop()
        {
            navMeshAgent.isStopped = true;
        }

        public void StartNav()
        {
            navMeshAgent.isStopped = false;
        }

    }
}