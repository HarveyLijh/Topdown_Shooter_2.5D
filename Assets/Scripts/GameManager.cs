using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] float playerCurrentHealth = 100;
    [SerializeField] float playerMaxHealth = 100;

    // Health bar
    [SerializeField] Slider health_slider;
    [SerializeField] Gradient health_gradient;
    [SerializeField] Image health_fill;

    public HealthManager playerHealth;


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
    }

    private void Start()
    {
        Cursor.visible = false;
        health_slider.maxValue = playerMaxHealth;
        health_slider.value = playerMaxHealth;
        health_fill.color = health_gradient.Evaluate(playerMaxHealth);
        playerHealth = new HealthManager(playerCurrentHealth, playerMaxHealth, health_slider, health_gradient, health_fill);
    }
}
