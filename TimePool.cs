using System;
using UnityEngine;
using UnityEditor;

//TimePool is a pooling system that deactivates pool objects based on a set object lifetime
[Serializable]
public class TimePool : BasePool {

	//Custom struct to keep track of time that GameObject is activated
	public struct TimedGameObject
	{
		public TimedGameObject(GameObject obj, Nullable<float> actTime){
			objectInstance = obj;
			activationTime = actTime;
		}
		public GameObject objectInstance;
		public Nullable<float> activationTime;
	}

	//How long objects should remain active
	[SerializeField]
	private float lifetime;

	//Pool Data Structure
	private TimedGameObject[] objects;

	//Position of object array that has the oldest object
	//Used to know what next object should be used when an object is reactivated
	private int oldestObjectPosition;

	//Position of object array after the last deactivated object
	//Used to efficiently deactivate objects that reach their lifetime
	private int afterDeactivatedPosition;

	protected override void resetOldestObject(){
		if (recycleActive || !objects[oldestObjectPosition].objectInstance.activeSelf) {
			objects[oldestObjectPosition].objectInstance.transform.position = poolStartPosition;
			objects [oldestObjectPosition].activationTime = Time.fixedTime;
			objects [oldestObjectPosition].objectInstance.SetActive (true);
			oldestObjectPosition = (oldestObjectPosition + 1 > objects.Length-1) ? 0 : oldestObjectPosition + 1;
		}
	}
		
	protected override void disableOldObjects(){
		// steps through array using afterDeactivatedPosition as a reference point
		while (objects.Length > 0 &&
			  objects[afterDeactivatedPosition].objectInstance.activeSelf == true &&
		      objects[afterDeactivatedPosition].activationTime != null &&
			  Time.fixedTime - objects[afterDeactivatedPosition].activationTime > lifetime) {
			objects[afterDeactivatedPosition].objectInstance.SetActive (false);
			afterDeactivatedPosition = (afterDeactivatedPosition + 1 > objects.Length-1) ? 0 : afterDeactivatedPosition + 1;
		}
	}

	protected override void buildObjectPool(){
		objects = new TimedGameObject[poolSize];
		for(int i = 0; i < poolSize; i++){
			GameObject newObject = (GameObject)Instantiate(poolObject, poolStartPosition, Quaternion.identity);
			newObject.SetActive (false);
			TimedGameObject timedObject = new TimedGameObject (newObject, null);
			objects[i] = timedObject;
		}
		afterDeactivatedPosition = 0;
		oldestObjectPosition = 0;
	}

	public override void OnInspectorGUI(){
		base.OnInspectorGUI ();
		lifetime = EditorGUILayout.FloatField ("Lifetime", lifetime);
	}
}
