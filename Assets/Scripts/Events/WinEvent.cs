using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinEvent : MonoBehaviour
{
    #region Properties
    public event Action OnWin;
    #endregion

    #region Fields
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
    #endregion

    #region Private Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")|| other.CompareTag("Machine"))
        {
            Debug.Log("Evento de victoria lanzado");
            OnWin?.Invoke();
        }
    }
    #endregion
}
