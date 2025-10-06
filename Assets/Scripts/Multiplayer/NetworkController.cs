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
using Unity.VisualScripting;
using UnityEngine;
namespace Deforestation.Multiplayer
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
        //Concion victoria
        private List<GameObject> machines = new List<GameObject>();
        public Photon.Realtime.Player currentPilot;
        private List<Photon.Realtime.Player> mountedPlayers = new List<Photon.Realtime.Player>();
        private List<Photon.Realtime.Player> alivePlayers = new List<Photon.Realtime.Player>();
        private bool gameEnded = false;
        bool IsDeath = true;
        #endregion
        #region Unity Callbacks
        void Start()
    {
            ConnectToServer();
            GameController.Instance.OnMachineModeChange += DisableAvatar;
            GameController.Instance.OnMachineModeChange += HandleMachineModeChange;
            
        }

    // Update is called once per frame
    void Update()
    {
    }

    #endregion

    #region Public Methods
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
                alivePlayers.Add(PhotonNetwork.LocalPlayer);
                _indexSpawns ++;
            }
            else
            {
                _waitingForSpawn = true;
                photonView.RPC("RPC_SpawnPoint", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber); //Llama al metodo RPC_SpawnPoint en el cliente maestro
            }

            _ui.LoadingComplete();
        }
        private void SpawnMe(Vector3 spawnPoint)
        {
            _player = PhotonNetwork.Instantiate("PlayerFPS_Multiplayer", spawnPoint, Quaternion.identity);//Instancia el objeto con el prefab "PlayerMultiplayer" en la posicion spawnPoint
            _machine = PhotonNetwork.Instantiate("TheMachine_Multiplayer", spawnPoint + Vector3.back * 7, Quaternion.identity);

            //dead control
            _player.GetComponent<HealthSystem>().OnDeath += PlayerDie;
            _machine.GetComponent<HealthSystem>().OnDeath += MachineDie;
            _uIGameController.enabled = true;
            photonView.RPC("RegisterAlivePlayer", RpcTarget.AllBuffered); //Registra el jugador como vivo en todos los clientes
            machines.Add(_machine);
        }
        [PunRPC]
        void RegisterAlivePlayer()
        {
            //if (!PhotonNetwork.IsMasterClient) return;
            alivePlayers.Add(PhotonNetwork.LocalPlayer);
            Debug.Log($"Jugador registrado: {PhotonNetwork.LocalPlayer.NickName}. Jugadores vivos: {alivePlayers.Count}");

        }
        [PunRPC]
        void RPC_SpawnPoint(int actorNumber) //Llamado por los clientes que no son maestros para pedir un punto de spawn
        {
            photonView.RPC("RPC_RecivePont", RpcTarget.Others, _spawnPoints[_indexSpawns].position);
            _indexSpawns++;
            if (_indexSpawns >= _spawnPoints.Count)
            {
                _indexSpawns = 0;
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
        private void HandleMachineModeChange(bool isDriving)
        {
            Debug.Log("Aqui registro player" + alivePlayers.Count);
            if (isDriving)
            {
                // Avisar al MasterClient que este jugador montó
                photonView.RPC("RequestMount", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                // Avisar que se desmontó
               photonView.RPC("RequestDismount", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
 
        [PunRPC]
        public void RequestMount(int actorNumber)
        {
            //if (!PhotonNetwork.IsMasterClient || gameEnded) return;
            Debug.Log("Entro en Mount");
            var player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (!mountedPlayers.Contains(player))
            {
                mountedPlayers.Add(player);
                Debug.Log("Montados" + mountedPlayers.Count);
                Debug.Log($"Jugador {player.NickName} se ha montado. Total montados: {mountedPlayers.Count}");
                CheckForWinnerMountedOnly();
            }

            // sincroniza el estado de pilotaje
            photonView.RPC("SyncPilot", RpcTarget.All, actorNumber);

        }

        [PunRPC]
        public void RequestDismount(int actorNumber)
        {
            if (!PhotonNetwork.IsMasterClient || gameEnded) return;

            var player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

            if (mountedPlayers.Contains(player))
            {
                mountedPlayers.Remove(player);
                Debug.Log($"Jugador {player.NickName} se ha desmontado. Total montados: {mountedPlayers.Count}");
                CheckForWinnerMountedOnly();
            }

            photonView.RPC("SyncPilot", RpcTarget.All, -1);
        }
        private void CheckForWinnerMountedOnly()
        {
            if (mountedPlayers.Count == 0 || machines.Count > 1 || mountedPlayers.Count > 1 || gameEnded) return;
            var winner = mountedPlayers[0];
            gameEnded = true;
            CheckForWinner();
        }
        [PunRPC]
        void SyncPilot(int actorNumber)
        {
            currentPilot = actorNumber == -1 ? null :
            PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

        }
        private void MachineDie()
        {
            if (GameController.Instance.MachineModeOn)
            {
                GameController.Instance.MachineMode(false);
                _player.GetComponent<HealthSystem>().TakeDamage(1000);
                machines.Remove(_machine);

                //photonView.RPC("NotifyPlayerEliminated", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            photonView.RPC("RPC_OnDeath", RpcTarget.All);
        }
        [PunRPC]
        void RPC_OnDeath()
        {
            Destroy(_machine);
            SpawnExplosions(_machine.transform.position + Vector3.up * 4, 5, 5);
        }
        [PunRPC]
        void NotifyPlayerEliminated(int actorNumber)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            var player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            alivePlayers.Remove(player);

            Debug.Log($"Jugador eliminado: {player.NickName}. Jugadores vivos: {alivePlayers.Count}");

            CheckForWinner();
        }
        private void CheckForWinner()
        {
            if (alivePlayers.Count == 1)
            {
                var winner = alivePlayers[0];
                photonView.RPC("DeclareWinner", RpcTarget.All, winner.ActorNumber);
            }
            else if (alivePlayers.Count == 0 || machines.Count == 0)
            {
                photonView.RPC("DeclareDraw", RpcTarget.All);
            }
        }
        [PunRPC]
        void DeclareWinner(int winnerActorNumber)
        {
            var winner = PhotonNetwork.CurrentRoom.GetPlayer(winnerActorNumber);
            Debug.Log($"¡Victoria de {winner.NickName} por ser el último montado!");
            //_uIGameController.ShowVictoryScreen(winner.NickName);
        }

        [PunRPC]
        void DeclareDraw()
        {
            Debug.Log("Empate: todos los jugadores han sido eliminados");
            //_uIGameController.ShowDrawScreen();
        }
        #endregion

        #region Private Methods
        private void ConnectToServer()
        {
            PhotonNetwork.ConnectUsingSettings();
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
        private void DisableAvatar(bool state)
        {
            photonView.RPC("RPC_ReciveModeMachine", RpcTarget.Others, state);
        }
        [PunRPC]
        private void RPC_ReciveModeMachine(bool value)
        {
            _player.active = value;
        }
        private void PlayerDie()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _ui.EndGamePanel.SetActive(true);
        }
        #endregion
    }

}