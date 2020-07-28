using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace meleeDemo
{
    public class PlayerControl : MonoBehaviour
    {
        public float speed = 6.0f;
        public float smooth = 5.0f;
        float times;

        CharacterController m_Controller;
        Animator m_Animator;
        AnimatorStateInfo stateinfo;

        private float hInput;
        private float vInput;
        private Vector3 moveDirection = Vector3.zero;

        void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_Controller = GetComponent<CharacterController>();
        }

        public void MoveForward(Vector3 dir, float s, float sgraph)
        {
            //m_Controller.Move(moveDirection * s * sgraph * Time.deltaTime);
            m_Controller.Move(dir * s * sgraph * Time.deltaTime);
        }

        void Update()
        {
            /*
            hInput = Input.GetAxis("Horizontal");
            vInput = Input.GetAxis("Vertical");
            //Debug.Log(hInput);
            //Debug.Log(vInput);
            */
            Vector2 input = VirtualInputManager.Instance.GetInput().InputVector;
            
            moveDirection = new Vector3(input.x, 0, input.y);
            //moveDirection = Vector3.ClampMagnitude(moveDirection, 1);
            stateinfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            //if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            if(moveDirection.magnitude > 0f)
            {
                m_Animator.SetBool("IsRun", true);
                if (stateinfo.IsName("Run"))
                {
                    MoveForward(moveDirection, speed, 1.0f);
                    float angle = Mathf.Acos(Vector3.Dot(new Vector3(0, 0, 1), moveDirection)) * Mathf.Rad2Deg;
                    if (input.x < 0.0f) { angle = -angle; }
                    Quaternion target = Quaternion.Euler(new Vector3(0, angle, 0));
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * smooth);
                }
            }
            else
            {
                m_Animator.SetBool("IsRun", false);
            }

            //Debug.Log(stateinfo.IsName("Run"));
            /*
             if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2")) { times = Mathf.Ceil(stateinfo.normalizedTime); }
             if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
             {
                 if (stateinfo.IsName("Idle") || stateinfo.IsName("Run") || stateinfo.normalizedTime > times)
                 {
                     if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
                     {
                         float h = Input.GetAxis("Horizontal");
                         float v = Input.GetAxis("Vertical");
                         //椭圆映射
                         float x = h * Mathf.Sqrt(1 - (v * v) / 2.0f);
                         float z = v * Mathf.Sqrt(1 - (h * h) / 2.0f);
                         moveDirection = new Vector3(x, 0, z);
                         m_Controller.Move(moveDirection * speed * Time.deltaTime);
                         //获得角度
                         float angle = Mathf.Acos(Vector3.Dot(new Vector3(0, 0, 1), moveDirection)) * Mathf.Rad2Deg;
                         //识别z轴两侧方向
                         if (h < 0.0f) { angle = -angle; }
                         //平滑转向
                         Quaternion target = Quaternion.Euler(new Vector3(0, angle, 0));
                         transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * smooth);
                     }
                 }
             }
             */
            if (Input.GetButtonDown("Fire3"))
            {
                this.enabled = false;
            }
        }
    }

}