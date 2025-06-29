using System.Collections;
using System.Collections.Generic;
using Deforestation;
using Deforestation.Machine;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    private MachineController _machineController;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        _machineController = GameController.Instance.MachineController;
    }

    // Update is called once per frame
    void Update()
    {
        Escape();
    }
    #endregion

    #region Public Methods

    #endregion

    #region Private Methods
    private void Escape()
    {
        if (Input.GetKeyUp(KeyCode.Escape) & GameController.Instance.MachineModeOn == true)
        {
            // If the player is in machine mode, stop driving and reset the player position
            _machineController.StopDriving();

        }
    }
    #endregion
}
