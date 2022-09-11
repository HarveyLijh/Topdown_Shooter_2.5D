using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] float playerSpeed = 2.0f;
    private float originalY;

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
        Vector3 movementOffSet = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movementOffSet *= Time.deltaTime * playerSpeed;
        if (transform.position.y != originalY)
        {
            movementOffSet.y = (originalY - transform.position.y);
        }
        controller.Move(movementOffSet);
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
