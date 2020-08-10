using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo {

    public class CameraManager : Singleton<CameraManager> {

        private Coroutine coroutine;
        private CameraController controller;

        public override void Init()
        {
            controller = GameObject.FindObjectOfType<CameraController>();
            //Time.timeScale = 0.5f;
        }

        IEnumerator _ShakeCamera(float second)
        {
            controller.TriggerCamera(CameraType.Shake);
            yield return new WaitForSeconds(second);
            controller.TriggerCamera(CameraType.Default);
        }

        public void ShakeCamera(float second)
        {
            if(coroutine != null)
                StopCoroutine(coroutine);
            if (second > 0f)
                coroutine = StartCoroutine(_ShakeCamera(second));
        }

        public void ResetTrigger()
        {
            controller.ResetTrigger();

        }
    }

}