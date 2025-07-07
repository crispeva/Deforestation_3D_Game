using System;
using UnityEngine;
using UnityEngine.AI;

namespace Deforestation.Dinosaurus
{
	public class Pachycephalasaurus : Dinosaur
	{
		#region Fields
        protected override Vector3 _targetPosition => GameController.Instance.MachineController.transform.position;
        #endregion

        #region Properties
        #endregion

        #region Unity Callbacks	
        protected override void Start()
		{
			base.Start();
			_attackDamage = 5;
            _attackTime = 2;
            _attackDistance = 10;
            _distanceDetection = 100;
        }


		#endregion

		#region Private Methods
		
		#endregion

	}

}