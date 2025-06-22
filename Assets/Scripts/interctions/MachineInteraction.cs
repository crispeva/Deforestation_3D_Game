using UnityEngine;
using System.Collections;
namespace Deforestation.Interaction
{
	public enum MachineInteractionType
	{
		Door,
		Stairs,
		Machine
	}
	public class MachineInteraction : MonoBehaviour, IInteractable
	{
		#region Properties
		#endregion

		#region Fields
		[SerializeField] protected MachineInteractionType _type;
		[SerializeField] protected Transform _target;
        Vector3 _originalposition;

		[SerializeField] protected InteractableInfo _interactableInfo;


        #endregion

        #region Public Methods
        private void Start()
        {
            _originalposition = transform.position;
        }
        public InteractableInfo GetInfo()
		{
			_interactableInfo.Type = _type.ToString();
			return _interactableInfo;
		}

		public virtual void Interact()
		{
			if (_type == MachineInteractionType.Door)
			{
				StartCoroutine(CloseDoor());
               

            }
			if (_type == MachineInteractionType.Stairs)
			{
				//Teleport Player
				GameController.Instance.TeleportPlayer(_target.position);
			}
			if (_type == MachineInteractionType.Machine)
			{
				GameController.Instance.MachineMode(true);
			}
		}

        private IEnumerator CloseDoor()
        {
            //Move Door
            transform.position = _target.position;
            yield return new WaitForSeconds(3f);
            transform.localPosition = new Vector3(0,0,0);
        }

        #endregion
    }

}