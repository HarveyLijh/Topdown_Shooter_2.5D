using System.Collections;
using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    // BulletType

    [SerializeField] GameObject pfBullet;
    //Gun stats
    public int damage;
    public float timeBetweenShooting, speed, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    public enum nonPlayerInput
    {
        FIRE,
        RELOAD,
        NOTHING
    }
    //events
    public delegate void FullyLoadedEvent();
    public FullyLoadedEvent OnGunLoaded;
    public delegate void GunEmptyEvent();
    public GunEmptyEvent OnGunEmpty;

    //bools
    [SerializeField] bool isPlayer = false;
    bool shooting, readyToShoot, reloading;

    //Reference
    private GameObject gunner;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public BulletPool bulletPool;

    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI text;

    private void Start()
    {

        bulletPool = BulletPool.Instance;
        Debug.Log(bulletPool);

        bulletsLeft = magazineSize;

        //Debug.Log(gameObject.GetComponentInParent<GameObject>());
        gunner = gameObject.transform.parent.gameObject;
        readyToShoot = true;

        if (isPlayer)
        {
            text = GameObject.Find("bulletNumText").GetComponent<TextMeshProUGUI>();

        }
    }
    private void Update()
    {
        if (isPlayer)
        {
            PlayerInput();

            //SetText
            text.SetText(bulletsLeft + " / " + magazineSize);
        }

    }
    public void NonPlayerInput(nonPlayerInput input)
    {
        if (allowButtonHold) shooting = (input == nonPlayerInput.FIRE);
        else shooting = (input == nonPlayerInput.FIRE);

        if ((input == nonPlayerInput.RELOAD) && bulletsLeft < magazineSize && !reloading) Reload();
        //if (bulletsLeft == 0 && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }

    }
    private void PlayerInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread) / 10;
        float z = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 shootDir = gunner.transform.forward + new Vector3(x, y, z);

        //RayCast
        //if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        //{
        //    Debug.Log(rayHit.collider.name);

        //    if (rayHit.collider.CompareTag("Enemy"))
        //        rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);
        //}


        
        //Graphics
        Bullet bullet = bulletPool.GetPool().Get();
        bullet.SetUp(attackPoint.position, attackPoint.rotation, damage, shootDir, range, speed, pfBullet);
        bullet.InitDestory(KillBullet);
        //Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        //Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot--;
        // let gunner know gun is empty
        if (bulletsLeft == 0)
        {
            OnGunEmpty?.Invoke();
        }

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }
    private void KillBullet(Bullet bullet)
    {
        if (bullet.isActiveAndEnabled)
        {
            bulletPool.GetPool().Release(bullet);
        }
    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        OnGunLoaded?.Invoke();
    }
}
