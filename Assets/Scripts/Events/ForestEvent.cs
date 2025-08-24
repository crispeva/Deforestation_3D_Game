using System;
using UnityEngine;
namespace Deforestation.Events
{

    public class ForestEvent : MonoBehaviour
    {
        #region Properties
        public event Action<CanvasGroup> OnForestEvent;
        public event Action<CanvasGroup> OnExitForestEvent;
        #endregion

        #region Fields
        [SerializeField] private CanvasGroup _canvasGroup;
        #endregion

        #region Unity Callbacks
        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
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
            if (other.CompareTag("Player") || other.CompareTag("Machine"))
            {
                Debug.Log("Evento de forest lanzado");
                OnForestEvent?.Invoke(_canvasGroup);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Machine"))
            {
                Debug.Log("Evento de vuelta");
                OnExitForestEvent?.Invoke(_canvasGroup);
            }
        }
        #endregion
    }
}
