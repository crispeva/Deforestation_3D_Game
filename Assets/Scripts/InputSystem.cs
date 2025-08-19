using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Deforestation;
using Deforestation.Machine;
using StarterAssets;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    private MachineMovement _machineMovement;
    public Action _onActiveMenu;
    public Action _onExitMachine;
    #endregion

    #region Unity Callbacks
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ExitMachine();
        PauseGame();
    }

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods
 
    private void PauseGame()
    {
        if (Input.GetKeyUp(KeyCode.Escape) & GameController.Instance.HealthSystem.CurrentHealth >0)
        {
            _onActiveMenu?.Invoke();
            
        }
    }
    private void ExitMachine()
    {
        if (GameController.Instance.MachineModeOn && Input.GetKeyUp(KeyCode.Q))
        {
            Debug.Log("Q detectada en MachineMovement");
            _onExitMachine?.Invoke();
        }
    }
    #endregion
}
