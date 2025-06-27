using UnityEngine;
using DG.Tweening;
using System;
using Deforestation.Interaction;
using Deforestation.Machine;

namespace Deforestation.Audio
{
	public class AudioController : MonoBehaviour
	{
		const float MAX_VOLUME = 0.1f;
        [SerializeField] InteractionSystem _interactionSystem;
        [SerializeField] MachineMovement _machinemovement;
        #region Fields
        [Header("FX")]
		[SerializeField] private AudioSource _steps;
		[SerializeField] private AudioSource _machineOn;
		[SerializeField] private AudioSource _machineOff;
		[SerializeField] private AudioSource _shoot;
		[SerializeField] private AudioSource _mineraltaked;
		[SerializeField] private AudioSource _walk_machine;

		[Space(10)]

		[Header("Music")]
		[SerializeField] private AudioSource _musicMachine;
		[SerializeField] private AudioSource _musicHuman;
		#endregion

		#region Properties
		#endregion

		#region Unity Callbacks	
		private void Awake()
		{
			GameController.Instance.OnMachineModeChange += SetMachineMusicState;
			GameController.Instance.MachineController.OnMachineDriveChange += SetMachineDriveEffect;
			GameController.Instance.MachineController.WeaponController.OnMachineShoot += ShootFX;
            _interactionSystem.OnMineralSound += TakeMaterialSound;
            _machinemovement.OnMachineWalking += WalkingMachineSound;
            //GameController.Instance.MachineController.OnMachineWalking += WalkingMachineSound;
		}

        private void Start()
		{
			_musicHuman.Play();
		}

		private void Update()
		{
			//TODO: MOVER A inputcontroller
			if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
			{
				if (!_steps.isPlaying)
					_steps.Play();
			}
			else
				if (_steps.isPlaying)
					_steps.Stop();

		}
		#endregion

		#region Private Methods
		private void SetMachineMusicState(bool machineMode)
		{
			if (machineMode)
			{
				_musicHuman.DOFade(0, 3);
				_musicMachine.DOFade(MAX_VOLUME, 3);
				_musicMachine.Play();
			}
			else
			{
				_musicHuman.DOFade(MAX_VOLUME, 3);
				_musicMachine.DOFade(0, 3);
			}
		}
        private void TakeMaterialSound()
        {
            _mineraltaked.PlayOneShot(_mineraltaked.clip, MAX_VOLUME);
        }
		private void WalkingMachineSound(bool driving)
        {
            if (driving)
			{
                _walk_machine.DOFade(MAX_VOLUME, 3);
                _walk_machine.Play();
               // _musicMachine.DOFade(0, 1);
			}
			else
			{
                _walk_machine.DOFade(0, 0.1f);
               //_walk_machine.Stop();
            }

				

        }

        private void SetMachineDriveEffect(bool startDriving)
		{
			if (startDriving)
			{
                _machineOn.DOFade(MAX_VOLUME, 3);
                _machineOn.Play();
            }
			else
				_machineOff.Play();

		}
		private void ShootFX()
		{
			_shoot.Play();
		}
		#endregion

		#region Public Methods
		#endregion

	}

}