using UnityEditor;


[CustomEditor(typeof(PoolController))]
public class PoolControllerEditor : Editor {

	public override void OnInspectorGUI()
	{
		PoolController myTarget = (PoolController)target;
		myTarget.OnInspectorGUI();
	}
}
