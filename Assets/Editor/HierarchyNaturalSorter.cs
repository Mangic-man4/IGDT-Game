#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Linq;

public class HierarchyNaturalSorter : MonoBehaviour
{
    [MenuItem("Tools/Sort Selected GameObject's Children By Natural Name")]
    private static void SortChildrenByNaturalName()
    {
        GameObject parent = Selection.activeGameObject;
        if (parent == null)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        // Fetch children
        Transform[] children = new Transform[parent.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = parent.transform.GetChild(i);
        }

        // Sort with natural comparison
        System.Array.Sort(children, (a, b) => NaturalCompare(a.name, b.name));

        // Apply new sibling order
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }

        Debug.Log($"Naturally sorted {children.Length} children of '{parent.name}'.");
    }

    // Natural (human-like) string comparison
    private static int NaturalCompare(string a, string b)
    {
        var regex = new Regex(@"\d+|\D+");
        var aParts = regex.Matches(a).Cast<Match>().Select(m => m.Value).ToArray();
        var bParts = regex.Matches(b).Cast<Match>().Select(m => m.Value).ToArray();

        int i = 0;
        while (i < aParts.Length && i < bParts.Length)
        {
            if (int.TryParse(aParts[i], out int aNum) && int.TryParse(bParts[i], out int bNum))
            {
                int numCompare = aNum.CompareTo(bNum);
                if (numCompare != 0) return numCompare;
            }
            else
            {
                int strCompare = string.Compare(aParts[i], bParts[i], true);
                if (strCompare != 0) return strCompare;
            }
            i++;
        }

        return aParts.Length.CompareTo(bParts.Length);
    }
}
#endif

