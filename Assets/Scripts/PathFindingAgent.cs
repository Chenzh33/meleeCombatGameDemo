using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace meleeDemo {

    public class PathFindingAgent : MonoBehaviour {
        public Transform Target;
        NavMeshAgent navMeshAgent;

        public void Init () {
            Target = null;
            navMeshAgent.enabled = true;
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

        public bool IsStopped () {
            if (navMeshAgent.enabled)
                return navMeshAgent.isStopped;
            else
                return true;
        }
        public void Stop () {
            if (navMeshAgent.enabled)
                navMeshAgent.isStopped = true;
        }
        public void Dead () {
            Stop ();
            navMeshAgent.enabled = false;
            PoolObject pobj = GetComponent<PoolObject> ();
            PoolManager.Instance.ReturnToPool (pobj);
        }

        public void StartNav () {
            navMeshAgent.isStopped = false;
        }

    }
}