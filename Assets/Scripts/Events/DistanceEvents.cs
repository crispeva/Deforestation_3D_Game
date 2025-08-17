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
    #endregion

    #region Fields
    [SerializeField] private Transform _player;
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
        if (other.transform == _player)
        {
            Debug.Log("Evento lanzado");
            OnEventVillage?.Invoke();
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion
}
