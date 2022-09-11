using UnityEngine;

namespace RSM
{
    public class ExampleCharacter : MonoBehaviour, IStateBehaviour
    {
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float gravityStrength;
        [SerializeField] private float speed;
        [SerializeField] private float jumpStrength;
        [SerializeField] private LayerMask terrainMask;

        private int maxJumps = 1;
        private int remainingJumps;

        private int horizontalInput;
        private bool jumpHeld;

        void Update()
        {
            GetInputs();
        }

        private void GetInputs()
        {
            if (Input.GetKeyDown(KeyCode.Space)) jumpTrigger.Trigger();
            if (Input.GetKeyUp(KeyCode.Space)) jumpTrigger.CancelTrigger();
            jumpHeld = Input.GetKey(KeyCode.Space);
            horizontalInput = 0;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontalInput -= 1;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontalInput += 1;
        }

        private void Gravity()
            => velocity.y -= gravityStrength;
        private void AirControl()
        {
            velocity.x += horizontalInput * speed * 0.005f;
            velocity.x = Mathf.Clamp(velocity.x, -speed, speed);
        }
        private void GroundControl()
        {
            velocity.x += horizontalInput * speed * 0.05f;
            velocity.x = Mathf.Clamp(velocity.x, -speed, speed);
        }

        private void ApplyVelocity()
        => transform.Translate(velocity * Time.deltaTime);

        private void CeilingCollision()
        {
            if (Physics2D.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector2.up, 0.05f, terrainMask))
            {
                if (velocity.y > 0) velocity.y = -0.01f;
            }
        }

        #region StateBehaviourMethods

        public void EnterIdle()
        {
            VSManager.Trace();
            remainingJumps = maxJumps;
            velocity.y = 0;
        }
        public void Idle()
        {
            VSManager.Trace();
            velocity.y = 0;
            velocity.x *= 0.98f;
            ApplyVelocity();
        }

        public void EnterRun()
        {
            VSManager.Trace();
            remainingJumps = maxJumps;
            velocity.y = 0;
        }
        public void Run()
        {
            VSManager.Trace();
            GroundControl();
            velocity.y = 0;
            ApplyVelocity();
        }

        public void Fall()
        {
            VSManager.Trace();
            CeilingCollision();
            AirControl();
            Gravity();
            ApplyVelocity();
        }

        public void EnterJump()
        {
            VSManager.Trace();
            remainingJumps--;
            velocity.x = horizontalInput * speed;
            velocity.y = jumpStrength;
        }
        public void Jump()
        {
            VSManager.Trace();
            CeilingCollision();
            AirControl();
            ApplyVelocity();
        }

        public void EnterDie()
        {
            VSManager.Trace();
            velocity = Vector2.zero;
        }
        #endregion

        #region ConditionMethods

        private TransitionCondition jumpTrigger;
        public TransitionCondition JumpTrigger()
        {
            VSManager.Trace();
            if (jumpTrigger == null) jumpTrigger = new TransitionCondition(false, true);
            return jumpTrigger;
        }

        private TransitionCondition dieTrigger;
        public TransitionCondition DieTrigger()
        {
            VSManager.Trace();
            if (dieTrigger == null) dieTrigger = new TransitionCondition(false, true);
            return dieTrigger;
        }

        public TransitionCondition IsGrounded()
        {
            VSManager.Trace();
            return new TransitionCondition(Physics2D.Raycast(transform.position + new Vector3(0, -0.2f, 0), Vector2.down, 0.05f, terrainMask));
        }

        public TransitionCondition IsFalling()
        {
            VSManager.Trace();
            return new TransitionCondition(velocity.y < 0);
        }

        public TransitionCondition JumpHeld()
        {
            VSManager.Trace();
            return new TransitionCondition(jumpHeld);
        }

        public TransitionCondition CanJump()
        {
            VSManager.Trace();
            return new TransitionCondition(remainingJumps > 0);
        }

        public TransitionCondition MoveHeld()
        {
            VSManager.Trace();
            return new TransitionCondition(horizontalInput != 0);
        }
        #endregion
    }
}