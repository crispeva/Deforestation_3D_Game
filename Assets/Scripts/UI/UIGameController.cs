using UnityEngine;
using TMPro;
using Deforestation.Recolectables;
using System;
using Deforestation.Interaction;
using UnityEngine.UI;
using UnityEngine.Audio;
using StarterAssets;
using System.Collections;
using UnityEngine.Events;

namespace Deforestation.UI
{
	public class UIGameController : MonoBehaviour
	{
		#region Properties
		#endregion

		#region Fields
		private Inventory _inventory => GameController.Instance.Inventory;		
		private InteractionSystem _interactionSystem => GameController.Instance.InteractionSystem;
		[SerializeField] private InputSystem _intputSystem;
		[SerializeField] private GameMenuManager _gameMenuManager;
		[SerializeField] private WinEvent _winEvent;
		[SerializeField] private ForestEvent _forestEvent;
        private HealthSystem _healthSystemPlayer => GameController.Instance.HealthSystem;
        private HealthSystem _healthSystemMachine => GameController.Instance.MachineController.HealthSystem;
        private VillageEventTrigger _distanceEvents => GameController.Instance.DistanceEvents;

        [Header("Settings")]
		[SerializeField] private AudioMixer _mixer;
		[SerializeField] private Slider _musicSlider;
		[SerializeField] private Slider _fxSlider;
		[SerializeField] private Button _lowQuality;
		[SerializeField] private Button _mediumQuality;
		[SerializeField] private Button _highQuality;
		[SerializeField] private Button _closeButton;

		[Header("Inventory")]
		[SerializeField] private TextMeshProUGUI _crystal1Text;
		[SerializeField] private TextMeshProUGUI _crystal2Text;
		[SerializeField] private TextMeshProUGUI _crystal3Text;
		[Header("Interaction")]
		[SerializeField] private InteractionPanel _interactionPanel;
		[Header("Live")]
		[SerializeField] private Slider _machineSlider;
		[SerializeField] private Slider _playerSlider;
		[Header("Events")]
        [SerializeField] private GameObject _diePanel;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private FirstPersonController FirstPersonController;
        private bool _settingsOn = false;
        public CanvasGroup canvasGroupDie;
        public float duration = 1f;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
		{

            //My Events 
            //Health events
            _inventory.OnInventoryUpdated += UpdateUIInventory;
			_interactionSystem.OnShowInteraction += ShowInteraction;
			_interactionSystem.OnHideInteraction += HideInteraction;
            //Events and Dialog
            _distanceEvents.OnEventVillage += ShowEventDialog;
            _distanceEvents.OnExitEventVillage += HideEventDialog;
            _forestEvent.OnForestEvent += ShowEventDialog;
            _winEvent.OnWin += ShowEventDialog;
            //Settings events
            _musicSlider.onValueChanged.AddListener(MusicVolumeChange);
			_fxSlider.onValueChanged.AddListener(FXVolumeChange);
            _lowQuality.onClick.AddListener(() => SetQuality(0));
            _mediumQuality.onClick.AddListener(() => SetQuality(1));
            _highQuality.onClick.AddListener(() => SetQuality(2));
            _closeButton.onClick.AddListener(CloseSettings);
            _intputSystem._onActiveMenu += ShowPausePanel;
            _gameMenuManager._onActivePauseMenu += HidePausePanel;
			_gameMenuManager._onActiveSettingsMenu += ShowSettingsPanel;
            //Die Panels
            _healthSystemPlayer.OnDeath += ShowDiePanel;
            _healthSystemMachine.OnDeath += ShowDiePanel;

        }

        private void CloseSettings()
        {
            _settingsOn = !_settingsOn;
            _settingsPanel.SetActive(_settingsOn);
            _pausePanel.SetActive(true);

        }

        private void Awake()
        {
            Cursor.visible = false; // No muestra el cursor
            Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor     
        }

        internal void UpdateMachineHealth(float value)
		{
			_machineSlider.value = value;
		}

		internal void UpdatePlayerHealth(float value)
		{
			_playerSlider.value = value;
		}

		#endregion

		#region Public Methods
		public void ShowInteraction(string message)
		{
			_interactionPanel.Show(message);
		}
        //Events and Dialogs
        public void ShowEventDialog(CanvasGroup canvasGroup)
        {
                StartCoroutine(FadeIn(canvasGroup, duration));
        }
        public void HideEventDialog(CanvasGroup canvasGroup)
        {

                StartCoroutine(FadeOut(canvasGroup, duration));
        }
        public void ShowSettingsPanel()
		{
            _settingsOn = !_settingsOn;
            _settingsPanel.SetActive(_settingsOn);
            _pausePanel.SetActive(false);
        }

        public void ShowDiePanel()
        {
            StartCoroutine(FadeIn(canvasGroupDie, duration));
        }
        IEnumerator FadeIn(CanvasGroup group, float duration)
        {
            float t = 0f;
            group.interactable = true;
            group.blocksRaycasts = true;

            while (t < duration)
            {
                group.alpha = Mathf.Lerp(0f, 1f, t / duration);
                t += Time.deltaTime;
                yield return null;
            }

            group.alpha = 1f;
        }
        public IEnumerator FadeOut(CanvasGroup group, float duration)
        {
            float t = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            while (t < duration)
            {
                group.alpha = Mathf.Lerp(1f, 0f, t / duration);
                t += Time.deltaTime;
                yield return null;
            }
            group.alpha = 0f;
        }
        public void ShowPausePanel()
        {
			if (_settingsPanel.active == false)
			{
                Time.timeScale = 0f; // Pausa el juego
                FirstPersonController.enabled = false; // Desactiva el input provider de Cinemachine
                Cursor.visible = true; // Muestra el cursor
                Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
                _pausePanel.SetActive(true);
            }

        } 
        public void HidePausePanel()
        {
            Time.timeScale = 1f; // Continúa el juego
            FirstPersonController.enabled = true;
            _pausePanel.SetActive(false);
            Cursor.visible = false; // No muestra el cursor
            Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor
        }
        public void HideInteraction()
		{
			_interactionPanel.Hide();

		}

		#endregion

		#region Private Methods
		private void UpdateUIInventory()
		{
			if (_inventory.InventoryStack.ContainsKey(RecolectableType.SuperCrystal))
				_crystal1Text.text = _inventory.InventoryStack[RecolectableType.SuperCrystal].ToString();
			else
				_crystal1Text.text = "0";
			if (_inventory.InventoryStack.ContainsKey(RecolectableType.HyperCrystal))
				_crystal2Text.text = _inventory.InventoryStack[RecolectableType.HyperCrystal].ToString();
			else
				_crystal2Text.text = "0";
            if (_inventory.InventoryStack.ContainsKey(RecolectableType.MegaCrystal))
                _crystal3Text.text = _inventory.InventoryStack[RecolectableType.MegaCrystal].ToString();
            else
                _crystal3Text.text = "0";
        }

		private void FXVolumeChange(float value)
		{
			_mixer.SetFloat("FXVolume", Mathf.Lerp(-60f, 0f, value));
		}

		private void MusicVolumeChange(float value)
		{
			_mixer.SetFloat("MusicVolume", Mathf.Lerp(-60f, 0f, value));

		}
        public void SetQuality(int index)
        {
            QualitySettings.SetQualityLevel(index, true);
            Debug.Log("Nivel de calidad cambiado a: " + QualitySettings.names[index]);
        }


        #endregion
    }

}