using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Deforestation
{

	public class HealthSystem : MonoBehaviour
	{
		public event Action<float> OnHealthChanged;
		public event Action OnDeath;

		[SerializeField]
		private float _maxHealth = 100f;
		private bool _isdeath;
		public float CurrentHealth { get; set; }
		[SerializeField]private float delayDeath = 1f;
		
        private void Awake()
		{
			CurrentHealth = _maxHealth;
            _isdeath= false;
        }

		public void TakeDamage(float damage)
		{
			CurrentHealth -= damage;
			OnHealthChanged?.Invoke(CurrentHealth);

			if (CurrentHealth <= 0)
			{
				Die();
			}
		}

		public void Heal(float amount)
		{
			CurrentHealth += amount;
			CurrentHealth = Mathf.Min(CurrentHealth, _maxHealth);
			OnHealthChanged?.Invoke(CurrentHealth);
		}

		public void SetHealth(float value)
		{
			CurrentHealth = value;
			CurrentHealth = Mathf.Min(CurrentHealth, _maxHealth);
			OnHealthChanged?.Invoke(CurrentHealth);
		}

		private void Die()
		{
			if(gameObject.name== "PlayerFPS" & _isdeath==false)
			{
                _isdeath=true;

                StartCoroutine(DelayedDeath(delayDeath));
                OnDeath?.Invoke();
            }
			else
			{
				return;
            }

        }
        IEnumerator DelayedDeath(float delay)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            yield return new WaitForSeconds(delay);

        }
    }

}