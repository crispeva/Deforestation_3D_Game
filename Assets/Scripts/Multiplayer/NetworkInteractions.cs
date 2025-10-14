using System.Collections;
using System.Collections.Generic;
using Deforestation.Interaction;
using Deforestation.Machine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Deforestation.Network
{
    public class NetworkInteractions : MachineInteraction, IPunOwnershipCallbacks
    {
        #region Fields
        private PhotonView _pendingMachineView;
        private MachineController _pendingMachine;
        private Transform _pendingFollow;
        #endregion

        #region Unity Callbacks
        void Start()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        #endregion

        #region Private Methods
        public override void Interact()
        {
            if (_type == MachineInteractionType.Door)
            {
                //Move Door
                transform.position = _target.position;
            }

            if (_type == MachineInteractionType.Stairs)
            {
                //Teleport Player
                GameController.Instance.TeleportPlayer(_target.position);
            }

            if (_type == MachineInteractionType.Machine)
            {
                PhotonView pv = _target.GetComponent<PhotonView>();

                if (pv == null)
                {
                    Debug.LogError("No se encontró PhotonView en la máquina!");
                    return;
                }

                // Guardar referencias para usar después de obtener ownership
                _pendingMachineView = pv;
                _pendingMachine = _target.GetComponent<MachineController>();
                _pendingFollow = _target.GetComponent<NetworkMachine>()._machineFollow;

                Debug.Log($"Solicitando ownership. ViewID: {pv.ViewID}, IsMine: {pv.IsMine}");

                // Si ya es tuyo, activar inmediatamente
                if (pv.IsMine)
                {
                    ActivateMachine();
                }
                else
                {
                    // Solicitar ownership y esperar callback
                    pv.RequestOwnership();
                }
            }
        }

        private void ActivateMachine()
        {
            if (_pendingMachine == null || _pendingFollow == null)
            {
                Debug.LogError("Referencias de máquina no válidas!");
                return;
            }

            Debug.Log("✓ Ownership confirmado. Inicializando máquina...");

            (NetworkGameController.Instance as NetworkGameController).InitializeMachine(_pendingFollow, _pendingMachine);
            GameController.Instance.MachineMode(true);

            // Limpiar referencias
            _pendingMachineView = null;
            _pendingMachine = null;
            _pendingFollow = null;
        }
        #endregion

        #region IPunOwnershipCallbacks
        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            // Solo procesar si es la máquina que solicitamos
            if (targetView == _pendingMachineView && targetView.IsMine)
            {
                Debug.Log("✓ Ownership transferido exitosamente!");
                ActivateMachine();
            }
        }

        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            // Permitir que otros jugadores tomen control
            if (targetView.IsMine)
            {
                Debug.Log($"Cediendo ownership a {requestingPlayer.NickName}");
                targetView.TransferOwnership(requestingPlayer);
            }
        }

        public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
        {
            if (targetView == _pendingMachineView)
            {
                Debug.LogError("❌ Falló la transferencia de ownership!");

                // Limpiar referencias
                _pendingMachineView = null;
                _pendingMachine = null;
                _pendingFollow = null;
            }
        }
        #endregion
    }
}