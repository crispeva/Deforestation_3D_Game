using System.Collections;
using System.Collections.Generic;
using Deforestation;
using Deforestation.Interaction;
using Deforestation.Machine;
using Deforestation.Recolectables;
using Deforestation.Inputs; 
using UnityEngine;
using Deforestation.UI;
using System;
namespace Deforestation.Network
{
public class NetworkGameController : GameController
{
        #region Properties
        #endregion

        #region Fields
        private bool _isMultiplayer = false;
        public int machines = 0;
        public int players = 0;
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
            players++;
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
            machines++;

            _machine.HealthSystem.OnHealthChanged += _uiController.UpdateMachineHealth;
            //Para refrescar la UI
            _machine.HealthSystem.TakeDamage(0);
        }
        #endregion

        private void Victory()
        {
            if (machines == 1||players==1)
            {
                _OnVictory?.Invoke();
            }
        }
        #region Private Methods
        #endregion
    }
}