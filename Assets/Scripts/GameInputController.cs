using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Deforestation;
using Deforestation.Machine;
using Deforestation.Recolectables;
using StarterAssets;
using UnityEngine;
namespace Deforestation.Inputs{ 
public class GameInputController : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    public Action _onActiveMenu;
    public Action _onExitMachine;
    public Action _onShootMachine;
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
        ShootBullet();
    }

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods
 
    private void PauseGame()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && GameController.Instance.HealthSystem.CurrentHealth >0)
        {
            _onActiveMenu?.Invoke();
            
        }
    }
    private void ShootBullet()
    {
        if (Input.GetMouseButtonUp(0) && GameController.Instance.MachineModeOn && GameController.Instance.Inventory.UseResource(Deforestation.Recolectables.RecolectableType.SuperCrystal) && GameController.Instance.UIGameController.Ispaused == false)
        {
            _onShootMachine?.Invoke();
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
}