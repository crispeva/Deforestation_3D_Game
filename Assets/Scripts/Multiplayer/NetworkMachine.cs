using System;
using System.Collections;
using System.Collections.Generic;
using Deforestation;
using Deforestation.Machine;
using Deforestation.Machine.Weapon;
using Deforestation.Network;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using UnityEngine;

public class NetworkMachine : MonoBehaviourPun, IPunObservable
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] private MachineController _machine;
    public Transform _machineFollow;
    private NetworkGameController _gameController;
    private Quaternion _lastReceivedTowerRotation;
    public bool IsAlive
    {
        get { return GameController.Instance.HealthSystem.CurrentHealth > 0; }
    }
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
            //Autoridad de animaciones en local
            GameController.Instance.MachineController.OnAnimationSync += SyncAnimation;

        }
        else
        {
            // Guardamos la rotación inicial de la torreta
            _lastReceivedTowerRotation = _machine.WeaponController.TowerWeapon.rotation;
        }
    }

  

    // Update is called once per frame
    void Update()
    {
        // Solo los clientes remotos interpolan la rotación de la torreta
        if (!photonView.IsMine)
        {
            // Suaviza la rotación de la torreta en clientes remotos
            _machine.WeaponController.TowerWeapon.rotation = Quaternion.Slerp(
                _machine.WeaponController.TowerWeapon.rotation,
                _lastReceivedTowerRotation,
                Time.deltaTime * 10f
            );
        }
        
    }
    #endregion
    #region Networking
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Solo el dueño envía la rotación de la torreta
            stream.SendNext(_machine.WeaponController.TowerWeapon.rotation);
        }
        else
        {
            // Los demás la reciben
            _lastReceivedTowerRotation = (Quaternion)stream.ReceiveNext();
        }
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
        _machine.HealthSystem.SetHealth(value,false);
        GameController.Instance.UIGameController.UpdateMachineHealth(value);
    }

    // Ejemplo: sincronizar el trigger "Jump"
    void SyncAnimation(string animName)
    {
        if (photonView.IsMine)
            photonView.RPC("RPC_PlayAnimation", RpcTarget.All, animName);
    }
    [PunRPC]
    void RPC_PlayAnimation(string animName)
    {
        _machine.GetComponent<Animator>().SetTrigger(animName);
    }
    #endregion
}
