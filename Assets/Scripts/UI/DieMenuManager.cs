using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DieMenuManager : MonoBehaviour
{
    #region Properties
    #endregion

    #region Fields
    [SerializeField] Button _retryGameButton;
    [SerializeField] Button _exitGameButton;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        _retryGameButton.onClick.AddListener(RetryGame);
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
    private void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}
