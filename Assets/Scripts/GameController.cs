using System;
using Cinemachine;
using Deforestation.Events;
using Deforestation.Interaction;
using Deforestation.Machine;
using Deforestation.Recolectables;
using Deforestation.UI;
using UnityEngine;

namespace Deforestation
{
	public class GameController : Singleton<GameController>
	{
		#region Properties
		public MachineController MachineController => _machine;
		public CharacterController CharacterController => _player;
		public Inventory Inventory => _inventory;
		public InteractionSystem InteractionSystem => _interactionSystem;
		public TreeTerrainController TerrainController => _terrainController;
		public HealthSystem HealthSystem => _playerHealth;
		public VillageEventTrigger VillageEvents => _villageEvent;
		public InputSystem InputSystem => _inputSystem;
		public MachineMovement MachineMovement => _machinemovement;
		public UIGameController UIGameController => _uiController;
		public WinEvent WinEvent => _winEvent;
        public Camera MainCamera;

		//Events
		public Action<bool> OnMachineModeChange;
		public Action OnEventVillage;

		public bool MachineModeOn
		{
			get
			{
				return _machineModeOn;
			}
			private set
			{
				_machineModeOn = value;
				OnMachineModeChange?.Invoke(_machineModeOn);
			}
		}
		#endregion

		#region Fields
		[Header("Player")]
		[SerializeField] protected CharacterController _player;
		[SerializeField] protected HealthSystem _playerHealth;
		[SerializeField] protected Inventory _inventory;
		[SerializeField] protected InteractionSystem _interactionSystem;
		[SerializeField] protected InputSystem _inputSystem;


        [Header("Camera")]
		[SerializeField] protected CinemachineVirtualCamera _virtualCamera;
		[SerializeField] protected Transform _playerFollow;
		[SerializeField] protected Transform _machineFollow;

		[Header("Machine")]
		[SerializeField] protected MachineController _machine;
        [SerializeField] protected MachineMovement _machinemovement;
        [Header("UI")]
		[SerializeField] protected UIGameController _uiController;
		[Header("Trees Terrain")]
		[SerializeField] protected TreeTerrainController _terrainController;
		[Header("Village")]
        [SerializeField] protected VillageEventTrigger _villageEvent;
		[SerializeField] protected GameObject _objetcsVillageEvent;
		[SerializeField] protected WinEvent _winEvent;
        private bool _machineModeOn;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
		{
			//UI Update
			_playerHealth.OnHealthChanged += _uiController.UpdatePlayerHealth;
			_machine.HealthSystem.OnHealthChanged += _uiController.UpdateMachineHealth;
			//Event
            _villageEvent.OnEventVillagePanel += ShowEventVillage;
			//Inputs
            MachineModeOn = false;
		}

		// Update is called once per frame
		void Update()
		{

        }
		#endregion

		#region Public Methods
		public void TeleportPlayer(Vector3 target)
		{
			_player.enabled = false;
			_player.transform.position = target;
			_player.enabled = true;
		}

		internal void MachineMode(bool machineMode)
		{
			MachineModeOn = machineMode;
			//Player
			_player.gameObject.SetActive(!machineMode);
			_player.enabled = !machineMode;

			//Cursor + UI
			if (machineMode)
			{
				//Start Driving
				if (Inventory.HasResource(RecolectableType.HyperCrystal))
					_machine.StartDriving(machineMode);

				_player.transform.parent = _machineFollow;
				_uiController.HideInteraction();
				Cursor.lockState = CursorLockMode.None;
				//Camera
				_virtualCamera.Follow = _machineFollow;

				_machine.enabled = true;
				_machine.WeaponController.enabled = true;
				_machine.GetComponent<MachineMovement>().enabled = true;

			}
			else
			{
				_machine.enabled = false;
				_machine.WeaponController.enabled = false;
				_machine.GetComponent<MachineMovement>().enabled = false;
				_player.transform.parent = null;

				//Camera
				_virtualCamera.Follow = _playerFollow;
				Cursor.lockState = CursorLockMode.Locked;
			}
			Cursor.visible = machineMode;
		}
		public void ShowEventVillage()
		{
            _objetcsVillageEvent.SetActive(true);

			
        }

        #endregion

        #region Private Methods
        #endregion
    }

}