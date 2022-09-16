using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private CharacterController controller;
    public float playerSpeed = 2.0f;
    [HideInInspector]
    public float originalY;
    [HideInInspector]
    public Vector3 movementOffSet;
    public Transform sphere;
    [Range(1, 20)]
    public float maga;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        originalY = transform.position.y;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerTakeDamage(20);
        }
        movementOffSet = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movementOffSet *= Time.deltaTime * playerSpeed;
        // always keep player's position y same
        if (transform.position.y != originalY)
        {
            movementOffSet.y = (originalY - transform.position.y);
        }
        controller.Move(movementOffSet);
    }
    public bool InterceptionDirection(Vector2 player, Vector2 gunner, Vector2 v_player, float s_gunner, out Vector2 result)
    {
        Vector2 playerToGunner = player - gunner;
        float d_playerToGunner = playerToGunner.magnitude;
        float alpha = Vector2.Angle(playerToGunner, v_player);
        float s_player = playerSpeed;
        float temp = s_player / s_gunner;
        if (Utils.SolveQuadratic(
            a: 1 - temp * temp,
            b: 2 * temp * d_playerToGunner * Mathf.Cos(alpha),
            c: -(d_playerToGunner * d_playerToGunner),
            out float root1,
            out float root2) == 0)
        {
            result = Vector3.zero;
            return false;

        }
        float d_player = Mathf.Max(a: root1, b: root2);
        float t = d_player / s_gunner * maga;
        Vector2 futureIntercep = player + v_player * t;
        result = (futureIntercep - gunner);
        return true;
    }
    public void playerTakeDamage(float dmg)
    {
        GameManager.Instance.playerHealth.Damage(dmg);
    }

    public void playerHeal(float healing)
    {
        GameManager.Instance.playerHealth.Heal(healing);
    }
}
