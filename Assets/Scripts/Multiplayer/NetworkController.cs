using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Deforestation.Machine;
using Deforestation.UI;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
namespace Deforestation.Network
{

public class NetworkController : MonoBehaviourPunCallbacks //Debe de heredar de MonoBehaviourPunCallbacks para poder usar los callbacks de Photon
    {
        #region Properties
        #endregion

        #region Fields
        //Master
        [SerializeField] private List<Transform> _spawnPoints;
        private int _indexSpawns;
        //UI
        [SerializeField] private UINetwork _ui;
        [SerializeField] private UIGameController _uIGameController;
        //Client
        private bool _waitingForSpawn=false;
        //Objects
        private GameObject _machine;
        private GameObject _player;
        [SerializeField] private GameObject _explosionPrefab;
        //Condicion victoria
        private List<GameObject> machines = new List<GameObject>();
        public Photon.Realtime.Player currentPilot;
        private List<Photon.Realtime.Player> mountedPlayers = new List<Photon.Realtime.Player>();
        private List<Photon.Realtime.Player> alivePlayers = new List<Photon.Realtime.Player>();
        private bool gameEnded = false;
        //[SerializeField] private TextMeshProUGUI _textVictory;
        #endregion
        #region Unity Callbacks
        void Start()
        {
            ConnectToServer();
            GameController.Instance.OnMachineModeChange += DisableAvatar;
            GameController.Instance.OnMachineModeChange += HandleMachineModeChange;
            
        }
        #endregion

        #region Photon Methods
        public override void OnConnectedToMaster() //Callback que se llama cuando el cliente se conecta al servidor maestro
        {
            Debug.Log("Connected to Master");
            PhotonNetwork.JoinOrCreateRoom("Deforestation", new Photon.Realtime.RoomOptions { MaxPlayers = 4 }, null);
        }
        public override void OnJoinedRoom() 
        {
            if (PhotonNetwork.IsMasterClient) //Si este cliente es el maestro
            {

                SpawnMe(_spawnPoints[0].position);
                _indexSpawns ++;
            }
            else
            {
                _waitingForSpawn = true;
                photonView.RPC("RPC_SpawnPoint", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber); //Llama al metodo RPC_SpawnPoint en el cliente maestro
            }

           
            _ui.LoadingComplete();
        }
        private void ConnectToServer()
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        #endregion'

        #region Spawn Methods
        private void SpawnMe(Vector3 spawnPoint)
        {
            _player = PhotonNetwork.Instantiate("PlayerFPS_Multiplayer", spawnPoint, Quaternion.identity);//Instancia el objeto con el prefab "PlayerMultiplayer" en la posicion spawnPoint
            _machine = PhotonNetwork.Instantiate("TheMachine_Multiplayer", spawnPoint + Vector3.back * 7, Quaternion.identity);

            //dead control
            _player.GetComponent<HealthSystem>().OnDeath += PlayerDie;
            _machine.GetComponent<HealthSystem>().OnDeath += MachineDie;
            _uIGameController.enabled = true;
            photonView.RPC("RegisterAlivePlayer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber); //Registra el jugador como vivo en todos los clientes
            _ui.PlayerScreen("JUGADOR" + PhotonNetwork.LocalPlayer.ActorNumber);
            // Registro de la máquina: solo el Master mantiene la lista authoritative.
            var machinePV = _machine.GetComponent<PhotonView>();
            if (machinePV != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    // Si yo soy el master, añado directamente
                    if (!machines.Contains(_machine))
                    {
                        machines.Add(_machine);
                    }
                }
                else
                {
                    // Si no soy master, informo al master con el viewID para que lo registre
                    photonView.RPC("RegisterMachine", RpcTarget.MasterClient, machinePV.ViewID);
                }
            }

            Debug.Log($"Total de machines local (master-only authoritative) {machines.Count}");
        }
        [PunRPC]
        void RegisterAlivePlayer(int actorNumber)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            var player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (player == null) return;

