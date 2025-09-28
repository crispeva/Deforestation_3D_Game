using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Deforestation;
using Deforestation.Inputs;
using Deforestation.Interaction;
using Deforestation.Network;
using Deforestation.Recolectables;
using Photon.Pun;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Deforestation.Multiplayer
{

public class NetworkPlayer : MonoBehaviourPun
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] private HealthSystem _health;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private InteractionSystem _interactions;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private FirstPersonController _fps;
    [SerializeField] private StarterAssetsInputs _inputs;
    [SerializeField] private PlayerInput _inputsPlayer;
    [SerializeField] private GameObject _3dAvatar;
    [SerializeField] private Transform _playerFollow;

    private NetworkGameController _gameController;
    private Animator _anim;
    private GameInputController _inputSystem;
        #endregion

        #region Unity Callbacks
    void Start()
    {
        _gameController = FindObjectOfType<NetworkGameController>(true);
        _inputSystem = FindObjectOfType<GameInputController>(true);
            if (photonView.IsMine)
        {
            _gameController.InitializePlayer(_health, _controller, _inventory, _interactions, _playerFollow, _inputSystem);
            _health.OnHealthChanged += Hit;
            _health.OnDeath += Die;
            _health.enabled = true;
            _inventory.enabled = true;
            _interactions.enabled = true;
            _fps.enabled = true;
            _controller.enabled = true;
            _inputs.enabled = true;
                var vcam = FindObjectOfType<CinemachineVirtualCamera>();
                if (vcam != null && _playerFollow != null)
                {
                    vcam.Follow = _playerFollow;
                    vcam.LookAt = _playerFollow;
                }
                Invoke(nameof(AddInitialCrystals), 1);

        }
        else
        {
            DisconectPlayer();
        }
    }
    private void Awake()
    {
        _anim = _3dAvatar.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //TODO: MOVER A inputcontroller
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                _anim.SetBool("Run", true);
            }
            else
            {
                _anim.SetBool("Run", false);
            }
            if (Input.GetKeyUp(KeyCode.Space))
                {
                    _anim.SetTrigger("Jump");
                }
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void AddInitialCrystals()
    {
        _inventory.AddRecolectable(RecolectableType.SuperCrystal, 5);
        _inventory.AddRecolectable(RecolectableType.HyperCrystal, 5);
        _inventory.AddRecolectable(RecolectableType.MegaCrystal, 5);
    }

    private void DisconectPlayer()
    {
        Destroy(_health);
        Destroy(_inventory);
        Destroy(_interactions);
        Destroy(_fps);
        Destroy(_controller);
        Destroy(_inputs);
        Destroy(_inputsPlayer);
    }
    private void Die()
    {
        _anim.SetTrigger("Die");
        DisconectPlayer();
        this.enabled = false;

    }

    private void Hit(float obj)
    {
        _anim.SetTrigger("Hit");
    }

    #endregion
}
}