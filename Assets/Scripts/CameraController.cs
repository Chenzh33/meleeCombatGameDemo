using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
        private CinemachineTargetGroup targetGroup;
        
        void Awake()
        {
            animator = GetComponent<Animator>();
            targetGroup = GetComponent<CinemachineTargetGroup>();
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

        public void AddToTargetGroup(CharacterControl unit)
        {
            Transform spine = unit.GetSpine();
            Debug.Log(spine);
            if (targetGroup.FindMember(spine) == -1)
                targetGroup.AddMember(spine, unit.CharacterData.TargetGroupWeight, unit.CharacterData.TargetGroupRadius);

        }

        public void RemoveFromTargetGroup(CharacterControl unit)
        {
            Transform spine = unit.GetSpine();
            if (targetGroup.FindMember(spine) != -1)
                targetGroup.RemoveMember(spine);

        }
        public bool IsShaking()
        {
            return animator.GetCurrentAnimatorStateInfo (0).IsName ("Shake");
        }
    }
}