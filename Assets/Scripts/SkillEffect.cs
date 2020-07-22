using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo
{
    public abstract class SkillEffect : ScriptableObject
    {
        public abstract void OnEnter(StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo);
        public abstract void UpdateEffect(StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo);
        public abstract void OnExit(StatewithEffect stateEffect, Animator animator, AnimatorStateInfo animatorStateInfo);
    }
}