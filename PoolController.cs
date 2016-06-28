using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

[Serializable]
public class PoolController : MonoBehaviour {
	[SerializeField]
	private BasePool pool;

	//Which position in the poolTypes array was selected
	[SerializeField]
	private int selectedType = 0;
	[SerializeField]
	private int typeStash = 0;

	//Matching arrays of poolTypes and their names (could refactor to struct)
	private string[] poolStringTypes;
	private Type[] poolTypes;


	public void OnInspectorGUI()
	{
		EditorGUILayout.LabelField ("Select a pooling system");
		storePoolTypes();

		selectedType = EditorGUILayout.Popup(selectedType, poolStringTypes);

		//Check to see if a new pooling system was selected from the list
		if (GUI.changed) {
			if (!selectedType.Equals(typeStash)) {
				typeStash = selectedType;
				ScriptableObject.DestroyImmediate (pool);
			}
		}
			
		if (pool == null) {
			pool = (BasePool)ScriptableObject.CreateInstance(poolTypes[selectedType]);
		}
		pool.OnInspectorGUI ();
	}

	void Start () {
		pool.OnStart();
		InvokeRepeating ("createOnTimestep", 0, pool.getCreationTimestep());
	}
		
	void Update () {
		pool.recycleObjects();
	}

	private void createOnTimestep(){
		pool.createOnTimestep();
	}

	//Stash the objects that inherit from BasePool to use as the list for seletion in the editor
	private void storePoolTypes(){
		if (poolTypes == null) {
			poolTypes = BasePool.getSubclasses ();
			poolStringTypes = poolTypes.Select(x => x.Name).ToArray();
		}
	}
}
