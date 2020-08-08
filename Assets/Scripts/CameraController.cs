using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo
{

    public enum CameraType
    {
        Default,
        Shake
    }

    public class CameraController : MonoBehaviour
    {
        private Animator animator;
        
        void Awake()
        {
            animator = GetComponent<Animator>();
        }


        public void TriggerCamera(CameraType c)
        {
            animator.SetTrigger(c.ToString());
        }

        public void ResetTrigger()
        {
            CameraType[] cams = System.Enum.GetValues(typeof(CameraType)) as CameraType[];
            foreach(CameraType c in cams)
            {
                animator.ResetTrigger(c.ToString());
            }
        }
        void Start()
        {

        }

        void Update()
        {
        }
    }
}