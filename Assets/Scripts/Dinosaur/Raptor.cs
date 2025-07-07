using UnityEngine;
using UnityEngine.AI;

namespace Deforestation.Dinosaurus
{
	public class Raptor : Dinosaur
	{
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Unity Callbacks	
        protected override void Awake()
        {

            _health = GetComponentInChildren<HealthSystem>();
            _anim = GetComponentInChildren<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            Debug.Log("Animator en objeto: " + _anim?.gameObject.name); 
            if (_health != null)
                _health.OnDeath += Die;
        }



        #endregion

        #region Private Methods
        protected override void Die()
        {
            if (_anim != null)
                _anim.SetTrigger("Die");
            if (_agent != null)
                Destroy(_agent);
            Destroy(this);
        }
        #endregion
    }
}
