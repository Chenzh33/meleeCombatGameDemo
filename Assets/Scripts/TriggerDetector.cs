using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class TriggerDetector : MonoBehaviour {
        [SerializeField]
        private CharacterControl Control;
        [SerializeField]
        private Collider TriggerCollider;
        [SerializeField]
        private CharacterControl attacker;

        //[SerializeField]
        public List<Collider> CollidingParts = new List<Collider> ();

        void Awake () {
            Control = this.GetComponentInParent<CharacterControl> ();
            TriggerCollider = this.gameObject.GetComponent<Collider> ();
        }

        void Start () {

        }

        void Update () {

        }

        private void OnTriggerEnter (Collider col) {
            attacker = col.GetComponentInParent<CharacterControl> ();
            //if (Control == attacker || attacker.GetTriggerDetector().gameObject == col.gameObject)
            if (attacker == null)
                return;
            if (Control == attacker)
                return;
            if (!attacker.AttackingParts.Contains (col) && attacker.AttackPoint != col)
                return;
            //if(Control == attacker)
            //if (attacker == null || Control == attacker || attacker.GetTriggerDetector().gameObject == col.gameObject)

            if (!CollidingParts.Contains (col))
                CollidingParts.Add (col);

        }

        private void OnTriggerExit (Collider col) {
            if (CollidingParts.Contains (col))
                CollidingParts.Remove (col);

        }
    }
}