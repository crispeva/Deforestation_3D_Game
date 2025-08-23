using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageEventTrigger : MonoBehaviour
{
    #region Properties

    // Events
    public event Action<CanvasGroup> OnEventVillage;
    public event Action  OnEventVillagePanel;
    public event Action<CanvasGroup> OnExitEventVillage;
    #endregion

    #region Fields
    [SerializeField] private CanvasGroup _canvasGroup;
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
            Debug.Log("Evento lanzado");
            OnEventVillage?.Invoke(_canvasGroup);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Machine"))
        {
            Debug.Log("Evento de vuelta");
            OnExitEventVillage?.Invoke(_canvasGroup);
            OnEventVillagePanel?.Invoke();
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion
}
