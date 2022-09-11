using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;

public class BulletPool : MonoBehaviour
{
    [SerializeField] Bullet basicBullet;
    [SerializeField] int defaultCapacity = 50;
    [SerializeField] int maxSize = 100;

    private static BulletPool _instance;
    private static ObjectPool<Bullet> pool;

    public static BulletPool Instance { get { return _instance; } }
    public TextMeshProUGUI text;



    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
        Debug.Log(_instance);
    }

    void Start()
    {
        pool = new ObjectPool<Bullet>(() =>
        {
            return Instantiate(basicBullet);

        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet.gameObject);
        }, false, defaultCapacity, maxSize);
    }

    private void Update()
    {
        text.SetText("Active: "+pool.CountActive+"\nInactive: "+pool.CountInactive);

    }
    public ObjectPool<Bullet> GetPool()
    {
        return pool;
    }


}
