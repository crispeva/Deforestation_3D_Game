using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DistanceEvents : MonoBehaviour
{
    #region Properties

    // Events
    public event Action OnEventVillage;
    public event Action OnExitEventVillage;
    #endregion

    #region Fields
    [SerializeField] private Transform _player;
    private bool _isPlayerInTrigger = false;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Machine"))
        {
           // _isPlayerInTrigger = true;
            Debug.Log("Evento lanzado");
            OnEventVillage?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Machine"))
        {
            Debug.Log("Evento de vuelta");
            OnExitEventVillage?.Invoke();
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion
}
