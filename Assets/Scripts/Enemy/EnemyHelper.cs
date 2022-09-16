using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Micosmo.SensorToolkit;

public class EnemyHelper : MonoBehaviour
{
    private Action<EnemyHelper> _destoryAction;

    private Collider sight;
    private CharacterController characterController;
    private RotateToAim rotateController;

    [SerializeField] Slider health_slider;
    [SerializeField] Gradient health_gradient;
    [SerializeField] Image health_fill;

    [SerializeField] GunSystem weapon;
    [SerializeField] float MaxHealth = 100f;

    [SerializeField] Transform destination;
    [SerializeField] float sightRadius = 20f;

    [Range(0, 20)]
    public float accuracyOffset = 1.0f;
    public bool enhancedAccuracy;

    [HideInInspector]
    public float bulletSpeed;
    [SerializeField] int numOfHideOutsToSelect = 5;

    // hide parameters
    [HideInInspector]
    public Transform Target;
    public LayerMask HidableLayers;
    public Sensor LineOfSightSensor;
    public NavMeshAgent navMeshAgent;
    [Range(-1, 1)]
    [Tooltip("Lower is a better hiding spot")]
    public float HideSensitivity = 0;
    [Range(1, 20)]
    public float MinPlayerDistance = 5f;
    [Range(0, 5f)]
    public float MinObstacleHeight = 1.25f;
    [Range(0.01f, 1f)]
    public float UpdateFrequency = 0.25f;

    private Coroutine MovementCoroutine;
    private Collider[] Colliders;

    // indicators for enemy event
    //[HideInInspector]
    //public bool shouldTakeCover;
    //[HideInInspector]
    public bool canSeePlayer;

    //[HideInInspector]
    public bool gunLoaded;
    //[HideInInspector]
    public bool isReloading;
    //[HideInInspector]
    public bool isGunEmpty;

    // health
    public HealthManager healthManager;
    public bool enemyDead;

    private void OnEnable()
    {
        enemyDead = false;
        rotateController.gameObject.SetActive(true);
    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        rotateController = GetComponentInChildren<RotateToAim>();
        Colliders = new Collider[numOfHideOutsToSelect];
        // health bar set up
        health_slider.maxValue = MaxHealth;
        health_slider.value = MaxHealth;
        health_fill.color = health_gradient.Evaluate(MaxHealth);
        healthManager = new HealthManager(MaxHealth, MaxHealth, health_slider, health_gradient, health_fill);

        navMeshAgent = GetComponent<NavMeshAgent>();

        // take cover setup
        //LineOfSightSensor.OnDetected += HandleGainSight;
        //LineOfSightSensor.OnLostDetection = HandleLoseSight;
        // weapon notification setup
        weapon.OnGunLoaded += HandleGunLoaded;
        weapon.OnGunEmpty += HandleGunEmpty;
        bulletSpeed = weapon.speed;
        weapon.spread *= accuracyOffset;
        gunLoaded = true;

    }

    private void HandleGunLoaded()
    {
        gunLoaded = true;
        isReloading = false;
        isGunEmpty = false;
    }
    private void HandleGunEmpty()
    {

        gunLoaded = false;
        isReloading = false;
        isGunEmpty = true;
    }

    public void ReloadGun()
    {
        gunLoaded = false;
        isReloading = true;
        weapon.NonPlayerInput(GunSystem.nonPlayerInput.RELOAD);
    }

    void Update()
    {

    }

    public void CancelLookAt()
    {
        rotateController.shouldRotate = false;
    }
    public void Attack()
    {
        rotateController.shouldRotate = true;
        weapon.NonPlayerInput(GunSystem.nonPlayerInput.FIRE);
    }
    public void Follow(Transform followee)
    {
        navMeshAgent.destination = new Vector3(followee.position.x, transform.position.y, followee.position.z);
    }
    public void TakeCover()
    {
        this.RestartCoroutine(Hide(), ref MovementCoroutine);
    }
    public void clearCurrentMovement()
    {
        this.TryStopCoroutine(ref MovementCoroutine);
    }

    public void SetEnemyDead()
    {
        enemyDead = true;
        DestroyEnemy();
        rotateController.shouldRotate = false;
    }
    public void enemyTakeDamage(int dmg)
    {
        healthManager.Damage(dmg);
    }
    public void enemyHeal(int healing)
    {
        healthManager.Heal(healing);
    }
    public void DestroyEnemy()
    {
        navMeshAgent.isStopped = true;
        if (_destoryAction != null)
        {
            _destoryAction(this);
        }
        else
        {
            Destroy(gameObject, 2);
        }
    }

    public void HandleGainSight(GameObject target, Sensor sensor)
    {
        clearCurrentMovement();
        canSeePlayer = true;
        Target = target.transform;
    }

    public void HandleLoseSight(GameObject target, Sensor sensor)
    {

        clearCurrentMovement();
        canSeePlayer = false;
        Target = null;
    }

    private IEnumerator Hide()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateFrequency);
        while (true)
        {
            for (int i = 0; i < Colliders.Length; i++)
            {
                Colliders[i] = null;
            }

            int hits = Physics.OverlapSphereNonAlloc(navMeshAgent.transform.position, sightRadius, Colliders, HidableLayers);

            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                if (Vector3.Distance(Colliders[i].transform.position, Target.position) < MinPlayerDistance || Colliders[i].bounds.size.y < MinObstacleHeight)
                {
                    Colliders[i] = null;
                    hitReduction++;
                }
            }
            hits -= hitReduction;

            System.Array.Sort(Colliders, ColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit, navMeshAgent.height * 2, navMeshAgent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, navMeshAgent.areaMask))
                    {
                        Debug.LogError($"Unable to find edge close to {hit.position}");
                    }

                    if (Vector3.Dot(hit.normal, (Target.position - hit.position).normalized) < HideSensitivity)
                    {
                        navMeshAgent.SetDestination(hit.position);
                        break;
                    }
                    else
                    {
                        // Since the previous spot wasn't facing "away" enough from teh target, we'll try on the other side of the object
                        if (NavMesh.SamplePosition(Colliders[i].transform.position - (Target.position - hit.position).normalized * 2, out NavMeshHit hit2, navMeshAgent.height * 2, navMeshAgent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, navMeshAgent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                            }

                            if (Vector3.Dot(hit2.normal, (Target.position - hit2.position).normalized) < HideSensitivity)
                            {
                                navMeshAgent.SetDestination(hit2.position);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Unable to find NavMesh near object {Colliders[i].name} at {Colliders[i].transform.position}");
                }
            }
            yield return Wait;
        }
    }

    public int ColliderArraySortComparer(Collider A, Collider B)
    {
        if (A == null && B != null)
        {
            return 1;
        }
        else if (A != null && B == null)
        {
            return -1;
        }
        else if (A == null && B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(navMeshAgent.transform.position, A.transform.position).CompareTo(Vector3.Distance(navMeshAgent.transform.position, B.transform.position));
        }
    }

}