using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Water_Volume;

public class GameMenuManager : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] private Button _retryGameButton;
    [SerializeField] private Button _exitGameButton;
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _settingsButton;
    public  Action _onActivePauseMenu;
    public  Action _onActiveSettingsMenu;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        _retryGameButton.onClick.AddListener(RetryGame);
        _exitGameButton.onClick.AddListener(ExitGame);
        _continueGameButton.onClick.AddListener(ContinueGame);
        _settingsButton.onClick.AddListener(SwitchSettings);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);// Reinicia el juego cargando la escena actual
    }
    private void SwitchSettings()
    {
        _onActiveSettingsMenu?.Invoke();
    }
    private void ExitGame()
    {
        Application.Quit();
    }
    private void ContinueGame()
    {
        _onActivePauseMenu?.Invoke(); // Llama al evento para pausar el juego
    }

    #endregion
}
