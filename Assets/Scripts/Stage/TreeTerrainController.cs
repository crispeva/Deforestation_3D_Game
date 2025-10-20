using UnityEngine;
using System;
using System.Collections.Generic;

namespace Deforestation
{

	public class TreeTerrainController : MonoBehaviour
	{
		#region Properties
		public TreeInstance[] Trees => _trees;
		#endregion

		#region Fields
		[SerializeField] private Tree _treeDetectionPrefab;
		[SerializeField] private Tree _treePrefab;
		private TreeInstance[] _trees;
		Terrain _terrain;
		#endregion

		#region Unity Callbacks
		// Start is called before the first frame update
		void Start()
		{
			_terrain = Terrain.activeTerrain;
			_trees = _terrain.terrainData.treeInstances;

			InitializeTrees();
		}

		private void InitializeTrees()
		{
			for (int i = _trees.Length - 1; i >= 0; i--)
			{
				TreeInstance tree = _trees[i];
				Vector3 treeWorldPos = TreeToWorldPosition(tree);
				Tree treeDetector = Instantiate(_treeDetectionPrefab, treeWorldPos, Quaternion.identity);
				treeDetector.transform.parent = transform;
				treeDetector.Index = i;
			}
		}

		public GameObject DestroyTree(int i, Vector3 treeWorldPos)
		{
			//create tree
			Tree newTree = Instantiate(_treePrefab, treeWorldPos, Quaternion.identity);

			RemoveTreeFromTerrain(i);
			return newTree.gameObject;
		}

		void OnDestroy()
		{
			if (_trees != null)
			{
				_terrain.terrainData.treeInstances = _trees;
			}
		}
		#endregion

		#region Public Methods
		public Vector3 TreeToWorldPosition(TreeInstance tree)
		{
			return Vector3.Scale(tree.position, _terrain.terrainData.size) + _terrain.transform.position;
		}
		public void RemoveTreeFromTerrain(int index)
		{
			List<TreeInstance> trees = new List<TreeInstance>(_terrain.terrainData.treeInstances);
            if (index < 0 || index >= trees.Count)
            {
                //Debug.LogError($"Índice de árbol fuera de rango: {index}. Total de árboles: {trees.Count}");
                return;
            }
            trees.RemoveAt(index);
			_terrain.terrainData.treeInstances = trees.ToArray();

            // Elimina el detector correspondiente
            if (index < transform.childCount)
            {
                Destroy(transform.GetChild(index).gameObject);
            }

            // Reasigna los índices de los detectores restantes
            for (int i = 0; i < transform.childCount; i++)
            {
                Tree tree = transform.GetChild(i).GetComponent<Tree>();
                if (tree != null)
                    tree.Index = i;
            }
        }
		#endregion

		#region Private Methods

		#endregion
	}
}
