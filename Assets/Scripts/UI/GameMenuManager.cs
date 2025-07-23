using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] Button _retryGameButton;
    [SerializeField] Button _exitGameButton;
    [SerializeField] Button _continueGameButton;

    public  Action _onActivePauseMen;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        _retryGameButton.onClick.AddListener(RetryGame);
        _exitGameButton.onClick.AddListener(ExitGame);
        _continueGameButton.onClick.AddListener(ContinueGame);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
    private void ContinueGame()
    {
        _onActivePauseMen?.Invoke(); // Llama al evento para pausar el juego
    }
    private void MenuPause()
    {
        Time.timeScale = 1f; // Continúa el juego
    }
    #endregion
}
