using System;
using System.Collections;
using Deforestation.Machine.Weapon;
using UnityEngine;

namespace Deforestation.Machine
{
	[RequireComponent (typeof(HealthSystem))]
	public class MachineController : MonoBehaviour
	{
		#region Properties
		public HealthSystem HealthSystem => _health;
		public WeaponController WeaponController;
		public Action<bool> OnMachineDriveChange;
		public Action OnMachineWalking;
        [Header("Spawn_Player")]
        [SerializeField] Transform _machineSpawn;


        #endregion

        #region Fields
        private HealthSystem _health;
		private MachineMovement _movement;
		private Animator _anim;
        [Tooltip("Ángulo máximo permitido antes de considerar que ha volcado")]
        private float maxTiltAngle = 60f;

        [Tooltip("Tiempo que debe estar volcado antes de activar muerte")]
        private float tiempoParaMorir = 2f;

        private float tiempoVolcado = 0f;
        #endregion

        #region Unity Callbacks
        private void Awake()
		{
			_health = GetComponent<HealthSystem>();
			_movement = GetComponent<MachineMovement>();
			_anim = GetComponent<Animator>();

		}
		// Start is called before the first frame update
		void Start()
		{
            if (OnMachineDriveChange != null)
            {
                GameController.Instance.InputSystem._onExitMachine += PlayerExitMachine;
                _movement.enabled = false;
            }

		}

		// Update is called once per frame
		void Update()
		{
            FallMachine();

        }		

		#endregion

		#region Public Methods
		public void StopDriving()
		{
            StopMoving();
            
            OnMachineDriveChange?.Invoke(false);

        }

		public void StartDriving(bool machineMode)
		{
			enabled = machineMode;
			_movement.enabled = machineMode;
			_anim.SetTrigger("WakeUp");
			_anim.SetBool("Move", machineMode);
			OnMachineDriveChange?.Invoke(true);

        }

		public void StopMoving()
		{
			_movement.enabled = false;
			_anim.SetBool("Move", false);

        }
        public void JumpMachine()
        {
            _anim.SetTrigger("Jump");
        }
        public void PlayerExitMachine()
        {
            if (_movement.enabled == true)
			{
                StartCoroutine(WaitMachineModeChange());
				GameController.Instance.MachineController.StopDriving();
				_movement.driving = false;

            }
			else
			{
                GameController.Instance.TeleportPlayer(_machineSpawn.position);
                GameController.Instance.MachineMode(false);
            }

        }
        private IEnumerator WaitMachineModeChange()
        {
            yield return new WaitForSeconds(8f); // Espera para realizar la animacion
            GameController.Instance.TeleportPlayer(_machineSpawn.position);
            GameController.Instance.MachineMode(false);

        }private void FallMachine()
		{
            // Calcula el ángulo entre el 'arriba' del objeto y el 'arriba' global
            float angulo = Vector3.Angle(transform.up, Vector3.up);

            if (angulo > maxTiltAngle)
            {
                // Está inclinado más de lo permitido
                tiempoVolcado += Time.deltaTime;

                if (tiempoVolcado >= tiempoParaMorir)
                {
                    // Aquí pones la lógica de "pantalla de muerte"
                    Debug.Log("¡La máquina ha volcado! Mostrar pantalla de muerte.");
                    // Ejemplo:
                   _health.TakeDamage(9999);
                }
            }
            else
            {
                // Se resetea el contador si vuelve a estar estable
                tiempoVolcado = 0f;
            }
        }
    }

        #endregion

        #region Private Methods

        #endregion
    }

