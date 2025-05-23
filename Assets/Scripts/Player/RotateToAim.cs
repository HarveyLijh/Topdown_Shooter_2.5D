using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateToAim : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] LayerMask mask;
    [SerializeField] AimTarget aimTarget;
    [SerializeField] Camera topCam;
    [SerializeField] Transform crosshairWorld;
    [SerializeField] Image crosshairUI;
    [SerializeField] float AimHeight;
    //[SerializeField] Transform armL;
    //[SerializeField] Transform armR;

    [HideInInspector]
    public bool shouldRotate;
    private EnemyHelper enemyHelper;
    private Transform playerTransform;
    private PlayerBehavior playerBehavior;
    private enum AimTarget
    {
        PLAYER,
        MOUSE
    }
    void Awake()
    {
        playerTransform = GameObject.Find("PlayerObj").transform;
        playerBehavior = playerTransform.GetComponent<PlayerBehavior>();
        if (aimTarget == AimTarget.PLAYER)
        {
            enemyHelper = transform.parent.gameObject.GetComponent<EnemyHelper>();
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (shouldRotate && aimTarget == AimTarget.PLAYER)
        {
            Vector3 target = playerTransform.position;
            Vector3 direction = target - transform.position;
            if (enemyHelper != null && enemyHelper.enhancedAccuracy)
            {
                if (playerBehavior.InterceptionDirection(
                    player: new Vector2(target.x, target.z),
                    gunner: new Vector2(transform.position.x, transform.position.z),
                    v_player: new Vector2(playerBehavior.movementOffSet.x, playerBehavior.movementOffSet.z),
                    s_gunner: enemyHelper.bulletSpeed,
                    result: out Vector2 predictedDirection))
                {
                    direction = new Vector3(predictedDirection.x, playerBehavior.originalY, predictedDirection.y);

                }
            }
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, rotation, 0), rotationSpeed * 100 * Time.deltaTime);

        }
        else if (aimTarget == AimTarget.MOUSE)
        {
            Ray ray = topCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, mask))
            {
                Vector3 target = hit.point;
                Vector3 direction = target - transform.position;
                float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, rotation, 0), rotationSpeed * 100 * Time.deltaTime);
                //armL.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, rotation, 0), rotationSpeed * 100 * Time.deltaTime);
                //armR.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, rotation, 0), rotationSpeed * 100 * Time.deltaTime);

                //set crosshair
                crosshairWorld.position = new Vector3(target.x, crosshairWorld.position.y, target.z);
                if (crosshairUI)
                {
                    Vector3 screenPos = topCam.WorldToScreenPoint(crosshairWorld.position);
                    crosshairUI.GetComponent<RectTransform>().anchoredPosition = screenPos;

                }
            }

        }
    }

    //void flipWeapon(float angle)
    //{
    //    Vector3 localScale = Vector3.one;
    //    if (angle >= 0)
    //    {
    //        localScale.x = 1;
    //    }
    //    else
    //    {
    //        localScale.x = -1;
    //    }
    //    weapon.localScale = localScale;

    //}
}
