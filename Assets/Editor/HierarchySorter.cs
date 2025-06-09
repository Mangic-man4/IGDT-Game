#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class HierarchySorter : MonoBehaviour
{
    [MenuItem("Tools/Sort Selected GameObject's Children By Name")]
    private static void SortChildrenByName()
    {
        GameObject parent = Selection.activeGameObject;
        if (parent == null)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        // Store children in an array and sort them
        Transform[] children = new Transform[parent.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = parent.transform.GetChild(i);
        }

        System.Array.Sort(children, (a, b) => a.name.CompareTo(b.name));

        // Reorder them by setting sibling index
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }

        Debug.Log($"Sorted {children.Length} children of '{parent.name}' by name.");
    }
}
#endif
