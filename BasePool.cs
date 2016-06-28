using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[Serializable]
public abstract class BasePool : ScriptableObject {
	//The source object for the Pool
	[SerializeField]
	protected GameObject poolObject;

	//Number of objects built into pool
	[SerializeField]
	protected int poolSize;

	//Position new objects should start from
	[SerializeField]
	protected Vector3 poolStartPosition;

	//Whether or not active objects should be reset when they are the oldest
	[SerializeField]
	protected bool recycleActive;

	//How often an old object should be activated
	[SerializeField]
	protected float creationTimestep;

	private static Type[] types;

	//Use to reactivate the oldest object in the pool
	protected abstract void resetOldestObject();

	//Use to deactivate old objects based on pooling type
	protected abstract void disableOldObjects();

	//Use to initialize the pool of objects
	protected abstract void buildObjectPool();

	public float getCreationTimestep(){
		return creationTimestep;
	}

	public void recycleObjects(){
		disableOldObjects();
	}

	public void createOnTimestep(){
		resetOldestObject();
	}

	public void OnStart(){
		buildObjectPool();
	}

	public virtual void OnInspectorGUI(){
		poolObject = (GameObject)EditorGUILayout.ObjectField ("Pool Object", poolObject, typeof(GameObject));
		poolSize = EditorGUILayout.IntField("Pool Size", poolSize);
		poolStartPosition = EditorGUILayout.Vector3Field ("Pool Start Position", poolStartPosition);
		recycleActive = EditorGUILayout.Toggle ("Recycle Active Objects", recycleActive);
		creationTimestep = EditorGUILayout.FloatField ("Creation Timestep", creationTimestep);
	}

	//Find all the types that inherit from BasePool
	public static Type[] getSubclasses(){
		if (types == null) {
			var baseType = typeof(BasePool);
			types = Assembly.GetExecutingAssembly ()
				.GetTypes ()
				.Where (x => baseType.IsAssignableFrom(x))
				.Where(x => baseType != x)
				.ToArray ();
		}
		return types;
	}

}
