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
        if (Input.GetKeyUp(KeyCode.Escape) && GameController.Instance.MachineModeOn == true)//Se podria agregar si todavia esta con la animacion estandar no se meta
        {
            // If the player is in machine mode, stop driving and reset the player position
            if (GameController.Instance.MachineModeOn)
            {
                _machineController.StopDriving();
            }
            else
            {
                GameController.Instance.MachineMode(true);
                _machineController.StartDriving(true);
            }
            

        }
    }
    #endregion
}
