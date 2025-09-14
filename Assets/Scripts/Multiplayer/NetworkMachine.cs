using System.Collections;
using System.Collections.Generic;
using Deforestation;
using Deforestation.Machine;
using Deforestation.Network;
using Photon.Pun;
using UnityEngine;

public class NetworkMachine : MonoBehaviourPun
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] private MachineController _machine;
    public Transform _machineFollow;
    private NetworkGameController _gameController;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        _gameController = FindObjectOfType<NetworkGameController>(true);
        if (photonView.IsMine)
        {
            _gameController.InitializeMachine(_machineFollow, _machine);
            _gameController.gameObject.SetActive(true);
            _machine.enabled = true;
            _machine.WeaponController.enabled = true;
            _machine.GetComponent<MachineMovement>().enabled = true;
            //Autoridad de la vida en local
            _machine.HealthSystem.OnHealthChanged += SyncHealth;
            //Autoridad de disparos en local
            _machine.WeaponController.OnMachineShoot += SyncShoot;
        }
        else
        {
            //---
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void SyncShoot()
    {
        //Capturar la direccion del cañon
        //TODO: refactorizar!
        RaycastHit hit;
        Ray ray = GameController.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);

        //Mandar RPC
        photonView.RPC("OthersShoot", RpcTarget.Others, hit.point);
    }

    [PunRPC]
    private void OthersShoot(Vector3 shootDirection)
    {
        _machine.WeaponController.Shoot(shootDirection);
    }

    private void SyncHealth(float value)
    {
        photonView.RPC("RefreshHealth", RpcTarget.Others, value);
    }

    [PunRPC]
    private void RefreshHealth(float value)
    {
        _machine.HealthSystem.SetHealth(value);
    }
    #endregion
}
