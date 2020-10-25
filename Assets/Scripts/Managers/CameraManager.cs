using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class CameraManager : Singleton<CameraManager> {

        //private Coroutine coroutine;
        private CameraController controller;
        public List<CameraShaker> CurrentCameraShakers = new List<CameraShaker> ();
        private Coroutine PlayCloseUpCoroutine;
        private Coroutine ExitCloseUpCoroutine;

        public override void Init () {
            controller = GameObject.FindObjectOfType<CameraController> ();
            CharacterControl[] controls = FindObjectsOfType (typeof (CharacterControl)) as CharacterControl[];
            foreach (CharacterControl c in controls) {
                if (c.isPlayerControl) {
                    controller.AddToTargetGroup (c);
                    controller.AddToCloseUpTargetGroup (c);

                }
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

        public void PlayCloseUp (CharacterControl player) {
            if (PlayCloseUpCoroutine != null)
                StopCoroutine (PlayCloseUpCoroutine);
            PlayCloseUpCoroutine = StartCoroutine (_PlayCloseUp (player));

        }
        public void ExitCloseUp (CharacterControl player) {
            if (ExitCloseUpCoroutine != null)
                StopCoroutine (ExitCloseUpCoroutine);
            ExitCloseUpCoroutine = StartCoroutine (_ExitCloseUp (player));

        }

        IEnumerator _PlayCloseUp (CharacterControl player) {
            controller.RemoveFromCloseUpTargetGroup (player);
            controller.RemoveFromCloseUpTargetGroup (player.CharacterData.GrapplingTarget);
            controller.AddToCloseUpTargetGroup (player);
            controller.AddToCloseUpTargetGroup (player.CharacterData.GrapplingTarget);
            controller.TriggerCamera (CameraType.CloseUp);
            float t = 0f;
            while (t < 0.25f) {
                controller.ZoomCameraPerFrame (-20f * Time.deltaTime);
                t = t + Time.deltaTime;
                yield return null;
            }
        }
        IEnumerator _ExitCloseUp (CharacterControl player) {
            Debug.Log ("trigger default!");
            controller.TriggerCamera (CameraType.Default);
            yield return new WaitForSeconds (0.5f);
            controller.RemoveFromCloseUpTargetGroup (player);
            controller.RemoveFromCloseUpTargetGroup (player.CharacterData.GrapplingTarget);
        }
    }

}