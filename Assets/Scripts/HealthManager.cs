using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class HealthManager
{
    float _currentHealth;
    float _currentMaxHealth;

    Slider _slider;
    Gradient _gradient;
    Image _fill;

    public float Health
    {
        get
        {
            return _currentHealth;
        }

        set
        {
            _currentHealth = value;
        }
    }

    public float MaxHealth
    {
        get
        {
            return _currentMaxHealth;
        }

        set
        {
            _currentMaxHealth = value;
        }
    }

    // Constructor
    public HealthManager(float health, float maxHealth, Slider slider = null, Gradient gradient = null, Image fill = null)
    {
        _currentHealth = health;
        _currentMaxHealth = maxHealth;
        if (slider != null)
        {
            _slider = slider;
        }
        if (gradient != null)
        {
            _gradient = gradient;
        }
        if (fill != null)
        {
            _fill = fill;
        }
    }

    public void Damage(float damgeAmmount)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damgeAmmount;
        }
        SetHealthBar(_currentHealth);
    }

    private void SetHealthBar(float health)
    {
        _slider.value = health;
        _fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    public void Heal(float healAmmount)
    {
        if (_currentHealth < _currentMaxHealth)
        {
            _currentHealth += healAmmount;
        }
        if (_currentHealth > _currentMaxHealth)
        {
            _currentHealth = _currentMaxHealth;
        }
        SetHealthBar(_currentHealth);
    }
}