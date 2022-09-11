using UnityEngine;

namespace RSM
{
    public class EnemyState : MonoBehaviour, IStateBehaviour
    {
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float speed;
        [SerializeField] private LayerMask terrainMask;

        private Collider sight;

        private CharacterController characterController;
        private RotateToAim rotateController;
        private EnemyHelper enemyHelper;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            rotateController = GetComponentInChildren<RotateToAim>();
            enemyHelper = GetComponent<EnemyHelper>();
        }
        //void Update()
        //{
        //    //GetInputs();
        //}

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.tag == "Player")
        //    {
        //        canSeePlayer = true;
        //    }
        //}
        //private void OnTriggerExit(Collider other)
        //{
        //    if (other.tag == "Player")
        //    {
        //        canSeePlayer = false;
        //    }
        //}

        #region StateBehaviourMethods

        // idle
        public void EnterIdle()
        {
            VSManager.Trace();
            enemyHelper.clearCurrentMovement();
        }
        public void Idle()
        {
            VSManager.Trace();
        }

        // chase
        public void EnterChase()
        {
            VSManager.Trace();
        }
        public void Chase()
        {
            VSManager.Trace();
        }

        // take cover
        public void TakeCover()
        {
            VSManager.Trace();
            enemyHelper.TakeCover();
        }
        public void ExitTakeCover()
        {
            VSManager.Trace();
            enemyHelper.clearCurrentMovement();
        }

        // attack
        //public void EnterAttack()
        //{
        //    VSManager.Trace();
        //    enemyHelper.CanShootPlayer();
        //}
        public void Attack()
        {
            VSManager.Trace();
            enemyHelper.Attack();
        }
        public void ExitAttack()
        {

            VSManager.Trace();
            enemyHelper.CancelLookAt();
        }

        // reload
        public void Reload()
        {
            VSManager.Trace();
            enemyHelper.ReloadGun();
        }

        // die
        public void EnterDie()
        {
            VSManager.Trace();
            enemyHelper.SetEnemyDead();
        }
        #endregion

        #region ConditionMethods

        public TransitionCondition DieTrigger()
        {
            VSManager.Trace();
            return new TransitionCondition(enemyHelper.healthManager.Health <= 0);
        }
        public TransitionCondition PlayerInSight()
        {
            VSManager.Trace();
            return new TransitionCondition(enemyHelper.canSeePlayer);
        }
        public TransitionCondition PlayerOutofSight()
        {
            VSManager.Trace();
            return new TransitionCondition(!enemyHelper.canSeePlayer);
        }
        public TransitionCondition Reloading()
        {
            VSManager.Trace();
            return new TransitionCondition(enemyHelper.isReloading);
        }
        public TransitionCondition NotReloading()
        {
            VSManager.Trace();
            return new TransitionCondition(!enemyHelper.isReloading);
        }
        public TransitionCondition GunLoaded()
        {
            VSManager.Trace();
            return new TransitionCondition(enemyHelper.gunLoaded);
        }
        public TransitionCondition GunEmpty()
        {
            VSManager.Trace();
            return new TransitionCondition(enemyHelper.isGunEmpty);
        }

        #endregion
    }
}