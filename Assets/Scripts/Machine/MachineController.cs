using UnityEngine;
using System;
using Deforestation.Machine.Weapon;
using System.Collections;

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
        [SerializeField] private Transform _targetSpawn;
        [SerializeField] private Transform _playerTransform;

        #endregion

        #region Fields
        private HealthSystem _health;
		private MachineMovement _movement;
		private Animator _anim;

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
			_movement.enabled = false;
		}

		// Update is called once per frame
		void Update()
		{
		}		

		#endregion

		#region Public Methods
		public void StopDriving()
		{
            StopMoving();
            StartCoroutine(WaitMachineModeChange());
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
            _playerTransform.position = _targetSpawn.position;
        }
        #endregion

        #region Private Methods
        private IEnumerator WaitMachineModeChange()
        {
            yield return new WaitForSeconds(7f); // Espera para realizar la animacion
            GameController.Instance.MachineMode(false);

        }
        #endregion
    }

}