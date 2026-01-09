using UnityEditor;
using UnityEngine;

public class TagSearchWindow : EditorWindow
{
    string tagName = "Enemy";

    [MenuItem("Tools/Tag Search")]
    public static void ShowWindow()
    {
        GetWindow<TagSearchWindow>("Tag Search");
    }

    void OnGUI()
    {
        tagName = EditorGUILayout.TextField("Tag:", tagName);

        if (GUILayout.Button("Find Objects"))
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);
            foreach (GameObject obj in objs)
            {
                Selection.activeGameObject = obj; // Hierarchy에서 선택
                Debug.Log("Found: " + obj.name, obj);
            }
        }
    }
}