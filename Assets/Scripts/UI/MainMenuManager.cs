using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] Button _startGameButton;
    [SerializeField] Button _exitGameButton;
    [SerializeField] Button _multiplayerGameButton;

    #endregion

    #region Unity Callbacks
    void Start()
    {
        _startGameButton.onClick.AddListener(StartGame);
        _exitGameButton.onClick.AddListener(ExitGame);
        _multiplayerGameButton.onClick.AddListener(MultiplayerGame);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void StartGame()
    {
        SceneManager.LoadScene("Main Scene");
    }
    private void MultiplayerGame()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    private void ExitGame()
    {
        Application.Quit();
    }
        #endregion
    }
