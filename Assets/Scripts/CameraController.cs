using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace meleeDemo {

    public enum CameraType {
        Default,
        Shake,
        CloseUp
    }

    public class CameraController : MonoBehaviour {
        private Animator animator;
        private CinemachineTargetGroup targetGroup;
        private CinemachineTargetGroup closeUpTargetGroup;
        private CinemachineStateDrivenCamera cameras;
        private CinemachineCameraOffset cameraOffset;

        public float MaxExtraOffset = 5f;
        public float MinExtraOffset = 0f;

        void Awake () {
            animator = GetComponent<Animator> ();

            cameras = GameObject.FindObjectOfType (typeof (CinemachineStateDrivenCamera)) as CinemachineStateDrivenCamera;
            cameraOffset = cameras.gameObject.GetComponent<CinemachineCameraOffset> ();

            targetGroup = GetComponent<CinemachineTargetGroup> ();
            closeUpTargetGroup = cameras.GetComponentInChildren<CinemachineTargetGroup> ();
        }

        public void TriggerCamera (CameraType c) {
            animator.SetTrigger (c.ToString ());
        }

        public void ResetTrigger () {
            CameraType[] cams = System.Enum.GetValues (typeof (CameraType)) as CameraType[];
            foreach (CameraType c in cams) {
                animator.ResetTrigger (c.ToString ());
            }
        }
        void Start () {

        }

        public void ZoomCameraPerFrame (float offset) {
            if ((offset > 0 && cameraOffset.m_Offset.z > -MaxExtraOffset) ||
                offset < 0 && cameraOffset.m_Offset.z < -MinExtraOffset) {
                //cameraOffset.m_Offset.y += offset;
                cameraOffset.m_Offset.z += -offset;
            }
        }

        void Update () {

        }

        public void AddToTargetGroup (CharacterControl unit) {
            Transform spine = unit.GetSpine ();
            //Debug.Log (spine);
            if (targetGroup.FindMember (spine) == -1)
                targetGroup.AddMember (spine, unit.CharacterData.TargetGroupWeight, unit.CharacterData.TargetGroupRadius);

        }

        public void AddToCloseUpTargetGroup (CharacterControl unit) {
            Transform spine = unit.GetSpine ();
            if (closeUpTargetGroup.FindMember (spine) == -1)
                closeUpTargetGroup.AddMember (spine, unit.CharacterData.TargetGroupWeight, unit.CharacterData.TargetGroupRadius);

        }
        public void RemoveFromTargetGroup (CharacterControl unit) {
            Transform spine = unit.GetSpine ();
            if (targetGroup.FindMember (spine) != -1)
                targetGroup.RemoveMember (spine);

        }
        public void RemoveFromCloseUpTargetGroup (CharacterControl unit) {
            Transform spine = unit.GetSpine ();
            if (closeUpTargetGroup.FindMember (spine) != -1)
                closeUpTargetGroup.RemoveMember (spine);

        }
        public void UpdateTargetWeight (CharacterControl unit) {
            int idx = targetGroup.FindMember (unit.GetSpine ());
            if (idx != -1)
                targetGroup.m_Targets[idx].weight = unit.CharacterData.TargetGroupWeight;
        }

        public bool IsShaking () {
            return animator.GetCurrentAnimatorStateInfo (0).IsName ("Shake");
        }
    }
}