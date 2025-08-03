using System;
using UnityEngine;
using UnityEngine.AI;

namespace Deforestation.Dinosaurus
{

	public class Dinosaur : MonoBehaviour
	{
		#region Fields
        protected enum DinoState { Idle, Chase, Attack }
        private DinoState _state = DinoState.Idle;
        protected Animator _anim;
		protected NavMeshAgent _agent;
		protected HealthSystem _health;
       protected virtual Vector3 _targetPosition => GameController.Instance.CharacterController.transform.position;

        [SerializeField] protected float _distanceDetection = 50;
        [SerializeField] protected float _attackDistance = 10;
        [SerializeField] protected float _attackTime = 2;
        [SerializeField] protected float _attackDamage = 5;
        [SerializeField] protected float _radiusMovement = 100f;

        protected float _attackColdDown;
        protected bool _chase;
        protected bool _attack;

        #endregion

        #region Properties
        public HealthSystem Health => _health;
        #endregion

        #region Unity Callbacks	
        protected virtual void Awake()
		{
			_health = GetComponent<HealthSystem>();
			_anim = GetComponent<Animator>();
			_agent = GetComponent<NavMeshAgent>();
			_health.OnDeath += Die;
		}
        protected virtual void Start()
        {
            _health.OnHealthChanged += Damage;
            _attackColdDown = _attackTime;
        }
        protected virtual void Update()
        {
            UpdateState();

            switch (_state)
            {
                case DinoState.Idle:
                    IdleAnimator();
                    break;
                case DinoState.Chase:
                    ChaseAnimator();
                    break;
                case DinoState.Attack:
                    AttackAnimator();
                    break;
            }

        }
        #endregion

        #region Private Methods
        //Animations
        protected virtual void Die()
        {
            _anim.SetTrigger("Die");
            Destroy(_agent);
            Destroy(this);
        }
        protected void IdleAnimator()
        {
            _anim.SetBool("Run", false);
            _anim.SetBool("Attack", false);
            _chase = false;
            _attack = false;
            _agent.isStopped = true;

        }
        protected void ChaseAnimator()
        {
            _anim.SetBool("Run", true);
            _anim.SetBool("Attack", false);
            _agent.isStopped = false;
            _agent.SetDestination(_targetPosition);
            _chase = true;
            _attack = false;
        }

        protected void AttackAnimator()
        {
            _anim.SetBool("Run", false);
            _anim.SetBool("Attack", true);
            _agent.isStopped = true;
            _chase = false;
            _attack = true;
        }
        //Gizmos para visualizar distancias de detección y ataque
         protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _distanceDetection);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);
        }
        //Estado del dinosaurio
        protected void UpdateState()
        {
            // Si está muerto, no cambiar estado (opcional)
            if (_health == null || _health.CurrentHealth <= 0)
                return;

            //Chase a player
            if (!_chase && !_attack && Vector3.Distance(transform.position, _targetPosition) < _distanceDetection)
            {
                _state = DinoState.Chase;
                return;
            }
            //Chase
            if (_chase)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(_targetPosition, out hit, _attackDistance, 1))
                    _agent.SetDestination(hit.position);

            }
            //Attack
            if (_attack)
            {
                DinosaurAttack();

            }

            if (_chase && Vector3.Distance(transform.position, _targetPosition) < _attackDistance)
            {
                _state = DinoState.Attack;
                return;
            }
            if (_chase && Vector3.Distance(transform.position, _targetPosition) > _distanceDetection)
            {
                _state = DinoState.Idle;
                return;
            }

            if (_attack && Vector3.Distance(transform.position, _targetPosition) > _attackDistance)
            {
                _state = DinoState.Chase;
                return;
            }

            // Aquí puedes añadir más condiciones, por ejemplo:
            // - Si el dinosaurio está aturdido, _state = DinoState.Stunned;
            // - Si está huyendo, _state = DinoState.Flee;
        }
        protected void Damage(float health)
        {
            DinosaurFlight();
        }
        protected virtual void DinosaurAttack()
        {
            //Atack damage
            _attackColdDown -= Time.deltaTime;
            if (_attackColdDown <= 0)
            {
                _attackColdDown = _attackTime;
                GameController.Instance.HealthSystem.TakeDamage(_attackDamage);
            }
        }

        protected void DinosaurFlight()
        {
            //Huida 
            Vector3 destinoAleatorio = UnityEngine.Random.insideUnitSphere * _radiusMovement;
            destinoAleatorio += transform.position;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(destinoAleatorio, out hit, _radiusMovement, 1))
            {
                _agent.SetDestination(hit.position);
            }
            _anim.SetBool("Run", true);

            //Parada
            if (!_agent.pathPending)
            { // Asegura que el agente haya calculado el camino
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                { // Comprueba si la distancia restante es menor que la distancia de parada
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                    {
                        _anim.SetBool("Run", false);

                    }
                }
            }
        }
        #endregion

        #region Public Methods
        #endregion

    }

}