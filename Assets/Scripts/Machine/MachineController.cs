using System;
using System.Collections;
using Deforestation.Machine.Weapon;
using Photon.Pun;
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
        // Evento para sincronizar el salto de la maquina
        public Action OnSyncWakeUp;
        public Action OnSyncExitMachine;
        public event Action<string> OnAnimationSync;
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
        private PhotonView _photonView;
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
            
            StartCoroutine(SubscribeWhenReady());
            _movement.enabled = false;
            
        }

		// Update is called once per frame
		void Update()
		{
            FallMachine();

        }

        #endregion

        #region Public Methods
        IEnumerator SubscribeWhenReady()
        {
            while (GameController.Instance.InputSystem == null)
                yield return null; // espera al siguiente frame

            GameController.Instance.InputSystem._onExitMachine += PlayerExitMachine;
            GameController.Instance.InputSystem._onRunMachine += StartRunning;
            _photonView = GetComponent<PhotonView>();
        }
        public void StopDriving()
		{
            if (_anim != null)
            {
                StopMoving();
                OnMachineDriveChange?.Invoke(false);
                
            }
        }

		public void StartDriving(bool machineMode)
		{
            if (_anim != null)
            {
                 enabled = machineMode;
			    _movement.enabled = machineMode;
			    WeaponController.enabled = machineMode;

			    _anim.SetTrigger("WakeUp");
                OnAnimationSync?.Invoke("WakeUp");
                _anim.SetBool("Move", machineMode);
			    OnMachineDriveChange?.Invoke(true);
            }
        }
        public void StartRunning()
        {
            if (_anim != null)
            {
                _anim.SetTrigger("Run");
                OnAnimationSync?.Invoke("Run");
            }
        }
		public void StopMoving()
		{
            if (_anim != null)
            {
                _movement.enabled = false;
                _anim.SetBool("Move", false);
                SyncBool("Move", false); // sincroniza el cambio
            }
        }
        void SyncBool(string paramName, bool value)
        {
           if (_photonView.IsMine)
                _photonView.RPC("RPC_SetBool", RpcTarget.All, paramName, value);
        }

        [PunRPC]
        void RPC_SetBool(string paramName, bool value)
        {
            _anim.SetBool(paramName, value);
        }
        public void JumpMachine()
        {
            if (_anim != null)
            {
                _anim.SetTrigger("Jump");
                OnAnimationSync?.Invoke("Jump");
            }
        }

        public void PlayerExitMachine()
        {
            if (_photonView != null && !_photonView.IsMine)
                return;
            if (_movement != null) {
                StartCoroutine(WaitMachineModeChange());
                GameController.Instance.MachineController.StopDriving();
                OnSyncExitMachine?.Invoke();
                _movement.driving = false;
               // _photonView.TransferOwnership(0);
            }
        }
        private IEnumerator WaitMachineModeChange()
        {
            yield return new WaitForSeconds(8f); // Espera para realizar la animacion
            GameController.Instance.TeleportPlayer(_machineSpawn.position);
            GameController.Instance.MachineMode(false);

        }
        private void FallMachine()
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

