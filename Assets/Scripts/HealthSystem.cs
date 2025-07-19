using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Deforestation
{

	public class HealthSystem : MonoBehaviour
	{
		public event Action<float> OnHealthChanged;
		public event Action OnDeath;

		[SerializeField]
		private float _maxHealth = 100f;
		public float CurrentHealth { get; set; }
		[SerializeField]private float delayDeath = 2f;
		private GameObject _livingBeing;
        private void Awake()
		{
			CurrentHealth = _maxHealth;

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
			if(gameObject.name== "PlayerFPS")
			{
                StartCoroutine(DelayedDeath(delayDeath));
                OnDeath?.Invoke();
            }
            // Aquí puedes añadir lógica adicional para la muerte, como destruir el objeto.

        }
        IEnumerator DelayedDeath(float delay)
        {
            yield return new WaitForSeconds(delay);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

}