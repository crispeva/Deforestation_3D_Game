using System;
using UnityEngine;
namespace Deforestation.Events
{
    public class WinEvent : MonoBehaviour
    {
        #region Properties
        public event Action<CanvasGroup> OnWin;
        public event Action OnWinMusic;
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
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Machine"))
            {
                Debug.Log("Evento de victoria lanzado");
                OnWin?.Invoke(_canvasGroup);
                OnWinMusic?.Invoke();
            }
        }
        #endregion
    }
}
