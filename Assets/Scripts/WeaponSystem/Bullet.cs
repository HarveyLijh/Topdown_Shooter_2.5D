using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

public class Bullet : MonoBehaviour
{
    // bullet features
    private bool _leaveTrailAfterHit;
    private float _range;
    private float _speed;
    private float _damage;
    private Transform _trail;
    private Transform _body;
    private Transform _originalTransform;

    private Action<Bullet> _destoryAction;

    // physics
    private float _penetratePower;
    private Rigidbody _rigidbody;

    // hit effects
    private ParticleSystem _hitEffect;
    private ParticleSystem _hitEffect_noDecal;
    private float _bulletForce;
    private AudioSource _hitSound;

    //Ray _ray;
    //RaycastHit _hitInfo;
    private int _ignore_layerMask = (1 << 6) | (1 << 15) | (1 << 16) | (1 << 18);

    // need reset when active if reused
    private float _travelDistance;
    private Vector3 prevPos;

    private void Awake()
    {
        _trail = Utils.FindComponentInChildWithTag<Transform>(gameObject,"bullet_trail");
        _body = Utils.FindComponentInChildWithTag<Transform>(gameObject, "bullet_body");
      
        _rigidbody = GetComponent<Rigidbody>();
        _originalTransform = transform;
    }
    private void OnDisable()
    {
        _travelDistance = 0;
        _rigidbody.velocity = Vector3.zero;
        transform.position = _originalTransform.position;
        transform.rotation = _originalTransform.rotation;
    }
    public void InitDestory(Action<Bullet> destoryAction)
    {
        _destoryAction = destoryAction;
    }

    public void SetUp(Vector3 startPosition, Quaternion startRotation, float damage, Vector3 shootDir, float range, float speed, GameObject pfBullet, float penetratePower = 0)
    {
        //Debug.Log(_trail)
        // copy the trail and body of the prefab bullet
        Transform newtrail = Utils.FindComponentInChildWithTag<Transform>(pfBullet, "bullet_trail");
        Transform newbody = Utils.FindComponentInChildWithTag<Transform>(pfBullet, "bullet_body");
        _trail.GetComponent<SpriteRenderer>().color = newtrail.GetComponent<SpriteRenderer>().color;
        _body.GetComponent<SpriteRenderer>().color = newbody.GetComponent<SpriteRenderer>().color;
        _body.GetComponent<SpriteGlowEffect>().GlowColor = newbody.GetComponent<SpriteGlowEffect>().GlowColor;

        transform.position = startPosition;
        transform.rotation = startRotation;
        // rotate to face forward
        transform.Rotate(shootDir.x, shootDir.y, shootDir.z, Space.Self);
        //transform.Rotate(bulletSpread.x, bulletSpread.y, bulletSpread.z, Space.Self);

        //_ray.origin = transform.position;
        //_ray.direction = transform.up;
        //if (Physics.Raycast(_ray, out _hitInfo, range + 5, ~_layerMask))
        //{

        //}
        _damage = damage;
        //_hitSound = hitSound;
        _rigidbody.AddForce(transform.up * speed, ForceMode.Impulse);
        //_leaveTrailAfterHit = leaveTrailAfterHit;
        _range = range;
        _speed = speed;
        //_hitEffect = hitEffect;
        //_hitEffect_noDecal = hitEffect_noDecal;
        //_bulletForce = bulletForce;
        prevPos = transform.position;
        _penetratePower = penetratePower == 0 ? 0 : penetratePower - 1;

    }

    void Update()
    {

        _travelDistance += Time.deltaTime * _speed;
        //transform.Translate(0.0f, _travelDistance, 0.0f);
        Ray shortRay = new Ray(prevPos, (transform.position - prevPos).normalized);
        RaycastHit[] hits = Physics.RaycastAll(shortRay, (transform.position - prevPos).magnitude);//, ~_ignore_layerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            //Debug.Log(hits[i].collider.gameObject.name);
            if (_penetratePower == 0)
            {
                //hits[i].Damage()
                afterHit(hits[0], shortRay);
            }
            else if (_penetratePower > 0)
            {
                _penetratePower -= 1;
                afterHit(hits[i], shortRay);
            }
            else if (_penetratePower == -100)
            {
                afterHit(hits[i], shortRay);
            }
        }

        prevPos = transform.position;
        // auto destory after facing range
        if (_travelDistance >= _range)
        {
            DestoryBullet();
        }

    }
    public GameObject FindParentWithTag(Transform t, string tag)
    {
        //Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.parent.tag == tag)
            {
                return t.parent.gameObject;
            }
            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }

    private void afterHit(RaycastHit hit, Ray ray)
    {
        if (hit.collider.isTrigger)
        {
            return;
        }
        // if hit an enemy, ignore trigger collider 
        if (hit.collider.gameObject.tag == "Enemy" && hit.collider.TryGetComponent<EnemyHelper>(out EnemyHelper enemyHelper))
        {
            enemyHelper.healthManager.Damage(_damage);
            DestoryBullet();
        }
        // bullet disappear after hit Obstacles
        if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Obstacles")
        {
            DestoryBullet();

        }
        // if hit player, ignore trigger collider 
        if ( hit.collider.gameObject.tag == "Player" && hit.collider.TryGetComponent<PlayerBehavior>(out PlayerBehavior playerBehavior))
        {
            playerBehavior.playerTakeDamage(_damage);
            DestoryBullet();

        }
        //GameObject EnemyBrain = FindParentWithTag(hit.collider.GetComponent<Transform>(), "EnemyBrain");
        //GameObject PlayerBrain = FindParentWithTag(hit.collider.GetComponent<Transform>(), "Player");
        //if (EnemyBrain != null)
        //{
        //    //EnemyBrain.GetComponent<HealthManager>().TakeDamage(_damage);
        //}
        //if (PlayerBrain != null)
        //{
        //    //PlayerBrain.GetComponent<HealthManager>().TakeDamage(_damage);
        //}
        //ParticleSystem presentingEffect;
        ////if (GetComponent<Explosive>())
        ////{
        ////    GetComponent<Explosive>().explode();
        ////}
        ////else
        ////{
        //    if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
        //    {
        //        rb.AddForceAtPosition(ray.direction * _bulletForce, hit.point, ForceMode.Impulse);
        //    }
        //    //if (hit.collider.TryGetComponent<HitManager>(out HitManager hitManager))
        //    //{
        //    //    hitManager.hit(1f);
        //    //}
        //    if (hit.transform.gameObject.isStatic)
        //    {
        //        presentingEffect = _hitEffect;

        //    }
        //    else
        //    {
        //        presentingEffect = _hitEffect_noDecal;
        //    }

        //    presentingEffect.transform.position = hit.point;
        //    presentingEffect.transform.forward = hit.normal;
        //    presentingEffect.Emit(1);
        //    if (_hitSound != null) _hitSound.Play();
        ////}
        ////}
    }
    private void DestoryBullet()
    {
        //if (_leaveTrailAfterHit)
        //{
        //    Destroy(_rigidbody);
        //    foreach (Transform child in transform)
        //    {
        //        if (child.tag == "bulletBody")
        //        {
        //            Destroy(child.gameObject);
        //        }
        //        if (child.tag == "bulletTrail")
        //        {
        //            //Destroy(gameObject, 5f);
        //            _destoryAction(this);
        //        }
        //    }
        //}
        //else
        //{
            _destoryAction(this);
        //}
    }
}
