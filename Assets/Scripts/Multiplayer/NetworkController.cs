using System;
using System.Collections;
using System.Collections.Generic;
using Deforestation.UI;
using Photon.Pun;
using Photon.Realtime;
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
        #endregion
        #region Unity Callbacks
        void Start()
    {
            ConnectToServer();
            GameController.Instance.OnMachineModeChange += DisableAvatar;
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
                _indexSpawns ++;
            }
            else
            {
                _waitingForSpawn = true;
                photonView.RPC("RPC_SpawnPoint", RpcTarget.MasterClient); //Llama al metodo RPC_SpawnPoint en el cliente maestro

            }

            _ui.LoadingComplete();
        }


        #endregion

        #region Private Methods
        private void ConnectToServer()
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        private void SpawnMe(Vector3 spawnPoint)
        {
            _player = PhotonNetwork.Instantiate("PlayerFPS_Multiplayer", spawnPoint, Quaternion.identity);//Instancia el objeto con el prefab "PlayerMultiplayer" en la posicion spawnPoint
            _machine = PhotonNetwork.Instantiate("TheMachine_Multiplayer", spawnPoint + Vector3.back * 7, Quaternion.identity);

            //dead control
            _player.GetComponent<HealthSystem>().OnDeath += PlayerDie;
           _machine.GetComponent<HealthSystem>().OnDeath += MachineDie;
            //me voy a guardar aqui un array de maquinas y players para cuando muera uno hacer respawn
            _uIGameController.enabled = true;
        }
        [PunRPC]
        void RPC_SpawnPoint() //Llamado por los clientes que no son maestros para pedir un punto de spawn
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
        private void MachineDie()
        {
            if (GameController.Instance.MachineModeOn)
            {
                GameController.Instance.MachineMode(false);
                _player.GetComponent<HealthSystem>().TakeDamage(1000);
            }
            //Aqui resto de la cuenta de maquinas

            Destroy(_machine);
            SpawnExplosions(_machine.transform.position + Vector3.up * 4, 5, 5);
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