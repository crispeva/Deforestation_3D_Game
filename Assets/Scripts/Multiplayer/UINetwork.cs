using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UINetwork : MonoBehaviour
{
    #region Properties
    [SerializeField] private GameObject _connectingPanel;

    #endregion

    #region Fields
    public GameObject EndGamePanel;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Public Methods
    public void LoadingComplete()
    {
        _connectingPanel.SetActive(false);
    }
    #endregion

    #region Private Methods
    private void Retry()
    {
        SceneManager.LoadScene(0);
    }
    private void Exit()
    {
        Application.Quit();
    }
    #endregion
}
