using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Array : MonoBehaviour
{
    public GameObject originalObject;
    public int count = 5;
    public Vector3 offset = new Vector3(2, 0, 0);

    private Transform clonesParent;

    public void GenerateArray()
    {
        if (originalObject == null)
        {
            return;
        }
        
        if (clonesParent == null)
        {
            GameObject container = new GameObject($"{originalObject.name}_Array");
            container.transform.SetParent(transform);
            clonesParent = container.transform;
        }
        else
        {
            ClearClones();
        }

        Vector3 basePosition = originalObject.transform.position;
        Quaternion baseRotation = originalObject.transform.rotation;

        int startIndex = 1;

        for (int i = startIndex; i < count; i++)
        {
            Vector3 pos = basePosition + offset * i;

            GameObject clone;

#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabAsset(originalObject))
                clone = (GameObject)PrefabUtility.InstantiatePrefab(originalObject);
            else
#endif
                clone = Instantiate(originalObject);

            if (clone == null)
            {
                return;
            }

            clone.transform.position = pos;
            clone.transform.rotation = baseRotation;
            clone.transform.SetParent(clonesParent);
            clone.name = $"{originalObject.name}_Clone_{i}";
            
            Array script = clone.GetComponent<Array>();
            if (script != null)
                DestroyImmediate(script);
        }
    }

    public void ClearClones()
    {
        if (clonesParent == null) return;

        for (int i = clonesParent.childCount - 1; i >= 0; i--)
        {
            if (Application.isEditor)
                DestroyImmediate(clonesParent.GetChild(i).gameObject);
            else
                Destroy(clonesParent.GetChild(i).gameObject);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Array))]
public class ArraySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Array spawner = (Array)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Array"))
        {
            spawner.GenerateArray();
        }
    }
}
#endif