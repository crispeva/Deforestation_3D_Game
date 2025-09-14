using System.Collections;
using System.Collections.Generic;
using Deforestation;
using Deforestation.Interaction;
using Deforestation.Machine;
using Deforestation.Recolectables;
using Deforestation.Inputs; 
using UnityEngine;
namespace Deforestation.Network
{
public class NetworkGameController : GameController
{
     #region Properties
    #endregion

    #region Fields
    #endregion

    #region Unity Callbacks
    void Start()
    {
        
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
        }
        public void InitializeMachine(Transform follow, MachineController machine)
        {
            if (_machine != null)
            {
                _machine.HealthSystem.OnHealthChanged -= _uiController.UpdateMachineHealth;
            }

            _machineFollow = follow;
            _machine = machine;

            _machine.HealthSystem.OnHealthChanged += _uiController.UpdateMachineHealth;
            //Para refrescar la UI
            _machine.HealthSystem.TakeDamage(0);
        }
        #endregion

        #region Private Methods
        #endregion
    }
}