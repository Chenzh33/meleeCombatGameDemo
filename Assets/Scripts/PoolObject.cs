using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class PoolObject : MonoBehaviour {
        public PoolObjectType type;

        void Start () {

        }

        void Update () {

        }
        IEnumerator _WaitAndDestroy (float duration) {
            yield return new WaitForSeconds (duration);
            ParticleSystem ps = this.GetComponent<ParticleSystem> ();
            ps.Clear ();
            ps.Stop (true);
            PoolManager.Instance.ReturnToPool (this);

        }
        public void WaitAndDestroy (float duration) {
            StartCoroutine (_WaitAndDestroy (duration));
        }
    }
}