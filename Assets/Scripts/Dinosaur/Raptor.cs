using UnityEngine;
using UnityEngine.AI;

namespace Deforestation.Dinosaurus
{
	public class Raptor : Dinosaur
	{
        #region Fields
        [SerializeField] private float _distanceDetection = 50;
        [SerializeField] private float _attackDistance = 10;
        private Vector3 _playerPosition => GameController.Instance.CharacterController.transform.position;

        private bool _chase;
        private bool _attack;

        [SerializeField] private float _attackTime = 2;
        [SerializeField] private float _attackDamage = 5;
        [SerializeField] private float _positionRaptor;
        private float _attackColdDown;
        #endregion

        #region Properties
        #endregion

        #region Unity Callbacks	
        private void Awake()
        {
            //_health = GetComponent<HealthSystem>();
            _anim = GetComponentInChildren<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            _chase = true;
            _attack = true;
        }
        private void Start()
        {
            _attackColdDown = _attackTime;
        }
        private void Update()
        {
            //Chase a player
            if (!_chase && !_attack && Vector3.Distance(transform.position, _playerPosition) < _distanceDetection)
            {
                ChasePlayer();
                return;
            }

            //Chase
            if (_chase)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(_playerPosition, out hit, _attackDistance, 1))
                    _agent.SetDestination(hit.position);

            }

            if (_chase && Vector3.Distance(transform.position, _playerPosition) < _attackDistance)
            {
                Attack();
                return;
            }
            if (_chase && Vector3.Distance(transform.position, _playerPosition) > _distanceDetection)
            {
                Idle();
                return;
            }

            //Attack
            if (_attack)
            {
                //Atack damage
                _attackColdDown -= Time.deltaTime;
                if (_attackColdDown <= 0)
                {
                    _attackColdDown = _attackTime;
                    GameController.Instance.HealthSystem.TakeDamage(_attackDamage);
                }

            }
            if (_attack && Vector3.Distance(transform.position, _playerPosition) > _attackDistance)
            {
                ChasePlayer();
                return;
            }

        }


        #endregion

        #region Private Methods
        private void Idle()
        {
            _anim.SetBool("Run", false);
            _anim.SetBool("Attack", false);
            _chase = false;
            _attack = false;
            _agent.isStopped = true;

        }

        private void ChasePlayer()
        {
            _anim.SetBool("Run", true);
            _anim.SetBool("Attack", false);
            _agent.isStopped = false;
            _agent.SetDestination(_playerPosition);
            _chase = true;
            _attack = false;
        }

        private void Attack()
        {
            _anim.SetBool("Run", false);
            _anim.SetBool("Attack", true);
            _agent.isStopped = true;
            _chase = false;
            _attack = true;
        }
        #endregion


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _distanceDetection);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);
        }
    }
}
