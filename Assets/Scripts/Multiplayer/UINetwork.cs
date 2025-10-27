using System.Collections;
using System.Collections.Generic;
using Deforestation;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UINetwork : MonoBehaviour
{
    #region Properties
    [Header("Multiplayer")]
    [SerializeField] private GameObject _connectingPanel;
    [SerializeField] private CanvasGroup _winPanel;
    [SerializeField] private CanvasGroup _drawPanel;
    [SerializeField] private TextMeshProUGUI _player_winner;
    [SerializeField] private TextMeshProUGUI _player;
    [SerializeField] private Button _buttonExit;
    [SerializeField] private Button _buttonRetry;
    #endregion


    #region Unity Callbacks
    void Start()
    {
        _buttonExit.onClick.AddListener(Exit);
        _buttonRetry.onClick.AddListener(Retry);
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
    internal void ShowVictoryScreen(string nickName)
    {
        StartCoroutine(GameController.Instance.UIGameController.FadeIn(_winPanel, 2f));
        _player_winner.text = nickName;

    }
    internal void ShowDrawScreen()
    {
        StartCoroutine(GameController.Instance.UIGameController.FadeIn(_drawPanel, 2f));

    }
    internal void PlayerScreen(string nickName)
    {
        _player.text = nickName;

    }
    #endregion

    #region Private Methods
    private void Retry()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
    private void Exit()
    {
        Debug.Log("Quit Application");

        Application.Quit();
    }
    #endregion
}