            if (!alivePlayers.Contains(player))
            {
                alivePlayers.Add(player);
               
                Debug.Log($"[Master] Jugador registrado: {player.NickName}. Jugadores vivos: {alivePlayers.Count}");
            }
        }
        [PunRPC]
        void RPC_SpawnPoint(int actorNumber) //Llamado por los clientes que no son maestros para pedir un punto de spawn
        {
            // Antes: photonView.RPC("RPC_RecivePont", RpcTarget.Others, _spawnPoints[_indexSpawns].position);
            // Enviar SOLO al player que lo solicitó.
            var target = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (target != null)
            {
                photonView.RPC("RPC_RecivePont", target, _spawnPoints[_indexSpawns].position);
                _indexSpawns++;
                if (_indexSpawns >= _spawnPoints.Count)
                {
                    _indexSpawns = 0;
                }
            }
            else
            {
                Debug.LogWarning($"RPC_SpawnPoint: no se encontró player actorNumber {actorNumber}");
            }
        }
        [PunRPC]
        void RPC_RecivePont(Vector3 spawnPos) //Llamado por el cliente maestro para enviar el punto de spawn al cliente que lo pidio
        {
            if (_waitingForSpawn)
            {
                _waitingForSpawn = false;
                SpawnMe(spawnPos);
            }
        }
        #endregion

