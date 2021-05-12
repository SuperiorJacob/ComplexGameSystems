using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Demo
{
    public class DemoDamageable : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int _health = 100;
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private bool _isDead;

        public void TakeDamage(int a_damage)
        {
            if (_isDead) return;

            _health -= a_damage;

            if (_health < 0)
            {
                _isDead = true;
            }
        }

        public float GetHealthPercentile()
        {
            return _health / _maxHealth;
        }

        public void AddHealth(int a_health)
        {
            if (_isDead) return;

            _health = Mathf.Clamp(_health + a_health, 0, _maxHealth);
        }

        public int GetHealth()
        {
            return _health;
        }

        public void SetHealth(int a_health)
        {
            _health = Mathf.Clamp(a_health, 0, _maxHealth);
        }

        public void SetMaxHealth(int a_maxHealth)
        {
            _maxHealth = a_maxHealth > 0 ? a_maxHealth : 100;
        }
    }
}