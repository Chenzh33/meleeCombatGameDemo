using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo
{
    public class StatewithEffect : StateMachineBehaviour
    {
        public List<SkillEffect> ListSkillEffect = new List<SkillEffect>();
        public PlayerControl playerControl;



        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(playerControl == null)
                playerControl = animator.transform.root.GetComponent<PlayerControl>();
            foreach (SkillEffect se in ListSkillEffect)
            {
                se.OnEnter(this, animator, stateInfo);
            }

        }
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (SkillEffect se in ListSkillEffect)
            {
                se.UpdateEffect(this, animator, stateInfo);
            }

        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (SkillEffect se in ListSkillEffect)
            {
                se.OnExit(this, animator, stateInfo);
            }

        }
    }
}