        #region Mount/Dismount Methods
        [PunRPC]
        private void RPC_ReciveModeMachine(bool value)
        {
            _player.SetActive(value);
        }
        private void HandleMachineModeChange(bool isDriving)
        {
            Debug.Log("Aqui registro player" + alivePlayers.Count);
            if (isDriving)
            {
                // Avisar SOLO al MasterClient que este jugador montó
                photonView.RPC("RequestMount", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                // Avisar SOLO al MasterClient que se desmontó
                photonView.RPC("RequestDismount", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
 
        [PunRPC]
        public void RequestMount(int actorNumber)
        {
            if (!PhotonNetwork.IsMasterClient || gameEnded) return;

            var player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (player == null) return;

            if (!mountedPlayers.Contains(player))
            {
                mountedPlayers.Add(player);
                Debug.Log($"[Master] Jugador {player.NickName} se ha montado. Total montados: {mountedPlayers.Count}");
                CheckForWinnerMountedOnly();
            }
        }

        [PunRPC]
        public void RequestDismount(int actorNumber)
        {
            if (!PhotonNetwork.IsMasterClient || gameEnded) return;

            var player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (player == null) return;

            if (mountedPlayers.Contains(player))
            {
                mountedPlayers.Remove(player);
                Debug.Log($"[Master] Jugador {player.NickName} se ha desmontado. Total montados: {mountedPlayers.Count}");
                CheckForWinnerMountedOnly();
            }
        }
        #endregion

        #region Victory Methods
        private void CheckForWinnerMountedOnly()
        {
            if (mountedPlayers.Count == 0 || machines.Count > 1 || mountedPlayers.Count > 1 || gameEnded) return;
            gameEnded = true;
            CheckForWinner();
        }
        private void CheckForWinner()
        {
            if (alivePlayers.Count == 1 && mountedPlayers.Count ==1)
            {
                var winner = alivePlayers[0];
                photonView.RPC("DeclareWinner", RpcTarget.All, winner.ActorNumber);
            }
            else if (alivePlayers.Count == 2 && mountedPlayers.Count==1)
            {
                var winner = mountedPlayers[0];
                photonView.RPC("DeclareWinner", RpcTarget.All, winner.ActorNumber);
            }
            else if (alivePlayers.Count == 0 || machines.Count == 0)
            {
                Debug.Log($"Machine eliminadas: {machines.Count}. Jugadores vivos: {alivePlayers.Count}");
                photonView.RPC("DeclareDraw", RpcTarget.All);
            }
        }
        [PunRPC]
        void DeclareWinner(int winnerActorNumber)
        {
            var winner = PhotonNetwork.CurrentRoom.GetPlayer(winnerActorNumber);
            Debug.Log($"¡Victoria de {winner.NickName} por ser el último montado!");
            _ui.ShowVictoryScreen("JUGADOR "+ winnerActorNumber);
        }

        [PunRPC]
        void DeclareDraw()
        {
            Debug.Log("Empate: todos los jugadores han sido eliminados");
            _ui.ShowDrawScreen();
        }

        #endregion

        #region  Machine Methods
        [PunRPC]
        void RegisterMachine(int viewID)
        {
            // Solo el Master debe mantener la lista authoritative
            if (!PhotonNetwork.IsMasterClient) return;

            PhotonView pv = PhotonView.Find(viewID);
            if (pv == null)
            {
                Debug.LogWarning($"RegisterMachine: no se encontró PhotonView con ViewID {viewID}");
                return;
            }

            if (!machines.Contains(pv.gameObject))
            {
                machines.Add(pv.gameObject);
                Debug.Log($"[Master] Máquina registrada: {pv.Owner.NickName} ViewID {viewID}. Total machines: {machines.Count}");
            }
        }
        private void MachineDie()
        {
            if (_machine == null) return;

            var machinePV = _machine.GetComponent<PhotonView>();
            if (machinePV != null)
            {
                // Desmontar al jugador si está en modo máquina
                if (GameController.Instance.MachineModeOn)
                {
                    GameController.Instance.MachineMode(false);
                    _player.GetComponent<HealthSystem>().TakeDamage(1000);
                }
                
                // Pedir al Master que destruya la máquina (autoridad)
                photonView.RPC("RPC_DestroyMachine", RpcTarget.MasterClient, machinePV.ViewID);
            }
        }

        public void SpawnExplosions(Vector3 centerPoint, int numberOfExplosions = 4, float maxDistance = 5f)
        {
            for (int i = 0; i < numberOfExplosions; i++)
            {
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere;
                Vector3 spawnPosition = centerPoint + randomDirection.normalized * UnityEngine.Random.Range(0f, maxDistance);
                Instantiate(_explosionPrefab, spawnPosition, Quaternion.identity);
            }
        }
        [PunRPC]
        private void RPC_DestroyMachine(int viewID)
        {
            // Este RPC debe ejecutarse en MasterClient (autoridad)
            if (!PhotonNetwork.IsMasterClient) return;

            PhotonView pv = PhotonView.Find(viewID);
            if (pv == null)
            {
                Debug.LogWarning($"[Master] RPC_DestroyMachine: PhotonView no encontrado. ViewID {viewID}");
                return;
            }

            Debug.Log($"[Master] RPC_DestroyMachine called. LocalMaster: {PhotonNetwork.LocalPlayer.ActorNumber}, PV Owner: {(pv.Owner != null ? pv.Owner.ActorNumber.ToString() : "null")}");

            // Asegurarnos de ser el propietario antes de destruir: el Master toma ownership si hace falta
            if (pv.Owner == null || pv.Owner.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                try
                {
                    pv.TransferOwnership(PhotonNetwork.LocalPlayer);
                    Debug.Log($"[Master] Ownership transferido al Master (Actor {PhotonNetwork.LocalPlayer.ActorNumber}) para ViewID {viewID}");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[Master] No se pudo transferir ownership: {ex.Message}");
                    // Intentamos continuar: Photon puede permitir destrucción por Master tras takeover, pero registramos la falla
                }
            }

            // Crear explosiones en todos los clientes
            photonView.RPC("RPC_CreateExplosions", RpcTarget.All, pv.transform.position);

            // Actualizar lista de máquinas y destruir desde el Master
            if (machines.Contains(pv.gameObject))
            {
                machines.Remove(pv.gameObject);
                Debug.Log($"[Master] Destruyendo máquina del jugador {pv.Owner?.NickName} total de machines {machines.Count}");
            }
            else
            {
                Debug.LogWarning($"[Master] RPC_DestroyMachine: la máquina no estaba registrada en la lista. ViewID {viewID}");
            }

            // Finalmente destruir en red
            PhotonNetwork.Destroy(pv.gameObject);

            CheckForWinner();
        }

        [PunRPC]
        private void RPC_CreateExplosions(Vector3 position)
        {
            SpawnExplosions(position + Vector3.up * 4, 5, 5);

            // Limpiar referencia local si es nuestra máquina
            if (_machine != null && _machine.transform.position == position)
            {
                _machine = null;
            }
        }
        #endregion

        #region Player Methods

        private void PlayerDie()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _ui.EndGamePanel.SetActive(true);
            photonView.RPC("NotifyPlayerEliminated", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        private void DisableAvatar(bool state)
        {
            Debug.Log("DisableAvatar RPC State: " + !state);
            if (photonView.IsMine)
                photonView.RPC("RPC_ReciveModeMachine", RpcTarget.All, !state);
        }

        [PunRPC]
        void NotifyPlayerEliminated(int actorNumber)
        {
            //if (!PhotonNetwork.IsMasterClient) return;

            var player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            Debug.Log($". Jugador actual: {player.ActorNumber}");
            Debug.Log($". Jugadores vivos: {alivePlayers.Count}");
            alivePlayers.Remove(player);

            Debug.Log($"Jugador eliminado: {player.NickName}. Jugadores vivos: {alivePlayers.Count}");

            CheckForWinner();
        }

        [PunRPC]
        void SyncPilot(int actorNumber)
        {
            currentPilot = actorNumber == -1 ? null :
            PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

        }



      

    
        #endregion

    }

}