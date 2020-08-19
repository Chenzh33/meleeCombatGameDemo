using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace meleeDemo {

    public class PathFindingAgent : MonoBehaviour {
        public GameObject target;
        NavMeshAgent navMeshAgent;

        void Awake () {
            navMeshAgent = GetComponent<NavMeshAgent> ();

        }

        void Update () {
            if (Input.GetKey (KeyCode.Space)) {
                GoToTarget(target.transform.position);

            }

        }

        public void GoToTarget (Vector3 targetPos) {
            navMeshAgent.SetDestination (targetPos);
        }
    }
}