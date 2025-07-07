using UnityEngine;

namespace Deforestation.Dinosaurus
{
	public class Stegasaurus : Dinosaur
	{
        #region Fields
        //[SerializeField] private float _radiusMovement = 100f;
        #endregion

        #region Properties
        #endregion

        #region Unity Callbacks	

        protected override void  Update()
		{
			if (!_agent.pathPending)
			{ // Asegura que el agente haya calculado el camino
				if (_agent.remainingDistance <= _agent.stoppingDistance)
				{ // Comprueba si la distancia restante es menor que la distancia de parada
					if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
					{
						_anim.SetBool("Run", false);

					}
				}
			}
		}

		#endregion

		#region Private Methods

		#endregion

		#region Public Methods
		#endregion

	}
}
