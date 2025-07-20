using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DIEMenuManager : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] Button _startGameButton;
    [SerializeField] Button _exitGameButton;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        _startGameButton.onClick.AddListener(StartGame);
        _exitGameButton.onClick.AddListener(ExitGame);
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
    private void ExitGame()
    {
        Application.Quit();
    }
        #endregion
    }
