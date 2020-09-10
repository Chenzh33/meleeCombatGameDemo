using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class CameraManager : Singleton<CameraManager> {

        //private Coroutine coroutine;
        private CameraController controller;
        public List<CameraShaker> CurrentCameraShakers = new List<CameraShaker> ();

        public override void Init () {
            controller = GameObject.FindObjectOfType<CameraController> ();
            CharacterControl[] controls = FindObjectsOfType (typeof (CharacterControl)) as CharacterControl[];
            foreach (CharacterControl c in controls) {
                if (c.isPlayerControl)
                    controller.AddToTargetGroup (c);
            }
        }
        /*
                IEnumerator _ShakeCamera(float second)
                {
                    controller.TriggerCamera(CameraType.Shake);
                    yield return new WaitForSeconds(second);
                    controller.TriggerCamera(CameraType.Default);
                }
                */

        public void AddToTargetGroup (CharacterControl unit) {
            controller.AddToTargetGroup (unit);
        }

        public void ZoomCameraPerFrame (float offset) {
            controller.ZoomCameraPerFrame (offset);

        }
        public void RemoveFromTargetGroup (CharacterControl unit) {
            controller.RemoveFromTargetGroup (unit);
        }

        public void UpdateTargetWeight (CharacterControl unit) {
            controller.UpdateTargetWeight (unit);
        }
        public void ShakeCamera () {
            controller.TriggerCamera (CameraType.Shake);
            //controller.ResetTrigger ();
            /*
            if(coroutine != null)
                StopCoroutine(coroutine);
            if (second > 0f)
                coroutine = StartCoroutine(_ShakeCamera(second));
                */
        }

        public void ResetCamera () {
            controller.TriggerCamera (CameraType.Default);
            //controller.ResetTrigger ();
            /*
            if(coroutine != null)
                StopCoroutine(coroutine);
            if (second > 0f)
                coroutine = StartCoroutine(_ShakeCamera(second));
                */
        }
        public void ResetTrigger () {
            controller.ResetTrigger ();

        }
        public bool IsShaking () {
            return (controller.IsShaking ());

        }
    }

}