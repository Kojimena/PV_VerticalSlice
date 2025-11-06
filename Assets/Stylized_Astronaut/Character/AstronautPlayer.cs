using UnityEngine;

namespace AstronautPlayer
{
    [RequireComponent(typeof(CharacterController))]
    public class AstronautPlayer : MonoBehaviour
    {
        private Animator anim;
        private CharacterController controller;

        public float speed = 4.0f;        
        public float turnSpeed = 400.0f;
        public float jumpForce = 5.0f;
        public float gravity = -9.81f;

        private Vector3 velocity; 

        void Start()
        {
            controller = GetComponent<CharacterController>();
            anim = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            // --- Input ---
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Girar
            transform.Rotate(0f, h * turnSpeed * Time.deltaTime, 0f);

            // Mover en plano (Z local)
            Vector3 move = transform.forward * v * speed;
            controller.Move(move * Time.deltaTime);

            // Animator locomoción (0 idle, 1 run)
            bool moving = Mathf.Abs(v) > 0.1f;
            anim.SetInteger("AnimationPar", moving ? 1 : 0);

            // Ground / Jump
            bool grounded = controller.isGrounded;
            anim.SetBool("IsGrounded", grounded);

            if (Input.GetButtonDown("Jump"))      
            {
                velocity.y = jumpForce;
                anim.ResetTrigger("Jump");
                anim.SetTrigger("Jump");
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}