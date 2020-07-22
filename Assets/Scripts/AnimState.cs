using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo
{
    public class AnimState : MonoBehaviour
    {
        Animator m_Animator;
        AnimatorStateInfo stateinfo;
        Quaternion target;
        Quaternion target1;
        Quaternion target2;
        Quaternion target3;
        //Shake m_Shake;

        void Start()
        {
            m_Animator = GetComponent<Animator>();
            //m_Shake = GameObject.Find("Main Camera").GetComponent<Shake>();
        }
        void Update()
        {
            stateinfo = m_Animator.GetCurrentAnimatorStateInfo(0);

            //移动
            /*
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                m_Animator.SetBool("IsRun", true);
            }
            else
            {
                m_Animator.SetBool("IsRun", false);
            }
            */

            //Fire1
            //if (Input.GetButton("Fire1"))
            //{
            //m_Animator.SetBool("IsAttack", true);
            //}
            //else
            //{
            //m_Animator.SetBool("IsAttack", false);
            //}

            if (Input.GetButtonUp("Fire1"))
            {
                if (!stateinfo.IsName("Attack2"))
                    m_Animator.SetTrigger("Attack 1");
                if (stateinfo.IsName("Attack1"))
                {
                    if (stateinfo.normalizedTime > 0.5f)
                    {
                        m_Animator.SetTrigger("Attack 2");
                    }
                }
            }

            //Fire3按钮触发Dead
            if (Input.GetButton("Fire3"))
            {
                m_Animator.SetTrigger("IsDead");
            }
        }
    }
}