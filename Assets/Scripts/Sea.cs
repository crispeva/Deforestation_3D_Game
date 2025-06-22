using System;
using System.Collections;
using Deforestation;
using UnityEngine;

public class Sea : MonoBehaviour
{
    #region Properties
    private Coroutine _damageCoroutine;
    #endregion

    #region Fields
    [SerializeField]
    private float _maxDamage = 100f;
    [SerializeField]
    private float _initialDamage = 10f;
    [SerializeField]
    private float _damageIncreasePerSecond = 5f;
    private Transform _seaTransform;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        _seaTransform=GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion

    #region Public Methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Machine"))
        {
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                _damageCoroutine = StartCoroutine(ApplyIncreasingDamage(health));
            }

        }
        
    }
    private void InstaKill(HealthSystem health) {

            if (health != null)
            {
            health.TakeDamage(999);
            }
    
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")|| other.CompareTag("Machine") && _damageCoroutine != null)
        {
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = null;
        }
        HealthSystem health = other.GetComponent<HealthSystem>();
        if (_seaTransform.position.y > other.transform.position.y)
            InstaKill(health);
    }

    private IEnumerator ApplyIncreasingDamage(HealthSystem health)
    {
        float currentDamage = _initialDamage;
        while (true)
        {
            health.TakeDamage(Mathf.Min(currentDamage, _maxDamage));
            yield return new WaitForSeconds(1f);
            currentDamage += _damageIncreasePerSecond;
        }
    }
    #endregion

    #region Private Methods
    #endregion
}
