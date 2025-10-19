using System;
using System.Collections;
using System.Collections.Generic;
using Deforestation;
using Deforestation.Inputs; 
using Deforestation.Interaction;
using Deforestation.Machine;
using Deforestation.Recolectables;
using Deforestation.UI;
using Photon.Pun;
using UnityEngine;
namespace Deforestation.Network
{
public class NetworkGameController : GameController
    {
        #region Properties
        public NetworkMachine MachineMultiplayer;
        #endregion

        #region Fields
        private bool _isMultiplayer = false;
        public Action _OnVictory;
        #endregion

        #region Unity Callbacks
        protected override void Start()
    {
            if (_isMultiplayer==true)
            {
                Debug.LogWarning("NetworkGameController started in single player mode. Use GameController for single player.");
                base.Start();
            }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
        #endregion

        #region Public Methods
        public void InitializePlayer(HealthSystem health, CharacterController player, Inventory inventory, InteractionSystem interaction, Transform playerFollow, GameInputController inputSystem)
        {
            _playerHealth = health;
            _player = player;
            _inventory = inventory;
            _interactionSystem = interaction;
            _playerFollow = playerFollow;
            _inputSystem = inputSystem;
            _isMultiplayer=true;
            _uiController.inizialiceinventory();
        }
        public void InitializeMachine(Transform follow, MachineController machine)
        {
            if (_machine != null)
            {
                _machine.HealthSystem.OnHealthChanged -= _uiController.UpdateMachineHealth;
            }

            _machineFollow = follow;
            _machine = machine;
            //Conteo de maquinas instanciadas
          

            _machine.HealthSystem.OnHealthChanged += _uiController.UpdateMachineHealth;
            //Para refrescar la UI
            _machine.HealthSystem.TakeDamage(0);
        }
        internal override void MachineMode(bool machineMode)
        {
            MachineModeOn = machineMode;
            //Player
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
        #endregion

        #region Private Methods
        #endregion
    }
}